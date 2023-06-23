using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;

namespace VectorTerrain.Scripts.Nodes.Filters
{
    [CreateNodeMenu("Filters/Resample")]
    public class ResampleNode: GeometryModifierNode
    {
        [Input(typeConstraint = TypeConstraint.Strict)]
        public FloatNoodle Resample;

        public VertexResample.ResampleMode mode;
        protected override SectorData Process(SectorData input)
        {
            var v1 = GetInputFloatNoodles()[0].value;
            var v2 = GetInputValue(nameof(Resample), Resample).value;
            input.Resample(v1, mode);
            return input;
        }
    }
}