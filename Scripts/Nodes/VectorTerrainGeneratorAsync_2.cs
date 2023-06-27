using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Sector;
using VectorTerrain.Scripts.Terrain;
using VectorTerrain.Scripts.Tester;

namespace VectorTerrain.Scripts.Nodes
{
    public class VectorTerrainGeneratorAsync_2 : MonoBehaviour
    {
        private TerrainGraph _graph;
        private Dictionary<int, TerrainGraphInput> _inputDict;
        // private Dictionary<int, TerrainGraphOutput> _outputDict;
        private ConcurrentDictionary<int, TerrainGraphOutput> _outputDict;
        private Dictionary<int, SectorController> _sectorControllerDict;
        private Dictionary<int, SectorController> SectorDict { get => _sectorControllerDict; }
        private Dictionary<int, Task> _taskDict;
        private Transform _terrainContainer;
        private TerrainContainerManager _terrainContainerManager;
        
        private bool _initted = false;
        public bool Initted { get => _initted; }

        
        [Button]
        public async Task Init(int seed)
        {
            Debug.Log("Initializing VectorTerrainGeneratorAsync");


            var manager = GetComponent<VectorTerrainManager>();
            if (manager == null) throw new Exception("A Vector Terrain Manager is required on the same game object as the generator");
            
            _graph = manager.graph;
            if(_graph == null) throw new Exception("No graph assigned to VectorTerrainGeneratorAsync");
            _graph.InitNodeIDs();
            
            _terrainContainerManager = gameObject.GetComponent<TerrainContainerManager>();
            if (_terrainContainerManager == null) _terrainContainerManager = gameObject.AddComponent<TerrainContainerManager>();
            
            _terrainContainer = _terrainContainerManager.Init();

            _inputDict = new();
            _sectorControllerDict = new();
            _taskDict = new();
            _outputDict = new();
            VectorTerrainGlobals.GlobalSeed = seed;
            
            DestroyAllSectors();
            
            float zOffset = 0;
            
            var inputN1 = new TerrainGraphInput(-1, zOffset);
            var input0 = new TerrainGraphInput(0, zOffset);
            
            
            await GetGraphOutputs(new List<TerrainGraphInput>(){inputN1, input0});
            await GetGraphOutput(_outputDict[0]);
            
            foreach (var key in _outputDict.Keys)
            {
                Debug.Log(key);
            }
            
            
            //
            // await InstantiateSector(input1); // generate backwards sector from blank initial state, then move on
            // var previousSectorController = await InstantiateSector(input2); // generate forwards sector from blank initial state
            // await InstantiateSector(new TerrainGraphInput(previousSectorController)); // generate 2nd forward sector from previous forward sector
            // _initted = true;
        }
        
        public async Task GetGraphOutputs(List<TerrainGraphInput> inputs)
        {
            List<Task> tasks = new();
            foreach (var input in inputs)
            {
                tasks.Add(GetGraphOutput(input));
            }
            await Task.WhenAll(tasks);
        }
        
        async Task<TerrainGraphOutput> GetGraphOutput(TerrainGraphOutput previousOutput)
        {
            if (previousOutput == null) throw new Exception("Previous output is null");
            var input = new TerrainGraphInput(previousOutput);
            return await GetGraphOutput(input);
        }
        
        async Task<TerrainGraphOutput> GetGraphOutput(TerrainGraphInput input)
        {

            if(_outputDict.Keys.Contains(input.generation))
                return _outputDict[input.generation];
            
            int gen = input.generation;
            
            var g = _graph.Copy() as TerrainGraph;
            
            // slow function, run on separate thread
            var graphOutput = await Task.Run(()=>g.GetGraphOutput(input, false));
            
            
            // Debug.Log(gen);
            _outputDict[gen] = graphOutput;
            // _outputDict.Add(gen, graphOutput);
            
            // _outputDict[gen] = graphOutput;

            if(!_inputDict.Keys.Contains(graphOutput.Generation))
                _inputDict[graphOutput.Generation] = input;
            return graphOutput;
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