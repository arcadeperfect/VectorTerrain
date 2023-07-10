using System;
using Shapes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using VectorTerrain.Scripts.Types.Burst;
using VectorTerrain.Scripts.Utils;


namespace VectorTerrain.Scripts.Types
{
    [Serializable]
    public struct Vertex2
    {
        private const float THICKNESS_DEFAULT = 1f;
        private static readonly Vector2 NormalDefault = Vector2.zero;
        private static readonly Color ColorDefault = Color.white;

        public Vector2 Pos;

        public Vector2 normal;

        public float thickness;

        private Color colorGenerator;



        // public int? id; //todo replace with proper attributes from previous version
        // public VertexID Id;
        
        /// <summary>
        ///     Return distance from this Vertex to the previous one
        /// </summary>
        public float? Dist;

        /// <summary>
        ///     Return distance from this Vertex to the first one
        /// </summary>
        public float? TotalDist;

        public Vertex2(Vector2[] vectorNormalArray)
        {
            Pos = vectorNormalArray[0];
            colorGenerator = ColorDefault;
            normal = vectorNormalArray[1];
            thickness = THICKNESS_DEFAULT;
            Dist = null;
            TotalDist = null;
            // id = null;
            // Id = new VertexID();
        }

        public Vertex2(Vector2 initVec, Vector2 Normal)
        {
            Pos = initVec;
            colorGenerator = ColorDefault;
            normal = Normal;
            thickness = THICKNESS_DEFAULT;
            Dist = null;
            TotalDist = null;
            // Id = null;
            // Id = new();
        }

        public Vertex2(Vector2 initVec)
        {
            Pos = initVec;
            colorGenerator = ColorDefault;
            normal = NormalDefault;
            thickness = THICKNESS_DEFAULT;
            Dist = null;
            TotalDist = null;
            // Id = null;
            // Id = new();

        }

        public Vertex2(Vector2 initVec, Color color)
        {
            Pos = initVec;
            colorGenerator = color;
            normal = NormalDefault;
            thickness = THICKNESS_DEFAULT;
            Dist = null;
            TotalDist = null;
            // Id = null;
            // Id = new();

        }

        public Vertex2(Vector2 initVec, Color color, float thickness)
        {
            Pos = initVec;
            colorGenerator = color;
            this.thickness = thickness;
            normal = NormalDefault;
            Dist = null;
            TotalDist = null;
            // Id = null;
            // Id = new();

        }

        public Vertex2(float x, float y)
        {
            Pos = new Vector2(x, y);
            colorGenerator = ColorDefault;
            thickness = THICKNESS_DEFAULT;
            normal = NormalDefault;
            Dist = null;
            TotalDist = null;
            // Id = null;
            // Id = new();

        }

        public Color Color
        {
            get => colorGenerator;
            set => colorGenerator = value;
        }

        public Vector2 Normal
        {
            get => normal;
            set => normal = value;
        }

        public float x
        {
            get => Pos.x;
            set => Pos.x = value;
        }

        public float y
        {
            get => Pos.y;
            set => Pos.y = value;
        }

        public static Vertex2 Lerp(Vertex2 a, Vertex2 b, float t)
        {
            var n = new Vertex2();

            n.Pos = Vector2.Lerp(a, b, t);
            n.normal = Vector2.Lerp(a.normal, b.normal, t);
            n.thickness = Mathf.Lerp(a.thickness, b.thickness, t);
            // n.Color = Color.Lerp(a.Color, b.Color, t); 
            n.Color = ColorPlus.LerpInLch(a.Color, b.Color, t);

            //todo also lerp other properties, instead of taking all other properties from a

            return n;
        }

        public static Vertex2 operator -(Vertex2 a, Vector2 b)
        {
            return new(a.Pos - b);
        }

        public static Vertex2 operator -(Vertex2 a, Vertex2 b)
        {
            return new(a.Pos - b.Pos);
        }

        public static Vertex2 operator +(Vertex2 a, Vector2 b)
        {
            return new(a.Pos + b);
        }

        // public static Vertex2Node operator+(Vertex2Node a, Vertex2Node b) { return new Vertex2Node(a.Pos+b.Pos); }
        public static Vertex2 operator *(Vertex2 a, Vertex2 b)
        {
            return new(a.x * b.x, a.y * b.y);
        }

        public static Vertex2 operator *(Vertex2 a, float d)
        {
            return new(a.x * d, a.y * d);
        }

        public static Vertex2 operator /(Vertex2 a, Vertex2 b)
        {
            return new(a.x / b.x, a.y / b.y);
        }

        // allows auto casting to vector2
        public static implicit operator Vector2(Vertex2 self)
        {
            return self.Pos;
        }

        // allows auto casting to vector3
        public static implicit operator Vector3(Vertex2 self)
        {
            return self.Pos;
        }
        
        public static implicit operator float2(Vertex2 self)
        {
            return self.Pos;
        }

        // allows auto casting from vector2
        public static implicit operator Vertex2(Vector2 v)
        {
            return new(v);
        }

        // allows auto casting from vector3
        public static implicit operator Vertex2(Vector3 v)
        {
            return new(v);
        }
        
        public static implicit operator BurstVertex(Vertex2 self)
        {
            var v = new BurstVertex();
            v.Pos = self.Pos;
            v.Color = self.Color.ToFloat4();
            v.Thickness = self.thickness;
            v.Normal = self.Normal;
            return v;   
        }
        
        public static implicit operator Vertex2(BurstVertex self)
        {
            var v = new Vertex2();
            v.Pos = self.Pos;
            v.Color = new Color(self.Color.x, self.Color.y, self.Color.z, self.Color.w);
            v.thickness = self.Thickness;
            v.Normal = self.Normal;
            return v;   
        }

        public static implicit operator PolylinePoint(Vertex2 self)
        {
            return new(self.Pos, self.Color, self.thickness);
        }

        public override string ToString()
        {
            return "" + x + " " + y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vertex2))
                return false;

            var other = (Vertex2) obj;
            return Pos == other.Pos &&
                   Dist == other.Dist &&
                   TotalDist == other.TotalDist &&
                   Color == other.Color &&
                   Normal == other.Normal &&
                   thickness == other.thickness;
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 31 + Pos.GetHashCode();
            hash = hash * 31 + Dist.GetHashCode();
            hash = hash * 31 + TotalDist.GetHashCode();
            hash = hash * 31 + Color.GetHashCode();
            hash = hash * 31 + Normal.GetHashCode();
            hash = hash * 31 + thickness.GetHashCode();
            return hash;
        }

        public static bool operator ==(Vertex2 a, Vertex2 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vertex2 a, Vertex2 b)
        {
            return !a.Equals(b);
        }
    }
}

// public struct VertexID
// {
//     private int valoo;
//     private bool isSet;
//
//     public bool IsSet
//     {
//         get => isSet;
//     }
//
//     public int Value
//     {
//         get => valoo;
//         set
//         {
//             valoo = value;
//             isSet = true;
//         }
//     }
//
//     public VertexID(int id)
//     {
//         this.valoo = id;
//         isSet = true;
//     }
// }