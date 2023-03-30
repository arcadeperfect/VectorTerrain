using UnityEngine;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts.Nodes.Weights
{
    public abstract class ReturnWeightNode : TerrainNode
    {
        [Output(typeConstraint = TypeConstraint.Strict)]
        public WeightNoodle Output;

        public float Get(Vector3 vectorSeed)
        {
            thisNodeSeedContainer.Increment();
            vectorSeed.z += TerrainGraphInput.zOffset;
            return Compute(vectorSeed);
        }
        protected abstract float Compute(Vector3 vectorSeed);
    }
}