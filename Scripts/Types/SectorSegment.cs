﻿using System.Collections.Generic;
using ProceduralToolkit;
using UnityEngine;

namespace VectorTerrain.Scripts.Types
{
    public struct SectorSegment
    {
        private Vertex2 _a;
        public Vertex2 a { get => _a;}
        private Vertex2 _b;
        public Vertex2 b { get => _b;}


        public int IndexA { get; private set; }
        public int IndexB { get; private set; }


        public float? LocalDist => a.TotalDist; 
        
        /// <summary>
        /// Returns the normalized direction of the vertexSegment
        /// </summary>
        public Vector2 direction => ((Vector2)(b - a)).normalized;
        /// <summary>
        /// Returns the normal of the vertexSegment
        /// </summary>
        public Vector2 normal => ((Vector2)(b - a)).normalized.RotateCCW90();
        /// <summary>
        /// Returns the length of the vertexSegment
        /// </summary>
        public float length => ((Vector2)(b - a)).magnitude;
        /// <summary>
        /// Returns the center of the vertexSegment
        /// </summary>
        public Vector2 center => GetPoint(0.5f);
        /// <summary>
        /// Returns the axis-aligned bounding box of the vertexSegment
        /// </summary>
        public Rect aabb
        {
            get
            {
                Vector2 min = Vector2.Min(a, b);
                Vector2 max = Vector2.Max(a, b);
                return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
            }
        }
        
        public SectorSegment(List<Vertex2> verts, int indexA, int indexB)
        {
            _a = verts[indexA];
            _b = verts[indexB];
            IndexA = indexA;
            IndexB = indexB;
        }
        
        /// <summary>
        /// Returns a point on the vertexSegment at the given normalized position
        /// </summary>
        /// <param RegionID="position">Normalized position</param>
        public Vector2 GetPoint(float position)
        {
            return Geometry.PointOnSegment2(a, b, position);
        }
        
        public override string ToString()
        {
            return string.Format("VertexSegment(a: {0}, b: {1}, LocalDist {2})", a, b, a.TotalDist);
        }
        
    }
}