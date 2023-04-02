﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Sector;
using VectorTerrain.Scripts.Terrain;

namespace VectorTerrain.Scripts
{
    public class VectorTerrainGeneratorAsync : MonoBehaviour, ITerrainGenerator
    {
        public TerrainGraph graph;

        private Dictionary<int, TerrainGraphInput> inputDict;
        
        public Dictionary<int,SectorController> _sectorControllerDict;
        public Dictionary<int, SectorController> SectorDict { get => _sectorControllerDict; }

        private Dictionary<int, Task> taskz;
        private Transform _terrainContainer;
        private TerrainContainerManager _terrainContainerManager;
        private TerrainShapesRenderSettings _terrainShapesRenderSettings;

        private bool _initted = false;
        public bool Initted { get => _initted; }
        
        public SectorController middleSectorController
        {
            get
            {
                int middleKey = _sectorControllerDict.Keys.ToList().OrderBy(x => x).ToList()[_sectorControllerDict.Count / 2];
                return _sectorControllerDict[middleKey];
            }
        }
        public async void Init(int seed)
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
            _sectorControllerDict = new();
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
                input = new TerrainGraphInput(_sectorControllerDict[HighestGeneration()]);
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
                input = new TerrainGraphInput(_sectorControllerDict[LowestGeneration()]);

            await InstantiateSector(input);
            DestroyTailSector();
            taskz.Remove(id - 1);
        }

        private SectorController TailSector()
        {
            return _sectorControllerDict[_sectorControllerDict.Keys.Max()];
        }
    
        SectorController HeadSector()
        {
            return _sectorControllerDict[_sectorControllerDict.Keys.Min()];
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
            var graphOutput = await Task.Run(()=>g.GetGraphOutput(input, false));
            var newSectorController = SectorController.New(graphOutput, _terrainContainer, new VisualiserConfig());
            _sectorControllerDict[newSectorController.Generation] = newSectorController;
            if(!inputDict.Keys.Contains(newSectorController.Generation))
                inputDict[newSectorController.Generation] = input;
        
            return newSectorController;
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
    }
}
