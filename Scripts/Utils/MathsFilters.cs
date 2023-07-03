using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace VectorTerrain.Scripts.Utils
{
    public static class MathsFilters
    {
        // public static Vector2[] ApplyGaussianFilter(Vector2[] points, float sigma, int windowSize = 5)
        // {
        //     Vector2[] result = new Vector2[points.Length];
        //     int radius = windowSize / 2;
        //     float[] weights = GaussianWeights(windowSize, sigma);
        //
        //     for (int i = 0; i < points.Length; i++)
        //     {
        //         Vector2 sum = Vector2.zero;
        //         for (int j = -radius; j <= radius; j++)
        //         {
        //             if (i + j >= 0 && i + j < points.Length)
        //             {
        //                 sum += points[i + j] * weights[j + radius];
        //             }
        //         }
        //         result[i] = sum;
        //     }
        //     return result;
        // }
        public static float[] GaussianWeights(int windowSize, float sigma)
        {
            float[] weights = new float[windowSize];
            int radius = windowSize / 2;
            float sum = 0;

            for (int i = -radius; i <= radius; i++)
            {
                weights[i + radius] = Mathf.Exp(-(i * i) / (2 * sigma * sigma));
                sum += weights[i + radius];
            }

            // Normalize weights
            for (int i = 0; i < windowSize; i++)
            {
                weights[i] /= sum;
            }
            return weights;
        }
        
        public static List<float> NormalDistribution(int length)
        {
            // Define the parameters of the Gaussian function
            float a = 1f;
            float b = length / 2f;
            float c = length / (2f * Mathf.Sqrt(2 * Mathf.Log(2)));  // FWHM

            return Enumerable.Range(0, length)
                .Select(i => a * Mathf.Exp(-Mathf.Pow(i - b, 2) / (2 * Mathf.Pow(c, 2))))
                .ToList();
        }
    }
}