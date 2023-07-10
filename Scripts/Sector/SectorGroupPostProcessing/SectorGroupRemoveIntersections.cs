// using System.Collections.Generic;
// using System.Threading.Tasks;
// using VectorTerrain.Scripts.Graph;
// using VectorTerrain.Scripts.Utils;
// using VectorTerrain.Scripts.Utils.Burst;
// using static VectorTerrain.Scripts.Sector.SectorGroupPostProcessing.SectorGroupPostProcessingUtils;
//
// namespace VectorTerrain.Scripts.Sector.SectorGroupPostProcessing
// {
//     public static class SectorGroupRemoveIntersections
//     {
//         public static Dictionary<int, TerrainGraphOutput> Clean(Dictionary<int, TerrainGraphOutput> data)
//         {
//             var v = CompileVerts(data);
//             var w = VertexRemoveIntersectionsBurst.Process(v);
//             var separateBySectorId = SeparateBySectorId(w);
//             data = RepopulateDict(data, separateBySectorId);
//             
//             return data;
//         }
//         
//         public static async Task<Dictionary<int, TerrainGraphOutput>> CleanAsync(Dictionary<int, TerrainGraphOutput> data)
//         {
//             var v = CompileVerts(data);
//             
//             var w = await Task.Run(() => VertexRemoveIntersectionsBurst.Process(v));
//             // var w = await Task.Run(() => VertexRemoveIntersections.Process(v));
//
//             
//             
//             // var w = VertexRemoveIntersections.Process(v, VertexRemoveIntersections.Mode.remove);
//             var separateBySectorId = SeparateBySectorId(w);
//             data = RepopulateDict(data, separateBySectorId);
//             
//             return data;
//         }
//     }
// }