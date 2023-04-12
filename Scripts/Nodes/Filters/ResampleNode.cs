using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;

namespace VectorTerrain.Scripts.Nodes.Filters
{
    [CreateNodeMenu("Filters/Resample")]
    public class ResampleNode: GeometryModifierNode
    {
        [Input(typeConstraint = TypeConstraint.Strict)]
        public FloatNoodle Resample;
        
        protected override SectorData Process(SectorData input)
        {
            var resampleFraction = GetInputValue(nameof(Resample), Resample);
            var resampledVecs = VectorTools.VectorPathFiltering.ResamplePathByFraction(VertexProcessing.Verts2Vectors(input.Verts).ToArray(), resampleFraction.value);
            var resampledVerts = VertexProcessing.Vectors2Verts(resampledVecs);
            input.Verts = resampledVerts;
            return input;
        }
    }
}