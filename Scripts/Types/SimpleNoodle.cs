using UnityEngine;
using VectorTerrain.Scripts.Nodes.Floats;
using XNode;


namespace VectorTerrain.Scripts.Types
{
    public class SimpleNoodle
    {
        [HideInInspector] public NodePort port; // todo make private, use constructor

        public float GetValue(Vector3 vectorSeed)
        {
            var node = port.Connection.node as ReturnFloatNode;
            return node.GetFloat(vectorSeed);
        }
    }
}