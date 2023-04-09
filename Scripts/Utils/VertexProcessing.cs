using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using VectorTerrain.Scripts.Types;

namespace VectorTerrain.Scripts.Utils
{
    public static class VertexProcessing
    {
        public enum ResampleMode
        {
            fraction,
            distance,
            points
        }
        
        public static List<Vector2> Verts2Vectors(List<Vertex2> verts)
        {
            List<Vector2> vectors = new();
            foreach (var vertex2 in verts)
            {
                vectors.Add(vertex2);
            }
            return vectors;
        }

        // public static List<Vertex2> RemoveCollinearPoints(List<Vertex2> verts, float tolerance)
        // {
        //     List<Vertex2> returnMe = new() {verts[0], verts[1]};
        //     // VertexSegment seg = new(verts[0], verts[1]);
        //
        //     var prevSlope = Slope(verts[0], verts[1]);
        //     prevSlope = float.IsInfinity(prevSlope) ? float.MaxValue : prevSlope;
        //
        //     for (var i = 2; i < verts.Count; i++)
        //     {
        //         // seg = new();
        //         // seg.b = verts[i-1];
        //         // seg.a = verts[i];
        //         var slope = Slope(verts[i - 1], verts[i]);
        //         slope = float.IsInfinity(slope) ? float.MaxValue : slope;
        //
        //         if (Mathf.Abs(slope - prevSlope) >= tolerance)
        //             returnMe.Add(verts[i]);
        //         prevSlope = slope;
        //     }
        //
        //     if (!Equals(returnMe[^1], verts[^1]))
        //         returnMe.Add(verts[^1]);
        //
        //     return returnMe;
        // }

        
        // public static float Slope(Vector2 a, Vector2 b)
        // {
        //     // 
        //     var slope = (a.y - b.y) / (a.x - b.x);
        //     return float.IsNaN(slope) ? 0 : slope;
        // }
        
        // public static List<VertexSegment> SegmentsFromVerts(List<Vertex2> verts)
        // {
        //     List<VertexSegment> segs = new();
        //
        //     for (var i = 0; i < verts.Count - 1; i++) segs.Add(new VertexSegment(verts[i], verts[i + 1]));
        //
        //     return segs;
        // }

        // public static float PathDistanceBetweenVerts(Vertex2 a, Vertex2 b)
        // {
        //     if (a.TotalDist == null || b.TotalDist == null)
        //         throw new NullReferenceException("A vertex did not have its distance processed");
        //
        //     var v = (float) (b.TotalDist - a.TotalDist);
        //     return Mathf.Abs(v);
        // }

        // public static List<Vertex2> RemovePointsTooCloseToPoints(List<Vertex2> verts, float threshold, float range)
        // {
        //     List<Vertex2> returnMe = new();
        //     List<int> deleteList = new();
        //
        //     for (var i = 0; i < verts.Count; i++)
        //     {
        //         var v1 = verts[i];
        //         for (var j = 0; j < verts.Count; j++)
        //         {
        //             if (i == j)
        //                 continue;
        //
        //             var v2 = verts[j];
        //
        //             if (Vector2.Distance(v1, v2) > threshold)
        //                 continue;
        //
        //             if (PathDistanceBetweenVerts(v1, v2) < range)
        //                 continue;
        //
        //             deleteList.Add(i);
        //         }
        //     }
        //
        //     for (var i = 0; i < verts.Count; i++)
        //         if (!deleteList.Contains(i))
        //             returnMe.Add(verts[i]);
        //
        //     return returnMe;
        // }

