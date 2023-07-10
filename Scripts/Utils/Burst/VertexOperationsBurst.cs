using Unity.Mathematics;
using UnityEngine;
using VectorTerrain.Scripts.Types.Burst;

namespace VectorTerrain.Scripts.Utils.Burst
{
    public static class VertexOperationsBurst
    {

        
        public static float DistanceToLine(BurstVertex point, BurstVertex segmentA, BurstVertex segmentB) {
            
            float l2 = length_squared(segmentA, segmentB);
            float t = math.max(0, math.min(1, Dot(point - segmentA, segmentB - segmentA) / l2));
            BurstVertex projection = segmentA + t * (segmentB - segmentA); 
            return math.distance(point.Pos, projection.Pos);
        }
        
        public static float length_squared(BurstVertex v, BurstVertex w)
        {
            return (w.X - v.X) * (w.X - v.X) + (w.Y - v.Y) * (w.Y - v.Y);
        }
        
        public static float Dot(BurstVertex lhs, BurstVertex rhs) => (float) ((double) lhs.X * (double) rhs.X + (double) lhs.Y * (double) rhs.Y);

    }
    
}