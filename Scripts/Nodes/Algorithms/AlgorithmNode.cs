
using UnityEngine;
using VectorTerrain.Scripts.Nodes;
using VectorTerrain.Scripts.Types;
using XNode;

namespace Nodez.BaseNodes
{
    public abstract class AlgorithmNode : TerrainNode
    {
        [Output] public AlgorithmNoodle output;
		
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public WeightNoodle weight = new();
        public abstract Vertex2 Result(Vector3 vectorSeed, SectorData dataSoFar);
        public float GetWeight(Vector3 vectorSeed)
        {
            NodePort weightPort = GetPort(nameof(weight));
            var weightSignal = weight.GetWeight(vectorSeed);
            return Mathf.Clamp(weightSignal, 0, 1);
        }
    }
}