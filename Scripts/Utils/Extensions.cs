using System.Collections.Generic;
using UnityEngine;
using VectorTerrain.Scripts.Types;

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
    }
}