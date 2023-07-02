using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Sector;
using VectorTerrain.Scripts.Sector.SectorGroupPostProcessing;
using VectorTerrain.Scripts.Terrain;
using VectorTerrain.Scripts.Tester;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;
using Random = UnityEngine.Random;

namespace VectorTerrain.Scripts
{
    public class VectorTerrainGeneratorAsync : MonoBehaviour
    {
        public bool clean;

        public int taskCount
        {
            get => _taskDict.Count;
        }

        private TerrainGraph _graph;

        private Dictionary<int, TerrainGraphInput> _inputDict;
        private Dictionary<int, SectorController> _sectorControllerDict;

        public Dictionary<int, SectorController> SectorDict
        {
            get => _sectorControllerDict;
        }

        private Dictionary<int, Task> _taskDict;

        // private List<Task> activeTasks = new();
        private Transform _terrainContainer;

        private TerrainContainerManager _terrainContainerManager;
        // private TerrainShapesRenderSettings _terrainShapesRenderSettings;

        private bool _initted = false;

        public bool Initted
        {
            get => _initted;
        }

        public SectorController middleSectorController
        {
            get
            {
                int middleKey =
                    _sectorControllerDict.Keys.ToList().OrderBy(x => x).ToList()[_sectorControllerDict.Count / 2];
                return _sectorControllerDict[middleKey];
            }
        }

        public async Task Init(int seed)
        {
            Debug.Log("Initializing VectorTerrainGeneratorAsync");

            var manager = GetComponent<VectorTerrainManager>();
            if (manager == null)
                throw new Exception("A Vector Terrain Manager is required on the same game object as the generator");
            _graph = manager.graph;

            if (_graph == null) throw new Exception("No graph assigned to VectorTerrainGeneratorAsync");

            _graph.InitNodeIDs();
            // _terrainShapesRenderSettings = gameObject.GetComponent<TerrainShapesRenderSettings>();
            _terrainContainerManager = gameObject.GetComponent<TerrainContainerManager>();

            if (_terrainContainerManager == null)
                _terrainContainerManager = gameObject.AddComponent<TerrainContainerManager>();

            _terrainContainer = _terrainContainerManager.Init();
            // VectorTerrainGlobals.GlobalSeed = seed;
            _inputDict = new();
            _sectorControllerDict = new();
            _taskDict = new();
            VectorTerrainGlobals.GlobalSeed = seed;
            DestroyAllSectors();
            float zOffset = 0;
            var input1 = new TerrainGraphInput(-1, zOffset);
            var input2 = new TerrainGraphInput(0, zOffset);
            await InstantiateSector(input1); // generate backwards sector from blank initial state, then move on
            var previousSectorController =
                await InstantiateSector(input2); // generate forwards sector from blank initial state
            await InstantiateSector(
                new TerrainGraphInput(
                    previousSectorController)); // generate 2nd forward sector from previous forward sector
            _initted = true;
            Debug.Log("initted");
        }

        public void Advance()
        {
            if (!_initted)
            {
                Debug.Log("Adbvance attempted before init");
                return;
            }

            int taskID;
            if (_taskDict.Count == 0) taskID = 0;
            else taskID = _taskDict.Keys.Max() + 1;
            _taskDict[taskID] = AdvanceTask(taskID);
        }

        public void Subvance()
        {
            if (!_initted)
            {
                Debug.Log("Subvance attempted before init");
                return;
            }

            int taskID;
            if (_taskDict.Count == 0) taskID = 0;
            else taskID = _taskDict.Keys.Max() + 1;
            _taskDict[taskID] = SubvanceTask(taskID);
        }

        async Task AdvanceTask(int id)
        {
            if (_taskDict.Keys.Contains(id - 1)) await _taskDict[id - 1];

            TerrainGraphInput input;

            if (_inputDict.Keys.Contains(HighestGeneration() + 1))
            {
                input = _inputDict[HighestGeneration() + 1];
            }
            else
            {
                input = new TerrainGraphInput(_sectorControllerDict[HighestGeneration()]);
            }

            await InstantiateSector(input);

            DestroyHeadSector();
            _taskDict.Remove(id - 1);
        }

