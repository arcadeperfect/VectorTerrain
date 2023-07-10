using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using VectorTerrain.Scripts.Utils;

namespace VectorTerrain.Scripts.Types.Burst
{
    public struct BurstVertex
    {
        public float2 Pos;
        public float2 Normal;
        public float Thickness;
        public float4 Color;
        
        public float X
        {
            get => Pos.x;
            set => Pos.x = value;
        }
        
        public float Y
        {
            get => Pos.y;
            set => Pos.y = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BurstVertex operator -(BurstVertex a, BurstVertex b)
        {
            var v = new BurstVertex();
            v.X = a.X - b.X;
            v.Y = a.Y - b.Y;
            v.Thickness = a.Thickness - b.Thickness;
            v.Color = a.Color - b.Color;
            return v;
        }
        
        public static BurstVertex operator +(BurstVertex a, BurstVertex b)
        {
            var v = new BurstVertex();
            v.X = a.X + b.X;
            v.Y = a.Y + b.Y;
            v.Thickness = a.Thickness - b.Thickness;
            v.Color = a.Color - b.Color;
            return v;
        }
        
        public static BurstVertex operator *(BurstVertex a, BurstVertex b)
        {
            var v = new BurstVertex();
            v.X = a.X * b.X;
            v.Y = a.Y * b.Y;
            v.Thickness = a.Thickness;
            v.Color = a.Color;
            return v;
        }
        
        public static BurstVertex operator *(float a, BurstVertex b)
        {
            var v = new BurstVertex();
            v.X = a * b.X;
            v.Y = a * b.Y;
            v.Thickness = b.Thickness;
            v.Color = b.Color;
            return v;
        }
    }
}