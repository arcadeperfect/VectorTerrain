// using System;
// using System.Runtime.InteropServices;
// using UnityEngine;
//
// namespace VectorTerrain
// {
//     public class CC
//     {
//         
//         [DllImport("cavalier_contours_ffi")]
//         public static extern IntPtr test();
//         
//         [DllImport("cavalier_contours_ffi", CallingConvention = CallingConvention.Cdecl)]
//         public static extern int cavc_pline_create(
//             CavcVertex[] vertexes,
//             uint n_vertexes,
//             byte is_closed,
//             out IntPtr pline
//         );
//
//         [DllImport("cavalier_contours_ffi", CallingConvention = CallingConvention.Cdecl)]
//         public static extern int cavc_pline_get_vertex_count(IntPtr pline, ref uint count);
//         
//         [DllImport("cavalier_contours_ffi", CallingConvention = CallingConvention.Cdecl)]
//         public static extern int cavc_pline_parallel_offset(IntPtr pline, double offset, IntPtr options, out IntPtr result);
//                 
//         [DllImport("cavalier_contours_ffi.dll")]
//         public static extern int cavc_pline_get_vertex(IntPtr pline, uint position, ref CavcVertex vertex);
//         
//         [DllImport("cavalier_contours_ffi", CallingConvention = CallingConvention.Cdecl)]
//         public static extern int cavc_pline_create_approx_aabbindex(IntPtr pline, out IntPtr aabbindex);
//         
//         [DllImport("cavalier_contours_ffi", CallingConvention = CallingConvention.Cdecl)]
//         public static extern int cavc_plinelist_get_pline(
//             IntPtr plinelist,
//             uint position,
//             out IntPtr pline);
//     }
// }