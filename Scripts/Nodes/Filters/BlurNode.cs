using System.Linq;
using UnityEngine;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;

namespace VectorTerrain.Scripts.Nodes.Filters
{
    [CreateNodeMenu("Filters/Blur")]
    public class BlurNode: GeometryModifierNode
    {
        [Input(typeConstraint = TypeConstraint.Strict)]
        public FloatNoodle BlurAmount;
        
        [Input(typeConstraint = TypeConstraint.Strict)]
        public int WindowSize;
        protected override SectorData Process(SectorData input)
        {
            var verts = input.Verts;
            var blurAmount = GetInputValue(nameof(BlurAmount), BlurAmount);
            var blurredVecs = VectorTools.VectorPathFiltering.SmoothLineFaster(VertexProcessing.Verts2Vectors(verts).ToArray(), WindowSize, blurAmount.value);
            var blurredVerts = VertexProcessing.Vectors2Verts(blurredVecs);
            input.Verts = blurredVerts;
            return input;
        }
    }
}