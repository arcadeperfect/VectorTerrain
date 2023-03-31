using System;
using System.Collections.Generic;
using Nodez.Nodes;
using Sirenix.Utilities;
using Terrain;
using UnityEngine;
using VectorTerrain.Scripts.Nodes;
using VectorTerrain.Scripts.Types;
using XNode;

namespace VectorTerrain.Scripts.Graph
{
    [CreateAssetMenu(fileName = "Terrain Graph", menuName = "Vector Terrain/Terrain Graph")]
    public class TerrainGraph : NodeGraph
    {
        [HideInInspector] public int startPoints;
        [HideInInspector] public float startDist;

        [HideInInspector] public List<Plot> graphPlotList = new();

        public TerrainGraphInput terrainGraphInput;
        public int GlobalSeed { get; private set; }

        public int Generation { get; private set; }

        public OutputNode GraphOutputNode
        {
            get
            {
                OutputNode outputNode = null;
                foreach (var node in nodes)
                    if (node.GetType() == typeof(OutputNode))
                        outputNode = node as OutputNode;

                if (outputNode is not null) return outputNode;
                throw new NoOutputNodeException();
            }
        }

        public event Action NodesUpdated;
        public event Action NodesChanged;
        public event Action GenerationStart;

        public event Action GenerationEnd;
        // public event Action Tick;


        public override Node AddNode(Type type)
        {
            Node.graphHotfix = this;
            var node = CreateInstance(type) as Node;
            node.graph = this;
            nodes.Add(node);
            NodesChanged?.Invoke(); // todo happening too early? doesn't always work
            return node;
        }

        public override void RemoveNode(Node node)
        {
            NodesChanged?.Invoke();
            base.RemoveNode(node);
        }

        public void InitNodeIDs()
        {
            foreach (var node in nodes)
            {
                var n = node as TerrainNode;
                if (n != null) n.ID = n.GetInstanceID().ToString();
            }
        }


        public Dictionary<string, SeedContainer> GetSeeds()
        {
            // Dictionary<Guid, SeedContainer> returnDict = new Dictionary<Guid, SeedContainer>();
            var returnDict = new Dictionary<string, SeedContainer>();
            foreach (var node in nodes)
                // if (node.GetType().InheritsFrom(typeof(TerrainNode)) && node.GetType().ImplementsOpenGenericInterface(typeof(IPersistentSeed)))
                if (node.GetType().InheritsFrom(typeof(TerrainNode)))
                {
                    var n = node as TerrainNode;
                    var id = n.ID;
                    var seed = n.SeedContainer;
                    returnDict[id] = seed;
                }
            return returnDict;
        }

        public bool SetSeeds(Dictionary<string, SeedContainer> newSeedsDict)
        {
            // Debug.Log("dict count " + newSeedsDict.Count);

            foreach (var node in nodes)
                if (node.GetType().InheritsFrom(typeof(TerrainNode)))
                {
                    var terrainNode = node as TerrainNode;
                    // var id = terrainNode.IDD;
                    var id = terrainNode.ID;

                    if
                        (newSeedsDict.Count == 0) terrainNode.resetSeedContainer();

                    else if
                        (newSeedsDict.ContainsKey(id)) terrainNode.SeedContainer = newSeedsDict[id];

                    else
                        throw new GuidMismatchException();
                }
            return true;
        }

        /// <summary>
        ///     Method for extracting data from the graph
        /// </summary>
        /// <param name="input"></param>
        /// <param name="viz"></param>
        /// <returns></returns>
        /// <exception cref="NodeIDsNotInitialisedException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public TerrainGraphOutput GetGraphOutput(TerrainGraphInput input, bool viz)
        {
            graphPlotList.Clear();
            foreach (TerrainNode node in nodes)
            {
                if (node == null) continue;
                if (string.IsNullOrEmpty(node.ID))
                    throw new NodeIDsNotInitialisedException(
                        "You must run InitNodeIDs externally, as it it is not thread safe and GetGraphOutput will often be run on other threads");
            }

            terrainGraphInput = input;
            startPoints = terrainGraphInput.totalPointsSoFar;
            startDist = terrainGraphInput.totalDistanceSoFar;

            var returnGraphOutput = new TerrainGraphOutput();
            returnGraphOutput.totalDistanceAtStart = input.totalDistanceSoFar;

            SetSeeds(input.seedDict);
            Generation = input.generation;
            var backwards = input.generation < 0; // if we are going backwards from the origin
            GlobalSeed = VectorTerrainGlobals.GlobalSeed;

            if (backwards) GlobalSeed += 123;
            if (backwards) terrainGraphInput.StartVector.x *= -1f;


            GenerationStart?.Invoke();
            var retrievedSectorData = GraphOutputNode.GetSectorData(terrainGraphInput);
            if (retrievedSectorData == null) return null;
            retrievedSectorData.Process();
            if (backwards) retrievedSectorData.MakeBackwards();
            // if (backwards) retrievedSectorData = ReverseVerts(retrievedSectorData);
            GenerationEnd?.Invoke();

            retrievedSectorData.EndSeeds = GetSeeds();
            retrievedSectorData.zOffset = terrainGraphInput.zOffset;

            returnGraphOutput.sectorData = retrievedSectorData;
            returnGraphOutput.generation = input.generation;

            // if (retrievedSectorData.TotalLengh == null)
            // 	throw new NullReferenceException("outputSector distances were not computed");

            returnGraphOutput.totalDistanceAtEnd = retrievedSectorData.TotalLengh + startDist;
            returnGraphOutput.totalPointsAtEnd = retrievedSectorData.Verts.Count + startPoints;

            // Debug.Log(graphPlotList[0].YVals[0]);


            returnGraphOutput.plotList = graphPlotList;

            return returnGraphOutput;
        }

        // SectorData ReverseVerts(SectorData sectorData)
        // {
        // 	List<Vertex2> reversedVerts = new List<Vertex2>();
        // 	var pivot = sectorData.Verts[0].Pos;
        // 	
        // 	foreach (var vert in sectorData.Verts)
        // 	{
        // 		var v = vert;
        // 		v.x -= pivot.x;
        // 		v.x *= -1;
        // 		v.x += pivot.x;
        // 		reversedVerts.Add(v);
        // 	}
        // 	sectorData.Verts = reversedVerts;
        //
        // 	sectorData.LocalStart.x *= -1;
        // 	
        // 	return sectorData;
        // }


        public void OnNodesUpdated()
        {
            NodesUpdated?.Invoke();
        }

        public void OnNodesChanged()
        {
            NodesChanged?.Invoke();
        }
    }

    public class NodeIDsNotInitialisedException : Exception
    {
        public NodeIDsNotInitialisedException(string message)
            : base(message)
        {
        }
    }
}