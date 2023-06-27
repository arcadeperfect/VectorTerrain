using UnityEngine;
using VectorTerrain.Scripts.Nodes.Floats;
using XNode;

namespace Nodez.Nodes.Floats
{
    [Node.CreateNodeMenu("Floats/Constant")]
    public class ConstantFloatNode : ReturnFloatNode
    {
        public float value;
        protected override float Compute(Vector3 vectorSeed)
        {
            return value;
        }
    }
}