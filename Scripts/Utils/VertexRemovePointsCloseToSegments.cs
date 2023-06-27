using System.Collections.Generic;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts.Utils
{
    public class VertexRemovePointsCloseToSegments
    {
        public static List<Vertex2> Process(List<Vertex2> verts, float threshold)
        {
            List<Vertex2> vertsCopy = new(verts);

            for (int i = 0; i < verts.Count; i++)
            {
                List <VertexSegment> segs = new();
                var v = verts[i];
            
                for (int j = 0; j < vertsCopy.Count - 1; j++) segs.Add(new VertexSegment(vertsCopy[j], vertsCopy[j + 1]));
                
                for (int j = 0; j < segs.Count; j++)
                {
                    if (v.Pos == segs[j].a.Pos || v.Pos == segs[j].b.Pos) continue;
                    
                    var d = segs[j].DistanceToPoint(v);
                
                    if (d < threshold)
                    {
                        vertsCopy.Remove(v);
                        break;
                    }
                }
            }

            if (vertsCopy.Count < 3) return new List<Vertex2>() { verts[0], verts[^1] };
            
            return vertsCopy;

        }
    }
}