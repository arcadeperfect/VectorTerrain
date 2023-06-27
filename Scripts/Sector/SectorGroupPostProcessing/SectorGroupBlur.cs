using System.Collections.Generic;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Utils;
using static VectorTerrain.Scripts.Sector.SectorGroupPostProcessing.SectorGroupPostProcessingUtils;

namespace VectorTerrain.Scripts.Sector.SectorGroupPostProcessing
{
    public static class SectorGroupBlur
    {
        
        
        public static Dictionary<int, TerrainGraphOutput> Blur(Dictionary<int, TerrainGraphOutput> data, float sigma, int windowsize = 40)
        {
            var v = CompileVerts(data);
            var w = VertexProcessing.Gaussian(v, 20, windowsize);
            var separateBySectorId = SeparateBySectorId(w);
            RepopulateDict(data, separateBySectorId);
            
            return data;
        }
    }
}