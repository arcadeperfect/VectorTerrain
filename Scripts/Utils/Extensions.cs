using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Types.Burst;

namespace VectorTerrain.Scripts.Utils
{
    public static class Extensions
    {
        
        public static List<Vector2> ToVector2List(this List<Vertex2> list)
        {
            var returnMe = new List<Vector2>();
            foreach (var Vertex in list)      
            {
                returnMe.Add((Vector2) Vertex);
            }
            return returnMe;
        }
        
        public static List<Vertex2> ToVertex2List(this List<Vector2> list)
        {
            var returnMe = new List<Vertex2>();
            foreach (var vector in list)      
            {
                returnMe.Add((Vertex2) vector);
            }
            return returnMe;
        }
        
        public static Vector2[] ToVector2Array(this List<Vertex2> list)
        {
            var returnMe = new Vector2[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                returnMe[i] = (Vector2) list[i];
            }
            return returnMe;
        }
        
        public static float4 ToFloat4(this Color color)
        {
            return new float4(color.r, color.g, color.b, color.a);
        }
        
        
        
        public static List<BurstVertex> ToBurstVertex(this List<Vertex2> sourceList)
        {
            return sourceList.Select(vertex => (BurstVertex)vertex).ToList();
        }
        
        public static List<Vertex2> ToVertex2(this List<BurstVertex> sourceList)
        {
            return sourceList.Select(burstVertex => (Vertex2)burstVertex).ToList();
        }
        
    }
}