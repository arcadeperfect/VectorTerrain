using UnityEngine;

namespace VectorTerrain.Scripts.Types
{
    public struct SeedContainer
    {
        private Vector3 _vectorSeed;
        public Vector3 vectorSeed => _vectorSeed;

        public int Ticks { get; private set; }

        public void SetSeed(Vector3 vectorSeed)
        {
            _vectorSeed = vectorSeed;
        }

        public void Increment()
        {
            _vectorSeed.x += 1;
            Ticks++;
        }

        // public void IncrementTick()
        // {
        //     ticks++;
        // }

        public void Reset()
        {
            _vectorSeed = Vector3.zero;
            Ticks = 0;
        }

        public override string ToString()
        {
            var toPrint = "---\n";
            toPrint += $"VecorSeed : {vectorSeed}\n";
            toPrint += $"Ticks : {Ticks}\n\n";
            return toPrint;
        }
    }
}