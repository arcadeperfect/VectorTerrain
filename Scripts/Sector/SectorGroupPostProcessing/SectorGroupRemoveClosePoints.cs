// using System.Collections.Generic;
// using VectorTerrain.Scripts.Graph;
// using VectorTerrain.Scripts.Utils;
// using static VectorTerrain.Scripts.Sector.SectorGroupPostProcessing.SectorGroupPostProcessingUtils;
//
//
// namespace VectorTerrain.Scripts.Sector.SectorGroupPostProcessing
// {
//     public static class SectorGroupRemoveClosePoints
//     {
//         public static Dictionary<int, TerrainGraphOutput> Process(Dictionary<int, TerrainGraphOutput> data, float sigma, int windowsize = 40)
//         {
//             var v = CompileVerts(data);
//             var w = VertexRemovePointsCloseToSegments.Process(v, 0.01f);
//             var separateBySectorId = SeparateBySectorId(w);
//             RepopulateDict(data, separateBySectorId);
//             
//             return data;
//         }
//     }
// }