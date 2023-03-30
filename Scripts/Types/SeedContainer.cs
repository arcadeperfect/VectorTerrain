using UnityEngine;

namespace VectorTerrain.Scripts.Types
{
    public struct SeedContainer
    {
        private Vector3 _vectorSeed;
        public Vector3 vectorSeed => _vectorSeed;
        private int ticks;
        
        public int Ticks => ticks;
        
        public void SetSeed(Vector3 vectorSeed)
        {
            _vectorSeed = vectorSeed;
        }

        public void Increment()
        {
            _vectorSeed.x += 1;
            ticks++;
        }

        // public void IncrementTick()
        // {
        //     ticks++;
        // }

        public void Reset()
        {
            _vectorSeed = Vector3.zero;
            ticks = 0;
        }

        public override string ToString()
        {
            string toPrint = "---\n";
            toPrint += $"VecorSeed : {vectorSeed}\n";
            toPrint += $"Ticks : {ticks}\n\n";
            return toPrint;
        }
    }
}
