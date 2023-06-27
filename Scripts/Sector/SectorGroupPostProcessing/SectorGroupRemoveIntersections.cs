using System;
using System.Collections.Generic;
using System.Linq;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts.Sector.SectorGroupPostProcessing
{
    public static class SectorGroupCleanup
    {
        public static Dictionary<int, TerrainGraphOutput> Clean(Dictionary<int, TerrainGraphOutput> data)
        {
            var v = CompileVerts(data);
            var w = VertexRemoveIntersections.Process(v, VertexRemoveIntersections.Mode.remove);
            var separateBySectorId = SeparateBySectorId(w);

            int counter = 0;
            foreach (var key in separateBySectorId.Keys)
            {
                if(key == null) throw new System.Exception("key is null");
                data[(int)key].sectorData.Verts = separateBySectorId[key];

                if (counter > 0)
                {
                    var current = separateBySectorId[key];
                    var previous = separateBySectorId[counter - 1];
                    var c = current[0];
                    var p = previous[^1];

                    if (CompareVertPositions(c, p)) continue;
                    
                    var newVert = p;
                    newVert.Pos = c.Pos;
                    previous.Add(newVert);
                }
                counter++;
            }
            return data;
        }
        
        private static Dictionary<int?, List<Vertex2>> SeparateBySectorId(List<Vertex2> vertices)
        {
            return vertices
                .GroupBy(v => v.id)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
        
        public static List<Vertex2> CompileVerts(Dictionary<int, List<Vertex2>> vertsDict)
        {
            List<Vertex2> allVerts = new List<Vertex2>();
            foreach (var pair in vertsDict)
            {
                var verts = pair.Value;
                
                for (int i = 0; i < verts.Count; i++)
                {
                    var thisVert = verts[i];
                    thisVert.id = pair.Key;
                    allVerts.Add(thisVert);
                }
            }
            return allVerts;
        }
        
        public static List<Vertex2> CompileVerts(Dictionary<int, TerrainGraphOutput> vertsDict)
        {
            List<Vertex2> allVerts = new List<Vertex2>();
            foreach (var pair in vertsDict)
            {
                var verts = pair.Value.sectorData.Verts;
                
                for (int i = 0; i < verts.Count; i++)
                {
                    var thisVert = verts[i];
                    thisVert.id = pair.Key;
                    allVerts.Add(thisVert);
                }
            }
            return allVerts;
        }
        
        private static bool CompareVertPositions(Vertex2 a, Vertex2 b)
        {
            const float tolerance = 0.01f;
            return Math.Abs(a.x - b.x) < tolerance && Math.Abs(a.y - b.y) < tolerance;
        }
        
    }
}