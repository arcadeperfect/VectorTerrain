using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts.Nodes.Geometry.Blenders
{
    [CreateNodeMenu("Blenders/Geometry/Lerp")]
    public class GeometryLerpNode: GeometryBlenderNode
    {
        public float lerp;
        protected override SectorData Process(SectorData input1, SectorData input2)
        {
            return SectorData.Lerp(input1, input2, lerp);
        }
    }
}