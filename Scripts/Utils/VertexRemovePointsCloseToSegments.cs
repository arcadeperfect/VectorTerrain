using System.Collections.Generic;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts.Utils
{
    public class VertexRemovePointsCloseToSegments
    {
        public static List<Vertex2> Process(List<Vertex2> verts, float threshold)
        {
        
            List<Vertex2> VertsCopy = new(verts);

            for (int i = 0; i < verts.Count; i++)
            {
                List <VertexSegment> segs = new();

                var v = verts[i];
            
                for (int j = 0; j < VertsCopy.Count - 1; j++)
                {
                    segs.Add(new VertexSegment(VertsCopy[j], VertsCopy[j + 1]));
                }

                for (int j = 0; j < segs.Count; j++)
                {
                    if (v.Pos == segs[j].a || v.Pos == segs[j].b)
                        continue;

                    var d = segs[j].DistanceToPoint(v);
                
                    if (d < threshold)
                    {
                        VertsCopy.Remove(v);
                        break;
                    }
                }
            }

            if (VertsCopy.Count < 3)
            {
                return new List<Vertex2>() {verts[0], verts[^1]};
            }

            return VertsCopy;

        }
    }
}