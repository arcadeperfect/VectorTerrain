

using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Types.Burst;

namespace VectorTerrain.Scripts.Utils.Burst
{
    public class VertexGaussianBurst
    {
        [BurstCompile]
        struct GaussianJob : IJob
        {
            public float Sigma;
            public int WindowSize;
            public NativeArray<BurstVertex> InputLine;
            public NativeArray<BurstVertex> OutputLine;


            public void Execute()
            {
                // smoothedLine = new NativeArray<float2>(inputLine.Length, Allocator.Persistent);
                
                var n = InputLine.Length;
                var xInput = new float[n];
                var yInput = new float[n];
                
                for (var i = 0; i < n; i++)
                {
                    xInput[i] = InputLine[i].X;
                    yInput[i] = InputLine[i].Y;
                }
                
                var gaussianKernel = GaussianWeights(WindowSize, Sigma);
                
                var xSmoothed = ApplyGaussianFilter(xInput, gaussianKernel);
                var ySmoothed = ApplyGaussianFilter(yInput, gaussianKernel);
                
                for (var i = 0; i < n; i++)
                {
                    var thisVert = InputLine[i];
                    thisVert.X = xSmoothed[i];
                    thisVert.Y = ySmoothed[i];
                    OutputLine[i] = thisVert;
                }
                
                OutputLine[0] = InputLine[0];
                OutputLine[n - 1] = InputLine[n - 1];
                
                // inputLine = smoothedLine;
            }
        }

        public static List<Vertex2> Gaussian(List<Vertex2> input, float sigma, int windowSize = 5)
        {
            
            if (windowSize % 2 == 0) windowSize++;
            
            if (input == null || input.Count < 2)
                throw new ArgumentException("Input line must contain at least two points.");

            if (windowSize < 1) throw new ArgumentException("Window size must be at least 1.");

            var n = input.Count;
            List<Vertex2> output = new List<Vertex2>(n);

        
            var toJob = new NativeArray<BurstVertex>(n, Allocator.Persistent);
            var smoothedLine = new NativeArray<BurstVertex>(n, Allocator.Persistent);
            
            for (var i = 0; i < n; i++)
            {
                toJob[i] = input[i];
            }
            
            var job = new GaussianJob
            {
                Sigma = sigma,
                WindowSize = windowSize,
                InputLine = toJob,
                OutputLine = smoothedLine
            };
            
            var handle = job.Schedule();
            handle.Complete();

            for(int i = 0; i < input.Count; i++)
            {
                var v = input[i];
                v.Pos = smoothedLine[i].Pos;
                output.Add(v);
            }
            
            toJob.Dispose();
            smoothedLine.Dispose();

            return output;
        }
        
        
        
        private static float[] ApplyGaussianFilter(float[] inputData, float[] gaussianKernel)
        {
            var n = inputData.Length;
            var halfWindowSize = gaussianKernel.Length / 2;
            var outputData = new float[n];

            for (var i = 0; i < n; i++)
            {
                float sum = 0;

                for (var j = -halfWindowSize; j <= halfWindowSize; j++)
                {
                    var index = math.clamp(i + j, 0, n - 1);
                    sum += inputData[index] * gaussianKernel[j + halfWindowSize];
                }

                outputData[i] = sum;
            }

            return outputData;
        }
        
        private static float[] GaussianWeights(int windowSize, float sigma)
        {
            float[] weights = new float[windowSize];
            int radius = windowSize / 2;
            float sum = 0;

            for (int i = -radius; i <= radius; i++)
            {
                weights[i + radius] = math.exp(-(i * i) / (2 * sigma * sigma));
                sum += weights[i + radius];
            }

            // Normalize weights
            for (int i = 0; i < windowSize; i++)
            {
                weights[i] /= sum;
            }
            return weights;
        }
    }
}