// using System;
// using System.Collections.Generic;
// using System.Runtime.InteropServices;
// using UnityEngine;
// using VectorTerrain;
//
// [StructLayout(LayoutKind.Sequential)]
// public struct CavcVertex
// {
//     public double X;
//     public double Y;
//     public double Bulge;
// }
//
// [StructLayout(LayoutKind.Sequential)]
// public struct CavcPolyline {
//     
//     public IntPtr vertex_data;
//     public int vertex_count;
//     public bool is_closed;
//     
//     public CavcPolyline(IntPtr vertex_data, int vertex_count, bool is_closed) {
//         
//         this.vertex_data = vertex_data;
//         this.vertex_count = vertex_count;
//         this.is_closed = is_closed;
//     }
//     
//     public CavcVertex[] GetVertexes()     // get an array of CavcVertex structs from the vertex_data pointer 
//     {
//         CavcVertex[] vertexes = new CavcVertex[vertex_count];
//         for(int i = 0; i < vertex_count; i++)
//         {
//             CC.cavc_pline_get_vertex(vertex_data, (uint) i, ref vertexes[i]);
//         }
//         return vertexes;
//     }
//
//     public Vector2[] GetVectors()
//     {
//         List<Vector2> vectors = new();
//         foreach (var vertex in GetVertexes())
//         {
//          vectors.Add(new Vector2((float)vertex.X, (float)vertex.Y));   
//         }
//
//         return vectors.ToArray();
//     }
// }
//
// [StructLayout(LayoutKind.Sequential)]
// public struct CavcPlineParallelOffsetO
// {
//     public IntPtr AabbIndex;
//     public double PosEqualEps;
//     public double SliceJoinEps;
//     public double OffsetDistEps;
//     public byte HandleSelfIntersects;
// }