        async Task SubvanceTask(int id)
        {
            if (_taskDict.Keys.Contains(id - 1)) await _taskDict[id - 1];
            TerrainGraphInput input;
            if (_inputDict.Keys.Contains(LowestGeneration() - 1))
                input = _inputDict[LowestGeneration() - 1];
            else
                input = new TerrainGraphInput(_sectorControllerDict[LowestGeneration()]);

            await InstantiateSector(input);
            DestroyTailSector();
            // taskDict[id-1].Dispose();
            _taskDict.Remove(id - 1);
        }

        public bool AreTasksRunning() => _taskDict.Values.Any(task => !task.IsCompleted);


        private SectorController TailSector() => _sectorControllerDict[_sectorControllerDict.Keys.Max()];

        SectorController HeadSector() => _sectorControllerDict[_sectorControllerDict.Keys.Min()];

        int HighestGeneration() => TailSector().Generation;

        int LowestGeneration() => HeadSector().Generation;


        async Task<SectorController> InstantiateSector(TerrainGraphInput input)
        {
            int gen = input.generation;
            var g = _graph.Copy() as TerrainGraph;

            // slow function, run on separate thread
            var graphOutput = await Task.Run(() => g.GetGraphOutput(input, false));

            if (clean)
            {
                if (gen > 0)
                {
                    SectorController previousSectorController;
                    SectorData previousSectorData;
                    
                    // if the previous sector has already been generated, use that
                    if (_sectorControllerDict.TryGetValue(gen - 1, out previousSectorController))
                    {
                        previousSectorData = previousSectorController.sectorData;
                        var thisSectorData = graphOutput.SectorData;
                        await Task.Run(() => TerrainGraphOutputPostProcessing.Clean(previousSectorData, thisSectorData));
                        Debug.Log( thisSectorData.Verts[^1]);
                        try
                        {
                            input.EndPos = thisSectorData.Verts[^1];
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                            throw;
                        }
                    }
                    else // otherwise, use the input to regenerate the previous sector
                    {
                        // the previous input must exist to regenerate the previous sector
                        var previousInputExists = _inputDict.TryGetValue(gen - 1, out var previousInput);
                        if (previousInputExists)
                        {
                            var previousGraphOutput = await GetGraphOutput(previousInput);
                            previousSectorData = previousGraphOutput.SectorData;
                            var thisSectorData = graphOutput.SectorData;
                            // var endPos = previousInput.EndPos;
                            // thisSectorData.SetEndPos(endPos);
                            await Task.Run(() => TerrainGraphOutputPostProcessing.Clean(previousSectorData, thisSectorData));
                            try
                            {
                                input.EndPos = thisSectorData.Verts[^1];
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e);
                                throw;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("The previous input does not exist. Cannot clean.");
                        }
                    }
                }
            }

            
            
            Random.InitState(gen);
            
            Color c = Color.HSVToRGB(Random.value, 1, 15, true);
            graphOutput.SectorData.SetColor(c);
        
            
            var newSectorController = SectorController.New(graphOutput, _terrainContainer, new VisualiserConfig());

            _sectorControllerDict[newSectorController.Generation] = newSectorController;
            
            if (!_inputDict.Keys.Contains(newSectorController.Generation))
                _inputDict[newSectorController.Generation] = input;

            return newSectorController;
        }

        async Task<TerrainGraphOutput> GetGraphOutput(TerrainGraphInput input)
        {
            var g = _graph.Copy() as TerrainGraph;

            // slow function, run on separate thread
            return await Task.Run(() => g.GetGraphOutput(input, false));
        }


        void DestroyTailSector()
        {
            var high = _sectorControllerDict.Keys.Max();
            _sectorControllerDict[high].DestroyMe();
            _sectorControllerDict.Remove(high);
        }

        void DestroyHeadSector()
        {
            var low = _sectorControllerDict.Keys.Min();
            _sectorControllerDict[low].DestroyMe();
            _sectorControllerDict.Remove(low);
        }

        void DestroyAllSectors()
        {
            foreach (var sc in GameObject.FindObjectsOfType<SectorController>())
            {
                sc.DestroyMe();
            }
        }

        void DestroyAllTasks()
        {
            foreach (var task in _taskDict.Values)
            {
                task.Dispose();
            }
        }

        void OnDestroy()
        {
            DestroyAllTasks();
        }
    }
}