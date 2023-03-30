﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Sector;

namespace VectorTerrain.Scripts.WorldGenerator
{
    public class WorldGeneratorAsync : MonoBehaviour
    {
        public TerrainGraph graph;
        private Dictionary<int, TerrainGraphInput> inputDict;
        private Dictionary<int,SectorController> _sectorControllerDict;

        public Dictionary<int, SectorController> SectorDict { get => _sectorControllerDict; }

        private Dictionary<int, Task> taskz;

        [Button]
        async void Init(int seed)
        {
            graph.InitNodeIDs();
        
            Globals.GlobalSeed = seed;
            inputDict = new();
            _sectorControllerDict = new();
            taskz = new();
            Globals.GlobalSeed = seed;
        
            DestroyAllSectors();
        
            float zOffset = 0;
        
            var input1 = new TerrainGraphInput(-1, zOffset);
            var input2 = new TerrainGraphInput(0, zOffset);

        
            await InstantiateSector(input1); // generate backwards sector from blank initial state, then move on
        
            var previousSectorController = await InstantiateSector(input2); // generate forwards sector from blank initial state
        
            await InstantiateSector(new TerrainGraphInput(previousSectorController)); // generate 2nd forward sector from previous forward sector
        }

        [Button]
        void Advance()
        {
            int taskID;
            if (taskz.Count == 0) taskID = 0;
            else taskID = taskz.Keys.Max() + 1;
            taskz[taskID] = AdvanceTask(taskID);
        }

        [Button]
        void Subvance()
        {
            int taskID;
            if (taskz.Count == 0) taskID = 0;
            else taskID = taskz.Keys.Max() + 1;
            taskz[taskID] = SubvanceTask(taskID);
        }

        async Task AdvanceTask(int id)
        {
        
            if (taskz.Keys.Contains(id - 1))
                await taskz[id - 1];
        
            TerrainGraphInput input;



            if (inputDict.Keys.Contains(HighestGeneration() + 1))
            {
                input = inputDict[HighestGeneration() + 1];
            }
            else
                input = new TerrainGraphInput(_sectorControllerDict[HighestGeneration()]);
            await InstantiateSector(input);
            DestroyHeadSector();
        
            taskz.Remove(id - 1);
        }
    
        async Task SubvanceTask(int id)
        {

            if (taskz.Keys.Contains(id - 1))
                await taskz[id - 1];

            TerrainGraphInput input;
        
            if (inputDict.Keys.Contains(LowestGeneration() - 1))
                input = inputDict[LowestGeneration() - 1];
            else
                input = new TerrainGraphInput(_sectorControllerDict[LowestGeneration()]);

            await InstantiateSector(input);
            DestroyTailSector();

            taskz.Remove(id - 1);
        }

        SectorController tailSector()
        {
            return _sectorControllerDict[_sectorControllerDict.Keys.Max()];
        }
    
        SectorController headSector()
        {
            return _sectorControllerDict[_sectorControllerDict.Keys.Min()];
        }

        int HighestGeneration()
        {
            return tailSector().Generation;
        }

        int LowestGeneration()
        {
            return headSector().Generation;
        }

        async Task<SectorController> InstantiateSector(TerrainGraphInput input)
        {
        
            // SectorController newSectorController;
        
            // if (inputDict.Keys.Contains(input.generation))
            //     input = inputDict[input.generation];

            var g = graph.Copy() as TerrainGraph;
            var graphOutput = await Task.Run(()=>g.GetGraphOutput(input, false));
            var newSectorController = SectorController.New(graphOutput, transform, new VisualiserConfig());

            // Debug.Log(input.generation);
        
            _sectorControllerDict[newSectorController.Generation] = newSectorController;
        
            // Debug.Log(newSectorController.Generation);
        
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
