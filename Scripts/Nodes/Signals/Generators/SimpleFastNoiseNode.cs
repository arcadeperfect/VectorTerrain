using ProceduralToolkit.FastNoiseLib;
using UnityEngine;
using VectorTerrain.Scripts.Nodes.Weights;
using XNode;

namespace Nodez.Nodes.Generators
{
    [Node.CreateNodeMenuAttribute("Signals/Generators/Simple Fast Noise")]

    // public class FractalFastNoiseNode : TerrainNode, IReturnSignal
    public class SimpleFastNoiseNode : ReturnSignalNode
    {
        // [Output()] public SignalNoodle output;

        public SimpleNoiseType noiseType = SimpleNoiseType.Simplex;
        [Min(0.01f)]
        public float frequency = 5;
        public Vector3 Mult = new(1,1,1);
        public Vector3 Offset;

        // private Vector3 seedMemory;

        private FastNoise noise;
		
        // Use this for initialization
        protected override void Init() {
            base.Init();
            noise = new FastNoise();
            noise.SetNoiseType((FastNoise.NoiseType) noiseType);
        }
		
        // Return the correct value of an output port when requested
        // public override object GetValue(NodePort port) {
        // 	return null; // Replace this
        // }
		
        private void SetParams(int globalSeed)
        {

            noise.SetNoiseType((FastNoise.NoiseType)noiseType);
            noise.SetFrequency(frequency*0.001f);
            noise.SetSeed(globalSeed);
        }
		
        protected override float Compute(Vector3 vectorSeed)
        {
            // Debug.Log(vectorSeed);
            SetParams(GlobalSeed);

            var u = Vector3.Scale(vectorSeed, Mult);
			
            u += Offset * 10 / frequency;
            var v = noise.GetNoise(u.x,u.y, u.z);
            return v;
        }
		
        public enum  SimpleNoiseType
        {
            Simplex = FastNoise.NoiseType.Simplex,
            Cubic=FastNoise.NoiseType.Cubic,
            Perlin=FastNoise.NoiseType.Perlin,
            Value=FastNoise.NoiseType.Value,
            WhiteNoise=FastNoise.NoiseType.WhiteNoise,
            Cellular=FastNoise.NoiseType.Cellular
        }

    }
}