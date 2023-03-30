using System.Collections.Generic;
using Terrain;
using UnityEngine;
using VectorTerrain.Scripts.Types;


public struct TerrainGraphInput
    {
        // public InitialStateContainer initialStateContainer;
        public Vertex2 StartPos;
        public Vertex2 StartVector;
        public float zOffset;
        
        public Dictionary<string, SeedContainer> seedDict;
        public int generation;
        
        public float totalDistanceSoFar;
        public int totalPointsSoFar;

        
        // for creating the first generation from nothing
        public TerrainGraphInput(int generation, float ZOffset) 
        {
            StartPos = new Vertex2(Vector2.zero);
            StartVector = new Vertex2(Vector2.right);
            this.zOffset = ZOffset;
            seedDict = new Dictionary<string, SeedContainer>();
            // Debug.Log("dict count " + seedDict.Count);
            this.generation = generation;
            totalDistanceSoFar = 0;
            totalPointsSoFar = 0;
        }

        // for initialising appending to previous generation
        public TerrainGraphInput(SectorController previousSectorController)
        {
            //todo properly validate the data from the graph
            
            StartPos = previousSectorController.sectorData.Verts[^1];
            StartVector = previousSectorController.sectorData.Verts[^1] - previousSectorController.sectorData.Verts[^2];
            zOffset = previousSectorController.sectorData.zOffset;
            seedDict = previousSectorController.sectorData.EndSeeds;
            generation = previousSectorController.Generation + (int)Mathf.Sign(previousSectorController.Generation);
            totalDistanceSoFar = previousSectorController.distCounter;
            totalPointsSoFar = previousSectorController.pointCounter;
        }

        public override string ToString()
        {
            string toPrint = "---TerrainGraphInput---\n";

            // toPrint += "---\n";
            toPrint += $"generation: {generation}\n";
            toPrint += ":: SeedDict ::\n";
            foreach (var v in seedDict)
            {
                toPrint += $"{v.Key} : {v.Value}\n";
            }

            toPrint += "\n\n";

            return toPrint;
        }
    }

    public class TerrainGraphOutput
    {
        public SectorData sectorData;
        public int generation;
        // public Dictionary<string, List<Plot>> PlotListDict;
        public List<Plot> plotList;
        public float totalDistanceAtStart;
        public float totalDistanceAtEnd;
        public int totalPointsAtEnd;
        // public float ZOffset;
    }


public struct SeedObject
{
    public float x;
    public float y;
    public float z;
}


    // public class VisualisationPlotContainer
    // {
    //     public Dictionary<string nodeName, List>
    // }

    // public struct InitialStateContainerr //todo not needed - refactor into obblivion
    // {
    //     public Vertex2 StartPos;
    //     public Vertex2 StartVector;
    //
    //     public float ZOffset;
    //     // public int? globalSeed;
    //     
    //     public InitialStateContainerr(float ZOffset)
    //     {
    //         StartPos = new Vertex2(Vector2.zero);
    //         StartVector = new Vertex2(Vector2.right);
    //         this.ZOffset = ZOffset;
    //         // this.globalSeed = null;
    //     }
    //
    //     public InitialStateContainerr(SectorData previousGeneration, float ZOffset)
    //     {
    //         StartPos = previousGeneration.Verts[^1];
    //         StartVector = previousGeneration.Verts[^1] - previousGeneration.Verts[^2];
    //         this.ZOffset = ZOffset;
    //     }
    // }
