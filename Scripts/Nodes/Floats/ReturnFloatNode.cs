using UnityEngine;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts.Nodes.Floats
{
    public abstract class ReturnFloatNode : TerrainNode
    {
        [Output(typeConstraint = TypeConstraint.Strict)]
        public FloatNoodle Output;

        public float GetFloat(Vector3 vectorSeed)
        {
            thisNodeSeedContainer.Increment();
            vectorSeed.z += TerrainGraphInput.zOffset;
            return Compute(vectorSeed);
        }
        protected abstract float Compute(Vector3 vectorSeed);
    }
}