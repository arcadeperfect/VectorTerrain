// using System;
// using UnityEngine;
// using VectorTerrain.Scripts.Types;
// using VectorTerrain.Scripts.Utils;
// using XNode;
//
// namespace VectorTerrain.Scripts.Nodes.Filters
// {
//     [Node.CreateNodeMenuAttribute("Filters/CutCorners")]
//
//     public class CutCornersNode: GeometryModifierNode
//     {
//         [Range(1,100)]
//         public int NoodleCornerAngle = 1;
//         protected override SectorData Process(SectorData input)
//         {
//
//             var v = input.Verts.ToVector2List().ToArray();
//             
//             var v2 = VertexProcessing.ChaikinsCornerCutting(v, NoodleCornerAngle);
//             input.Verts = v2.ToVertex2List();
//             return input;
//         }
//     }
// }