using System;
using System.Collections.Generic;
using ProceduralToolkit;
using UnityEngine;

namespace VectorTerrain.Scripts.Types
{
    /// <summary>
    ///     Representation of a 2D line vertexSegment
    /// </summary>
    [Serializable]
    public struct VertexSegment : IEquatable<VertexSegment>
    {
        public Vertex2 a;
        public Vertex2 b;

        public VertexSegment(Vertex2 a, Vertex2 b)
        {
            this.a = a;
            this.b = b;
            Properies = new Dictionary<string, float>();
            // indexA = -1;
            // indexB = -1;
        }

        public VertexSegment(List<Vertex2> verts, int indexA, int indexB)
        {
            a = verts[indexA];
            b = verts[indexB];
            // this.indexA = indexA;
            // this.indexB = indexB;
            Properies = new Dictionary<string, float>();
        }

        public float Slope
        {
            get
            {
                var slope = (a.y - b.y) / (a.x - b.x);

                return float.IsNaN(slope) ? 0 : slope;
            }
        }

        public Dictionary<string, float> Properies { get; }

        /// <summary>
        ///     Returns the normalized direction of the vertexSegment
        /// </summary>
        public Vector2 direction => (b.Pos - a.Pos).normalized;

        /// <summary>
        ///     Returns the normal of the vertexSegment
        /// </summary>
        public Vector2 normal => (b.Pos - a.Pos).normalized.RotateCCW90();

        /// <summary>
        ///     Returns the length of the vertexSegment
        /// </summary>
        public float length => (b.Pos - a.Pos).magnitude;

        /// <summary>
        ///     Returns the center of the vertexSegment
        /// </summary>
        public Vector2 center => GetPoint(0.5f);

        /// <summary>
        ///     Returns the axis-aligned bounding box of the vertexSegment
        /// </summary>
        public Rect aabb
        {
            get
            {
                var min = Vector2.Min(a, b);
                var max = Vector2.Max(a, b);
                return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
            }
        }

        /// <summary>
        ///     Access the a or b component using [0] or [1] respectively
        /// </summary>
        public Vector2 this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return a;
                    case 1: return b;
                    default:
                        throw new IndexOutOfRangeException("Invalid VertexSegment index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        a = value;
                        break;
                    case 1:
                        b = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid VertexSegment index!");
                }
            }
        }

        public bool Equals(VertexSegment other)
        {
            return a.Equals(other.a) && b.Equals(other.b);
        }

        // public string ToString(string format, IFormatProvider formatProvider)
        // {
        //     return $"[{a.ToString(format, formatProvider)}, {b.ToString(format, formatProvider)}]";
        // }

        /// <summary>
        ///     Returns a point on the vertexSegment at the given normalized position
        /// </summary>
        /// <param RegionID="position">Normalized position</param>
        public Vector2 GetPoint(float position)
        {
            return Geometry.PointOnSegment2(a, b, position);
        }

        /// <summary>
        ///     Returns a list of evenly distributed points on the vertexSegment
        /// </summary>
        /// <param RegionID="count">Number of points</param>
        public List<Vector2> GetPoints(int count)
        {
            return Geometry.PointsOnSegment2(a, b, count);
        }

        /// <summary>
        ///     returns the closest distance from the segment to a point
        /// </summary>
        public float DistanceToPoint(Vector2 point)
        {
            return Hutl.DistanceToLine(a, b, point);
        }

        /// <summary>
        ///     Linearly interpolates between two segments
        /// </summary>
        public static VertexSegment Lerp(VertexSegment a, VertexSegment b, float t)
        {
            t = Mathf.Clamp01(t);
            return new VertexSegment(a.a + (b.a - a.a) * t, a.b + (b.b - a.b) * t);
        }

        /// <summary>
        ///     Linearly interpolates between two segments without clamping the interpolant
        /// </summary>
        public static VertexSegment LerpUnclamped(VertexSegment a, VertexSegment b, float t)
        {
            return new(a.a + (b.a - a.a) * t, a.b + (b.b - a.b) * t);
        }

        public static VertexSegment operator +(VertexSegment vertexSegment, Vector2 vector)
        {
            return new(vertexSegment.a + vector, vertexSegment.b + vector);
        }

        public static VertexSegment operator -(VertexSegment vertexSegment, Vector2 vector)
        {
            return new(vertexSegment.a - vector, vertexSegment.b - vector);
        }

        public static bool operator ==(VertexSegment a, VertexSegment b)
        {
            return a.a == b.a && a.b == b.b;
        }

        public static bool operator !=(VertexSegment a, VertexSegment b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() ^ (b.GetHashCode() << 2);
        }

        public override bool Equals(object other)
        {
            return other is VertexSegment && Equals((VertexSegment) other);
        }

        public override string ToString()
        {
            return string.Format("VertexSegment(a: {0}, b: {1})", a, b);
        }

        // public string ToString(string format)
        // {
        //     return string.Format("VertexSegment(a: {0}, b: {1})", a.ToString(format), b.ToString(format));
        // }


        #region Casting operators

        public static explicit operator Line2(VertexSegment vertexSegment)
        {
            return new(vertexSegment.a, (vertexSegment.b.Pos - vertexSegment.a.Pos).normalized);
        }

        public static explicit operator Ray2D(VertexSegment vertexSegment)
        {
            return new(vertexSegment.a, (vertexSegment.b.Pos - vertexSegment.a.Pos).normalized);
        }

        public static explicit operator Segment3(VertexSegment vertexSegment)
        {
            return new(vertexSegment.a, vertexSegment.b);
        }

        #endregion Casting operators
    }
}