using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;
using XNode;

namespace VectorTerrain.Scripts.Nodes.Filters
{
    [CreateNodeMenu("Filters/RemoveIntersections")]
    public class RemoveIntersectionsNode : GeometryModifierNode
    {
        protected override SectorData Process(SectorData input)
        {
            var removedIntersections =
                VectorPathCleanup.RemoveSelfIntersections(VertexProcessing.Verts2Vectors(input.Verts).ToArray());
            input.Verts = VertexProcessing.Vectors2Verts(removedIntersections);

            return input;
        }
    }
}