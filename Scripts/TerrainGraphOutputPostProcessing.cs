using System;
using System.Collections.Generic;
using UnityEngine;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts
{
    public static class TerrainGraphOutputPostProcessing
    {

        public static void Clean(SectorData previous, SectorData current)
        {
            int index;
            if (FindLastIntersection(previous.Verts, current.Verts, out index))
            {
                current.Verts.RemoveRange(0, index);

                int blurRange = 50;
                
                // // force blurRange to be odd
                // if (blurRange % 2 == 0)
                // {
                //     blurRange++;
                // }
                
                var range1 = previous.Verts.GetRange(previous.Verts.Count - blurRange/2, blurRange/2);
                var range2 = current.Verts.GetRange(0, blurRange/2);
                
                current.SetStartPos(previous.Verts[^1]);
            }
        }
        
        
        private static bool FindLastIntersection(List<Vertex2> previousVerts, List<Vertex2>currentVerts, out int index)
        {
            for(int i = currentVerts.Count-2; i > 0; i--)
            {
                for(int j = previousVerts.Count-2; j > 0; j--)
                {

                    if (!CheckIntersection(currentVerts[i - 1], currentVerts[i], previousVerts[j - 1], previousVerts[j])) continue;
                    index = i;
                    return true;

                }
            }
            index = 0;
            return false;
        }
        private static bool CheckIntersection(Vertex2 a1, Vertex2 a2, Vertex2 b1, Vertex2 b2)
        {
            bool t = GeometeryUtils2D.FasterLineIntersection(a1, a2, b1, b2);
            return t;
        }
    }
}