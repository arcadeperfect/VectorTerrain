using System.Linq;
using ProceduralToolkit.FastNoiseLib;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using VectorTerrain.Scripts.Attributes;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;

namespace VectorTerrain.Scripts.Nodes.Filters
{
    [CreateNodeMenu("Filters/Noisy Offset")]
    public class NoisyOffsetNode: GeometryModifierNode
    {
        public float frequency = 10f;

        [Min(0)]
        public float noiseAmount = 0f;


        protected override SectorData Process(SectorData input)
        {
            if(noiseAmount == 0f) return input;

            var noise = new FastNoise();
            noise.SetNoiseType(FastNoise.NoiseType.Simplex);
            noise.SetFrequency(frequency);
            noise.SetSeed(0);
            
            for (int i = 0; i < input.Verts.Count; i++)
            {
                var v = input.Verts[i];
                var noiseVal1 = noise.GetNoise((float)i, 0f);
                var noiseVal2 = noise.GetNoise((float)i, 110f);
                v.Pos += new Vector2(noiseVal1, noiseVal2) * noiseAmount;
                input.Verts[i] = v;
            }

            return input;
        }
    }
}