        // public static List<Vertex2> RemovePointsTooCloseToSegments(List<Vertex2> verts, float threshold)
        // {
        //     List<Vertex2> VertsCopy = new(verts);
        //
        //     for (var i = 0; i < verts.Count; i++)
        //     {
        //         List<VertexSegment> segs = new();
        //
        //         var v = verts[i];
        //
        //         for (var j = 0; j < VertsCopy.Count - 1; j++)
        //             segs.Add(new VertexSegment(VertsCopy[j], VertsCopy[j + 1]));
        //
        //         for (var j = 0; j < segs.Count; j++)
        //         {
        //             if (v.Pos == segs[j].a || v.Pos == segs[j].b)
        //                 continue;
        //
        //             var d = segs[j].DistanceToPoint(v);
        //
        //             if (d < threshold)
        //             {
        //                 VertsCopy.Remove(v);
        //                 break;
        //             }
        //         }
        //     }
        //
        //     if (VertsCopy.Count < 3) return new List<Vertex2> {verts[0], verts[^1]};
        //
        //     return VertsCopy;
        // }

        // public static float DistanceToLine(Vertex2 point, Vertex2 lineA, Vertex2 lineB)
        // {
        //     return Hutl.DistanceToLine(lineA, lineB, point);
        // }

        // public static List<Vertex2> AverageVerts(List<Vertex2> verts, int width, float mix, bool applyX, bool applyY)
        // {
        //     return AverageVerts(verts, width, mix, 0, verts.Count, applyX, applyY);
        // }

        // public static List<Vertex2> AverageVerts(List<Vertex2> verts, int width, float mix, int beginIndex,
        //     int endIndex, bool applyX, bool applyY)
        // {
        //     if (width <= 0)
        //         return verts;
        //
        //     var newVerts = new List<Vertex2>();
        //
        //     if (width % 2 == 0)
        //         width -= 1;
        //
        //     var botty = width / 2;
        //
        //     for (var i = 0; i < verts.Count; i++)
        //     {
        //         var vert = verts[i];
        //
        //         if (i < beginIndex || i > endIndex)
        //         {
        //             newVerts.Add(vert);
        //             continue;
        //         }
        //
        //         float x = 0;
        //         float y = 0;
        //
        //         for (var j = 0; j < width; j++)
        //         {
        //             var index = i - botty + j;
        //             index = math.clamp(index, 0, verts.Count - 1);
        //             // index = math.max(index, 0);
        //             // index = math.min(index, verts.Count-1);
        //
        //             x += verts[index].x;
        //             y += verts[index].y;
        //         }
        //
        //         x /= width;
        //         y /= width;
        //
        //         mix = Mathf.Clamp(mix, 0, 1);
        //         x = Mathf.Lerp(vert.Pos.x, x, mix);
        //         y = Mathf.Lerp(vert.Pos.y, y, mix);
        //
        //         vert.Pos.x = applyX ? x : vert.Pos.x;
        //         vert.Pos.y = applyY ? y : vert.Pos.y;
        //
        //         newVerts.Add(vert);
        //     }
        //
        //     return newVerts;
        // }

        // public static List<Vertex2> GaussianFilter1D(List<Vertex2> verts, int widthX, int widthY, int iterationsX,
        //     int iterationsY, float mixX, float mixY)
        // {
        //     var blurredX = new List<float>();
        //     var blurredY = new List<float>();
        //
        //     foreach (var vert in verts)
        //     {
        //         blurredX.Add(vert.x);
        //         blurredY.Add(vert.y);
        //     }
        //
        //     blurredX = GaussianFilter1D(blurredX, widthX, iterationsX);
        //     blurredY = GaussianFilter1D(blurredY, widthY, iterationsY);
        //
        //     for (var i = 0; i < verts.Count; i++)
        //     {
        //         var _ = verts[i];
        //         _.x = Mathf.Lerp(_.x, blurredX[i], mixX);
        //         _.y = Mathf.Lerp(_.y, blurredY[i], mixY);
        //         verts[i] = _;
        //     }
        //
        //     return verts;
        // }

        // public static List<float> GaussianFilter1D(List<float> floats, int width, int iterations)
        // {
        //     for (var i = 0; i < iterations; i++) floats = GaussianFilter1D(floats, width);
        //     return floats;
        // }

