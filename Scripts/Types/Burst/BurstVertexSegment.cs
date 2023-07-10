using VectorTerrain.Scripts.Utils.Burst;

namespace VectorTerrain.Scripts.Types.Burst
{
    public struct BurstVertexSegment
    {
        // public BurstVertex2 a;
        // public BurstVertex2 b;
        public int a;
        public int b;
        
        public BurstVertexSegment(int a, int b)
        {
            this.a = a;
            this.b = b;
        }
        //
        // public float DistanceToPoint(BurstVertex2 p)
        // {
        //     return VertexOperationsBurst.DistanceToLine(p, a, b);
        // }
    }
}