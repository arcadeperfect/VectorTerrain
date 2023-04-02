using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Sector;
using VectorTerrain.Scripts.Terrain;

namespace VectorTerrain.Scripts
{
    public class VectorTerrainGeneratorAsync : MonoBehaviour
    {
        public int taskCount { get => taskz.Count; }
        
        public TerrainGraph graph;

        private Dictionary<int, TerrainGraphInput> inputDict;
        
        public Dictionary<int,SectorController> sectorControllerDict;
        public Dictionary<int, SectorController> SectorDict { get => sectorControllerDict; }

        private Dictionary<int, Task> taskz;
        private List<Task> activeTasks = new();
        private Transform _terrainContainer;
        private TerrainContainerManager _terrainContainerManager;
        private TerrainShapesRenderSettings _terrainShapesRenderSettings;

        private bool _initted = false;
        public bool Initted { get => _initted; }
        
        public SectorController middleSectorController
        {
            get
            {
                int middleKey = sectorControllerDict.Keys.ToList().OrderBy(x => x).ToList()[sectorControllerDict.Count / 2];
                return sectorControllerDict[middleKey];
            }
        }
        public async Task Init(int seed)
        {
            Debug.Log("Initializing VectorTerrainGeneratorAsync");
            
            if(graph == null) throw new Exception("No graph assigned to VectorTerrainGeneratorAsync");
            
            graph.InitNodeIDs();
            _terrainShapesRenderSettings = gameObject.GetComponent<TerrainShapesRenderSettings>();
            _terrainContainerManager = gameObject.GetComponent<TerrainContainerManager>();
            
            if (_terrainContainerManager == null)
                _terrainContainerManager = gameObject.AddComponent<TerrainContainerManager>();

            _terrainContainer = _terrainContainerManager.Init();
            VectorTerrainGlobals.GlobalSeed = seed;
            inputDict = new();
            sectorControllerDict = new();
            taskz = new();
            VectorTerrainGlobals.GlobalSeed = seed;
            DestroyAllSectors();
            float zOffset = 0;
            var input1 = new TerrainGraphInput(-1, zOffset);
            var input2 = new TerrainGraphInput(0, zOffset);
            await InstantiateSector(input1); // generate backwards sector from blank initial state, then move on
            var previousSectorController = await InstantiateSector(input2); // generate forwards sector from blank initial state
            await InstantiateSector(new TerrainGraphInput(previousSectorController)); // generate 2nd forward sector from previous forward sector
            _initted = true;
        }
        public void Advance()
        {
            if (!_initted)
            {
                Debug.Log("Adbvance attempted before init");
                return;
            }
            int taskID;
            if (taskz.Count == 0) taskID = 0;
            else taskID = taskz.Keys.Max() + 1;
            taskz[taskID] = AdvanceTask(taskID);
        }
        public void Subvance()
        {
            if (!_initted)
            {
                Debug.Log("Subvance attempted before init");
                return;
            }
            int taskID;
            if (taskz.Count == 0) taskID = 0;
            else taskID = taskz.Keys.Max() + 1;
            taskz[taskID] = SubvanceTask(taskID);
        }

        async Task AdvanceTask(int id)
        {
            if (taskz.Keys.Contains(id - 1)) await taskz[id - 1];
        
            TerrainGraphInput input;

            if (inputDict.Keys.Contains(HighestGeneration() + 1))
            {
                input = inputDict[HighestGeneration() + 1];
            }
            else
            {
                input = new TerrainGraphInput(sectorControllerDict[HighestGeneration()]);
            }

            await InstantiateSector(input);
            
            DestroyHeadSector();
            taskz.Remove(id - 1);
        }
    
        async Task SubvanceTask(int id)
        {
            if (taskz.Keys.Contains(id - 1)) await taskz[id - 1];
            TerrainGraphInput input;
            if (inputDict.Keys.Contains(LowestGeneration() - 1))
                input = inputDict[LowestGeneration() - 1];
            else
                input = new TerrainGraphInput(sectorControllerDict[LowestGeneration()]);

            await InstantiateSector(input);
            DestroyTailSector();
            taskz.Remove(id - 1);
        }
        
        public bool AreTasksRunning()
        {
            return taskz.Values.Any(task => !task.IsCompleted);
        }


        private SectorController TailSector()
        {
            return sectorControllerDict[sectorControllerDict.Keys.Max()];
        }
    
        SectorController HeadSector()
        {
            return sectorControllerDict[sectorControllerDict.Keys.Min()];
        }

        int HighestGeneration()
        {
            return TailSector().Generation;
        }

        int LowestGeneration()
        {
            return HeadSector().Generation;
        }

        async Task<SectorController> InstantiateSector(TerrainGraphInput input)
        {
            var g = graph.Copy() as TerrainGraph;
            
            // slow function, run on separate thread`
            var graphOutput = await Task.Run(()=>g.GetGraphOutput(input, false));
            
            // could be expensive but must run on main thread. Coroutines?
            var newSectorController = SectorController.New(graphOutput, _terrainContainer, new VisualiserConfig());
            
            sectorControllerDict[newSectorController.Generation] = newSectorController;
            if(!inputDict.Keys.Contains(newSectorController.Generation))
                inputDict[newSectorController.Generation] = input;
        
            return newSectorController;
        }
        void DestroyTailSector()
        {
            var high = sectorControllerDict.Keys.Max();
            sectorControllerDict[high].DestroyMe();
            sectorControllerDict.Remove(high);
        }
        void DestroyHeadSector()
        {
            var low = sectorControllerDict.Keys.Min();
            sectorControllerDict[low].DestroyMe();
            sectorControllerDict.Remove(low);
        }
        void DestroyAllSectors()
        {
            foreach (var sc in GameObject.FindObjectsOfType<SectorController>())
            {
                sc.DestroyMe();
            }
        }
    }
}
