using System.Collections.Generic;
using Unity.Mathematics;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts.Utils
{
    public class VertexResample
    {
        public static List<Vertex2> Resample(SectorData sectorData, float newAmount, ResampleMode mode)
        {
            switch (mode)
            {
                case ResampleMode.fraction:
                {
                    var v = newAmount * sectorData.Verts.Count;
                    return Resample(sectorData, (int) v);
                }
                case ResampleMode.distance:
                {
                    var verts = new SectorData();
                    var totatlDistance = VertexProcessing.CalculateTotalDistance(sectorData.Verts);
                    var v = totatlDistance / newAmount;
                    return Resample(sectorData, (int) v);
                }
                case ResampleMode.points:
                {
                    return Resample(sectorData, (int)newAmount);
                }
            }
            throw new System.Exception("Invalid ResampleMode");
        }
        
        private static List<Vertex2> Resample(SectorData sectorData, int numPoints)
        {
            var verts = sectorData.Verts;
            if (numPoints < 2)
                return verts;
        
            if (verts.Count < 2)
                return verts;
        
        
            numPoints = math.max(1, numPoints);
            float t = 1f / numPoints;

            float u = 0;

            List<Vertex2> resampledVerts = new List<Vertex2>();
        
            float totalDist = VertexProcessing.CalculateTotalDistance(verts);
        
            for (int i = 0; i < numPoints; i++)
            {
                var v = SectorData.Traverse(sectorData, u, true);
                resampledVerts.Add(v);
                u += t;
            }
        
            resampledVerts.Add(verts[^1]);

            return resampledVerts;
        }

        public enum ResampleMode
        {
            fraction,
            distance,
            points
        }
    }
}