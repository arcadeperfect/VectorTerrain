using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts.Sector.SectorGroupPostProcessing
{
    public static class SectorGroupPostProcessingUtils
    {
        public static Dictionary<int?, List<Vertex2>> SeparateBySectorId(List<Vertex2> vertices)
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
                var verts = pair.Value.SectorData.Verts;
                
                for (int i = 0; i < verts.Count; i++)
                {
                    var thisVert = verts[i];
                    thisVert.id = pair.Key;
                    allVerts.Add(thisVert);
                }
            }
            return allVerts;
        }
        
        public static bool CompareVertPositions(Vertex2 a, Vertex2 b)
        {
            const float tolerance = 0.01f;
            return Math.Abs(a.x - b.x) < tolerance && Math.Abs(a.y - b.y) < tolerance;
        }
        
        public static Dictionary<int, TerrainGraphOutput> RepopulateDict(Dictionary<int, TerrainGraphOutput> data,
            Dictionary<int?, List<Vertex2>> processedVerts)
        {
            int counter = 0;

            var v = processedVerts.Keys;
            bool areConsecutive = (v.Last() - v.First() == v.Count - 1);
            if (!areConsecutive) Debug.LogWarning("keys are not consecutive, this can lead to unexpected behaviour");
            
            var prevKey = processedVerts.Keys.First();
            
            
            foreach (var key in processedVerts.Keys)
            {
                if(key == null) throw new System.Exception("key is null");
                data[(int)key].SectorData.Verts = processedVerts[key];

                if (counter > 0)
                {
                    var current = processedVerts[key];
                    var previous = processedVerts[prevKey];
                    var c = current[0];
                    var p = previous[^1];

                    if (CompareVertPositions(c, p)) continue;
                    
                    var newVert = p;
                    newVert.Pos = c.Pos;
                    previous.Add(newVert);
                    prevKey = key;
                }
                counter++;
            }
            return data;
        }
    }
}