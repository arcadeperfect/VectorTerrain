using Shapes;
using UnityEngine;

// public class Vertex2Node
// {
//     /*
//         
//     Vertex2Node is a datatype similar to a Vector2 but with extra features specifically tailered to the terrain system
//         
//      */
//     
//     public Vector2 Pos;
//
//     public Color Color { get; set; }
//     
//     public float Normal { get; set; }
//
//     public float thickness = 3.0f;
//     
//     public float x => Pos.x;
//
//     public float y
//     {
//         get => Pos.y;
//         set => Pos.y = value;
//     }
//     
//
//     public Vertex2Node(Vector2 initVec)
//     {
//         Pos = initVec;
//     }
//
//     public Vertex2Node(Vector2 initVec, Color color)
//     {
//         Pos = initVec;
//         Color = color;
//     }
//     
//     public Vertex2Node(Vector2 initVec, Color color, float Thickness)
//     {
//         Pos = initVec;
//         Color = color;
//         thickness = Thickness;
//     }
//     
//     public Vertex2Node(float x, float y)
//     {
//         Pos = new Vector2(x, y);
//     }
//
//     public static Vertex2Node operator-(Vertex2Node a, Vector2 b) { return new Vertex2Node(a.Pos-b); }
//     public static Vertex2Node operator-(Vertex2Node a, Vertex2Node b) { return new Vertex2Node(a.Pos-b.Pos); }
//     
//     public static Vertex2Node operator+(Vertex2Node a, Vector2 b) { return new Vertex2Node(a.Pos+b); }
//     // public static Vertex2Node operator+(Vertex2Node a, Vertex2Node b) { return new Vertex2Node(a.Pos+b.Pos); }
//     
//     public static Vertex2Node operator*(Vertex2Node a, Vertex2Node b) { return new Vertex2Node(a.x * b.x, a.y * b.y); }
//     public static Vertex2Node operator*(Vertex2Node a, float d) { return new Vertex2Node(a.x * d, a.y * d); }
//     
//     // allows auto casting to vector2
//     public static implicit operator Vector2(Vertex2Node self)
//     {
//         return self.Pos;
//     }
//     // allows auto casting to vector3
//     public static implicit operator Vector3(Vertex2Node self)
//     {
//         return self.Pos;
//     }
//     
//     // allows auto casting from vector2
//     public static implicit operator Vertex2Node(Vector2 v)
//     {
//         return new Vertex2Node(v);
//     }
//     // allows auto casting from vector3
//     public static implicit operator Vertex2Node(Vector3 v)
//     {
//         return new Vertex2Node(v);
//     }
//
//     public static implicit operator Shapes.PolylinePoint(Vertex2Node self)
//     {
//         return new PolylinePoint(self.Pos, self.Color, self.thickness);
//     }
//     
//     
//     public override string ToString()
//     {
//         return "" + x + " " + y;
//     }
//     
//     
// }

namespace VectorTerrain.Scripts.Types
{
    [System.Serializable]
    public struct Vertex2
    {
        private const float THICKNESS_DEFAULT = 1f;
        private static readonly Vector2 NormalDefault = Vector2.zero;
        private static readonly Color ColorDefault = Color.white;
    
        public Vector2 Pos;
    
        /// <summary>
        ///     Return distance from this Vertex to the previous one
        /// </summary>
        public float? Dist;

        /// <summary>
        ///     Return distance from this Vertex to the first one
        /// </summary>
        public float? TotalDist;

        private Color colorGenerator;

        public Color Color
        {
            get
            {
                return colorGenerator;
            }
            set
            {
                colorGenerator = value;
            }
        }

        public Vector2 normal;
        public Vector2 Normal
        {
            get { return normal;}
            set { normal = value; }
        }

        public float thickness;
    
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
        public Vertex2(Vector2[] vectorNormalArray)
        {
            Pos = vectorNormalArray[0];
            colorGenerator = ColorDefault;
            normal = vectorNormalArray[1];
            thickness = THICKNESS_DEFAULT;
            Dist = null;
            TotalDist = null;
        }
        public Vertex2(Vector2 initVec, Vector2 Normal)
        {
            Pos = initVec;
            colorGenerator = ColorDefault;
            normal = Normal;
            thickness = THICKNESS_DEFAULT;
            Dist = null;
            TotalDist = null;
        }
        public Vertex2(Vector2 initVec)
        {
            Pos = initVec;
            colorGenerator = ColorDefault;
            normal = NormalDefault;
            thickness = THICKNESS_DEFAULT;
            Dist = null;
            TotalDist = null;
        }
        public Vertex2(Vector2 initVec, Color Color)
        {
            Pos = initVec;
            colorGenerator = Color;
            normal = NormalDefault;
            thickness = THICKNESS_DEFAULT;
            Dist = null;
            TotalDist = null;
        }
        public Vertex2(Vector2 initVec, Color Color, float Thickness)
        {
            Pos = initVec;
            colorGenerator = Color;
            thickness = Thickness;
            normal = NormalDefault;
            Dist = null;
            TotalDist = null;
        }
        public Vertex2(float x, float y)
        {
            Pos = new Vector2(x, y);
            colorGenerator = ColorDefault;
            thickness = THICKNESS_DEFAULT;
            normal = NormalDefault;
            Dist = null;
            TotalDist = null;
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
        public static Vertex2 operator-(Vertex2 a, Vector2 b) { return new Vertex2(a.Pos-b); }
        public static Vertex2 operator-(Vertex2 a, Vertex2 b) { return new Vertex2(a.Pos-b.Pos); }
        public static Vertex2 operator+(Vertex2 a, Vector2 b) { return new Vertex2(a.Pos+b); }
        // public static Vertex2Node operator+(Vertex2Node a, Vertex2Node b) { return new Vertex2Node(a.Pos+b.Pos); }
        public static Vertex2 operator*(Vertex2 a, Vertex2 b) { return new Vertex2(a.x * b.x, a.y * b.y); }
        public static Vertex2 operator*(Vertex2 a, float d) { return new Vertex2(a.x * d, a.y * d); }
        public static Vertex2 operator/(Vertex2 a, Vertex2 b) { return new Vertex2(a.x / b.x, a.y / b.y); }

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
    
        // allows auto casting from vector2
        public static implicit operator Vertex2(Vector2 v)
        {
            return new Vertex2(v);
        }
        // allows auto casting from vector3
        public static implicit operator Vertex2(Vector3 v)
        {
            return new Vertex2(v);
        }

        public static implicit operator Shapes.PolylinePoint(Vertex2 self)
        {
            return new PolylinePoint(self.Pos, self.Color, self.thickness);
        }
    
        public override string ToString()
        {
            return "" + x + " " + y;
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is Vertex2))
                return false;
        
            Vertex2 other = (Vertex2)obj;
            return Pos == other.Pos &&
                   Dist == other.Dist &&
                   TotalDist == other.TotalDist &&
                   Color == other.Color &&
                   Normal == other.Normal &&
                   thickness == other.thickness;
        }

        public override int GetHashCode()
        {
            int hash = 17;
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
