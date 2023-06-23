using UnityEngine;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;
using VectorTools;
using XNode;

namespace VectorTerrain.Scripts.Nodes.Filters
{
    [CreateNodeMenu("Filters/RemoveClosePoints")]
    public class RemoveClosePoints : GeometryModifierNode
    {
        [Range(0.01f, 10f)]
        public float MinDistance = 0.1f;
        protected override SectorData Process(SectorData input)
        {
            input.Verts = VertexRemovePointsCloseToSegments.Process(input.Verts, MinDistance);

            return input;
        }
    }
}