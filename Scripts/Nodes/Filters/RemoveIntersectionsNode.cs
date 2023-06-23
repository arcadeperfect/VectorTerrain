using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;
using VectorTools;
using XNode;

namespace VectorTerrain.Scripts.Nodes.Filters
{
    [CreateNodeMenu("Filters/RemoveIntersections")]
    public class RemoveIntersectionsNode : GeometryModifierNode
    {
        protected override SectorData Process(SectorData input)
        {
          input.Verts = VertexRemoveIntersections.Process(input.Verts, VertexRemoveIntersections.Mode.remove);
          return input;
        }
    }
}