using System.Collections.Generic;
using Nodez.BaseNodes;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Types.Interfaces;
using XNode;

namespace VectorTerrain.Scripts.Nodes.Geometry.Generators
{
    [CreateNodeMenu("Geometry/Generators/GeometryGeneratorNode")]
    public class GeometryGeneratorNode : TerrainNode, IReturnSectorData, IPersistentSeed
    {
        public bool LimitPoints = true;
        [Min(2)] 
        public int maxPoints = 1000;
        public bool LimitDistance;
        [Min(1)]
        public float maxDistance = 1000;

        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public AlgorithmNoodle algorithm;
        
        [Output()]
        public GeometryNoodle Output;
        public SectorData GetSectorData(TerrainGraphInput thisInput)
        {
            if (GetConnectedAlgorithm() == null) return null;
            
            // Vertex2 initVector = thisInput.StartVector;
            
            var initPoint = thisInput.StartPos;
            
            initPoint.thickness = 1;
            // initVector.thickness = 1;
            
            SectorData newSectorData = new SectorData(new List<Vertex2>{initPoint});
            
            while (!EndCondition(newSectorData.Verts, maxPoints, maxDistance))
            {
                // thisNodeSeedContainer.vectorSeed.x += 1;
                thisNodeSeedContainer.Increment();
                var result = GetConnectedAlgorithm().Result(thisNodeSeedContainer.vectorSeed, newSectorData);
                var previous = newSectorData.Verts[^1];
                Vertex2 r = result + previous;
                newSectorData.Verts.Add(r);
            }
            // Debug.Log(thisNodeSeedContainer.Ticks);
            return newSectorData;
        }
        AlgorithmNode GetConnectedAlgorithm()
        {
            NodePort port = GetPort(nameof(algorithm));
            if (!port.IsConnected) return null;
            return port.GetConnection(0).node as AlgorithmNode;
        }
        bool EndCondition(List<Vertex2> verts, int maxPoints, float maxDistance) //todo the distance should be calculated based on an averaged version of the acvtual contours, to take into account vertical distances
        {
            if (!LimitPoints && !LimitDistance)
                return true;

            if (LimitPoints)
            {
                if (verts.Count > maxPoints)
                    return true;                
            }

            if (LimitDistance)
            {
                if(verts[^1].x - TerrainGraphInput.StartPos.x > maxDistance)
                    return true;
            }
            
            return false;
        }
    }
}