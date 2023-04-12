using System;
using UnityEngine;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;
using XNode;

namespace VectorTerrain.Scripts.Nodes.Filters
{
    [Node.CreateNodeMenuAttribute("Filters/CutCorners")]

    public class CutCornersNode: GeometryModifierNode
    {
        [Range(1,20)]
        public int NoodleCornerAngle;
        protected override SectorData Process(SectorData input)
        {
            var verts = input.Verts;
            var corneredVecs = VectorTools.VectorPathFiltering.ChaikinsCornerCutting(VertexProcessing.Verts2Vectors(verts).ToArray(), NoodleCornerAngle);
            var corneredVerts = VertexProcessing.Vectors2Verts(corneredVecs);
            input.Verts = corneredVerts;
            return input;
        }
    }
}