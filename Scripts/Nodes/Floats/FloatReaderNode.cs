using Sirenix.OdinInspector;
using UnityEngine;
using VectorTerrain.Scripts.Types;
using XNode;

namespace VectorTerrain.Scripts.Nodes.Floats
{
    

    
    [Node.CreateNodeMenu("Floats/Reader")]
    public class FloatReaderNode: TerrainNode
    {
        [PropertyRange(0,5)]
        public float test;
        
        [Input(ShowBackingValue.Always)] public FloatNoodle floatIn;
    }
}