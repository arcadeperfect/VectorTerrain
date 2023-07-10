using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nodez.BaseNodes;
using UnityEngine;
using UnityEngine.Serialization;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Types.Interfaces;
using XNode;
using Debug = UnityEngine.Debug;

namespace VectorTerrain.Scripts.Nodes.Geometry.Generators
{
    [CreateNodeMenu("Geometry/Generators/GeometryGeneratorNode")]
    public class GeometryGeneratorNode : TerrainNode, IReturnSectorData, IPersistentSeed
    {

        public float buffer = 10f;
        public bool CheckForMaxX;
        public bool limitPoints = true;
        
        private float _maxX = Mathf.NegativeInfinity;
        private const int MaxRetries = 5000;
        private int _retryCount = 0;
        private float _turningPoint;
        private bool _backwards;
        
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
            
            while (true)
            {
                // Debug.Log(newSectorData.Verts[^1]);
                // thisNodeSeedContainer.vectorSeed.x += 1;
                thisNodeSeedContainer.Increment();
                var result = GetConnectedAlgorithm().Result(thisNodeSeedContainer.vectorSeed, newSectorData);
                var previous = newSectorData.Verts[^1];
                Vertex2 r = result + previous;
                _maxX = Mathf.Max(_maxX, r.x);
                newSectorData.Verts.Add(r);

                if (r.x <= previous.x && !_backwards) _turningPoint = r.x;
                _backwards = r.x < previous.x;
                
                if(EndCondition(newSectorData.Verts, maxPoints, maxDistance, r.x)) break;
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
        bool EndCondition(List<Vertex2> verts, int maxPoints, float maxDistance, float newX) //todo the distance should be calculated based on an averaged version of the acvtual contours, to take into account vertical distances
        {
            // return true if end condition met
            
            
            if (!limitPoints && !LimitDistance)
                return true;

            // bool verticalIntersection = CheckUpAndDown(verts);
            
            bool end = false;
            
            if (limitPoints)
            {
                // if (verts.Count > maxPoints + 1000) return true;
                // if (ChecUpAndDown) return CheckUpAndDown(verts);
                if (verts.Count > maxPoints) end = true;                
            }

            if (LimitDistance)
            {
                // if(verts[^1].x - TerrainGraphInput.StartPos.x > maxDistance + 1000) return true;
                // if (ChecUpAndDown) return CheckUpAndDown(verts);
                if(newX - TerrainGraphInput.StartPos.x > maxDistance) end = true;
            }

            if (end)
            {
                _retryCount++;
                if(!CheckForMaxX) return true;
                if (_retryCount > MaxRetries)
                {
                    Debug.Log("max retries");
                    return true;
                }

                // if (newX < maxX) return false;
                if (newX < _maxX || newX < _turningPoint + buffer) return false;

               
                return true;

            }
            
            return false;
        }

    }
}