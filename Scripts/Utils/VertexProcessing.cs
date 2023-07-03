using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts.Utils
{
    public static class VertexProcessing
    {
        /// <summary>
        /// Offset a Vertex2 list by a Vector2
        /// </summary>
        /// <param name="verts"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static List<Vertex2> Offset(List<Vertex2> verts, Vector2 offset)
        {
            List<Vertex2> OffsetList = new();
            for (var i = 0; i < verts.Count; i++)
            {
                var v = verts[i];
                v.Pos += offset;
                OffsetList.Add(v);
            }

            return OffsetList;
        }
        
        /// <summary>
        /// Calculate the total distance of a Vertex2 list
        /// </summary>
        /// <param name="verts"></param>
        /// <returns></returns>
        public static float CalculateTotalDistance(List<Vertex2> verts)
        {
            float totalDist = 0;
            for (int i = 1; i < verts.Count; i++)
            {
                var temp = verts[i];
                totalDist +=Vector2.Distance(verts[i - 1], verts[i]);
            }
            return totalDist;
        }

        public static List<Vertex2> Gaussian(List<Vertex2> inputLine, List<float> mask, float sigma, int windowSize = 5)
        {
            List<Vertex2> original = new List<Vertex2>(inputLine);
            List<Vertex2> smoothed = new List<Vertex2>(inputLine);
            List<Vertex2> lerped = new List<Vertex2>(original.Count);
            smoothed = Gaussian(smoothed, sigma, windowSize);
            
            for (int i = 0; i < original.Count; i++)
            {
                var v = original[i];
                var v2 = smoothed[i];
                var lerp = Vector2.Lerp(v, v2, mask[i]);
                lerped.Add(lerp);
            }
            return lerped;
        }
        
        
        /// <summary>
        /// Apply Gaussian filter to a list of points
        /// </summary>
        /// <param name="inputLine"></param>
        /// <param name="sigma"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<Vertex2> Gaussian(List<Vertex2> inputLine, float sigma, int windowSize = 5)
        {
            
            if (windowSize % 2 == 0) windowSize++;
            
            if (inputLine == null || inputLine.Count < 2)
                throw new ArgumentException("Input line must contain at least two points.");

            if (windowSize < 1) throw new ArgumentException("Window size must be at least 1.");

            var n = inputLine.Count;
            List<Vertex2> smoothedLine = new List<Vertex2>(n);
            
            var gaussianKernel = MathsFilters.GaussianWeights(windowSize, sigma);

            // Apply the 1D Gaussian filter separately for x and y
            var xInput = new float[n];
            var yInput = new float[n];
            for (var i = 0; i < n; i++)
            {
                xInput[i] = inputLine[i].x;
                yInput[i] = inputLine[i].y;
            }

            var xSmoothed = ApplyGaussianFilter(xInput, gaussianKernel);
            var ySmoothed = ApplyGaussianFilter(yInput, gaussianKernel);

            for (var i = 0; i < n; i++)
            {
                var thisVert = inputLine[i];
                thisVert.x = xSmoothed[i];
                thisVert.y = ySmoothed[i];
                smoothedLine.Add(thisVert);
            }
            
            
            // for (var i = 0; i < n; i++) smoothedLine.Add( new Vertex2(xSmoothed[i], ySmoothed[i]));

            // Maintain the beginning and end points
            smoothedLine[0] = inputLine[0];
            smoothedLine[n - 1] = inputLine[n - 1];

            return smoothedLine;
        }
        
        // <summary>
        /// Apply Gaussian filter to a list of floats
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="gaussianKernel"></param>
        /// <returns></returns>
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
                    var index = Mathf.Clamp(i + j, 0, n - 1);
                    sum += inputData[index] * gaussianKernel[j + halfWindowSize];
                }

                outputData[i] = sum;
            }

            return outputData;
        }
        
        
        /// <summary>
        /// Calculate the total distance of a Vertex2 list
        /// </summary>
        /// <param name="verts"></param>
        /// <returns></returns>
        public static float VertexPathLength(List<Vertex2> verts)
        {
            float totalDist = 0;
            for (int i = 1; i < verts.Count; i++)
            {
                var temp = verts[i];
                totalDist +=Vector2.Distance(verts[i - 1], verts[i]);
            }
            return totalDist;
        }
    }
}


