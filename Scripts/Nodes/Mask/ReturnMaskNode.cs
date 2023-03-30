using UnityEngine;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts.Nodes.Mask
{
    public abstract class ReturnMaskNode : TerrainNode
    {
        [Output] public MaskNoodle Output;

        public float[] GetMask(Vector3 vectorSeed, int totalIterationCount)
        {
            var returnMe = new float[totalIterationCount];
            for (var i = 0; i < totalIterationCount; i++)
            {
                thisNodeSeedContainer.Increment();
                vectorSeed.z += TerrainGraphInput.zOffset;
                // returnMe[i] = input.GetSignal(new Vector3(vectorSeed.x + i, vectorSeed.y, vectorSeed.z)) / 2f + 0.5f;
                returnMe[i] = Compute(thisNodeSeedContainer.vectorSeed);
            }

            return returnMe;
        }

        protected abstract float Compute(Vector3 vectorSeed);
    }
}