        // public static List<float> GaussianFilter1D(List<float> floats, int width)
        // {
        //     if (!floats.Distinct().Skip(1).Any()) return floats;
        //
        //     var results = new List<float>();
        //
        //     var avg = floats.Average();
        //
        //
        //     var sigma = Mathf.Sqrt(floats.Average(v => Mathf.Pow(v - avg, 2)));
        //
        //
        //     var kernel = GaussianKernal(width, sigma);
        //     var weightSum = kernel.Sum();
        //
        //
        //     for (var i = 0; i < floats.Count; i++) // iterate through all floats in list
        //     {
        //         float result = 0;
        //         for (var j = -width; j <= width; j++)
        //         {
        //             var index = i + j;
        //             index = math.clamp(index, 0, floats.Count - 1);
        //             var v = floats[index];
        //             v *= kernel[j + width];
        //             result += v;
        //         }
        //
        //         result /= weightSum;
        //         results.Add(result);
        //     }
        //
        //     return results;
        // }

        // private static float[] GaussianKernal(int width, float sigma)
        // {
        //     var kernel = new float[width + 1 + width];
        //
        //     for (var i = -width; i <= width; i++)
        //     {
        //         var k = Mathf.Exp(-(i * i) / (2 * sigma * sigma)) / (Mathf.PI * 2 * sigma * sigma);
        //
        //         kernel[width + i] = k;
        //     }
        //
        //     return kernel;
        // }

        // public static List<Vertex2> CalculateVertexDistances(List<Vertex2> verts)
        // {
        //     {
        //         var temp = verts[0];
        //         temp.Dist = 0;
        //         verts[0] = temp;
        //     }
        //
        //     for (var i = 1; i < verts.Count; i++)
        //     {
        //         var temp = verts[i];
        //         temp.Dist = Vector2.Distance(temp.Pos, verts[i - 1].Pos);
        //         verts[i] = temp;
        //     }
        //
        //     return verts;
        // }

        // public static float CalculateTotalDistance(List<Vector2> verts)
        // {
        //     float totalDist = 0;
        //     for (var i = 1; i < verts.Count; i++)
        //     {
        //         var temp = verts[i];
        //         totalDist += Vector2.Distance(verts[i - 1], verts[i]);
        //     }
        //
        //     return totalDist;
        // }
        //
        // public static float CalculateTotalDistance(List<Vertex2> verts)
        // {
        //     float totalDist = 0;
        //     for (var i = 1; i < verts.Count; i++)
        //     {
        //         var temp = verts[i];
        //         totalDist += Vector2.Distance(verts[i - 1], verts[i]);
        //     }
        //
        //     return totalDist;
        // }
        
        // public static SectorData Resample(SectorData sectorData, float newAmount, ResampleMode mode)
        // {
        //     sectorData.ProcessDistances();
        //
        //     switch (mode)
        //     {
        //         case ResampleMode.fraction:
        //         {
        //             var v = newAmount * sectorData.Verts.Count;
        //             return Resample(sectorData, (int) v);
        //         }
        //         case ResampleMode.distance:
        //         {
        //             var totatlDistance = sectorData.TotalLengh;
        //             var v = totatlDistance / newAmount;
        //             return Resample(sectorData, (int) v);
        //         }
        //         default:
        //             return null;
        //     }
        // }

        // public static SectorData Resample(SectorData sectorData, int numPoints)
        // {
        //     if (numPoints < 2)
        //         return sectorData;
        //
        //     if (sectorData.Verts.Count < 2)
        //         return sectorData;
        //
        //     numPoints = math.max(1, numPoints);
        //
        //     var t = 1f / numPoints;
        //     float u = 0;
        //
        //     List<Vertex2> newVerts = new();
        //
        //     for (var i = 0; i < numPoints; i++)
        //     {
        //         var v = SectorData.Traverse(sectorData, u, true);
        //         newVerts.Add(v);
        //         u += t;
        //     }
        //
        //     newVerts.Add(sectorData.Verts[^1]);
        //     sectorData.Verts = newVerts;
        //
        //     return sectorData;
        // }

        /// <summary>
        /// Offset a list
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
    }
}