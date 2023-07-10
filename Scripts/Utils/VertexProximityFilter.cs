using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Types.Burst;
using Debug = UnityEngine.Debug;

namespace VectorTerrain.Scripts.Utils
{
    public static class VertexProximityFilter
    {
        private const int Iterations = 5000;
        private const int Range = 3;

        public static List<Vertex2> ProcessWithArrays(List<Vertex2> inputLine, float tolerance)
        {
            return ExecuteWithArrays(inputLine.ToBurstVertex(), tolerance, Iterations).ToVertex2();
        }

        private static  List<BurstVertex> ExecuteWithArrays(List<BurstVertex> inputLine, float tolerance, int loops)
        {
            // var segments = new List<BurstVertexSegment>();
            BurstVertexSegment[] segments = new BurstVertexSegment[inputLine.Count];
            
            // var dissalowed = new List<bool>();
            bool[] dissalowed = new bool[inputLine.Count];
            
            
            // var temp = new List<BurstVertex>();
            BurstVertex[] temp = new BurstVertex[inputLine.Count];

            
            int maxLoops = loops;
            
            int counter = 0;
            bool found = true;

            while (found)
            {
                // Stopwatch sw1 = new Stopwatch();
                // sw1.Start();
                dissalowed = Enumerable.Repeat(false,  inputLine.Count).ToArray();
                // sw1.Stop();
                // Debug.Log("sw1: " + sw1.ElapsedMilliseconds);
                
                // dissalowed = new bool[inputLine.Count];
                //
                // Stopwatch sw2 = new Stopwatch();
                // sw1.Start();
                // for(int i = 0; i < dissalowed.Length; i++)
                //     dissalowed[i] = false;
                // sw1.Stop();
                // Debug.Log("sw2: " + sw1.ElapsedMilliseconds);
                
                
                if (counter > maxLoops - 1)
                    break;

                found = false;


                // Construct Segments

                for (int i = 1; i < inputLine.Count; i++)
                {
                    var segment = new BurstVertexSegment(i, i - 1);
                    segments[i] = segment;
                }


                // Find Dissalowed

                int disallowedIndex = -1;

                int removeCounter = 0;

                for (int h = 0; h < inputLine.Count-1; h++)
                {
                    var s = segments[h];
                    
                    for (int i = 0; i < inputLine.Count; i++)
                    {
                        if (i == s.a || i == s.b)
                            continue;

                        if (i < s.a && s.a - i < Range)
                            continue;

                        if (i > s.b && i - s.b < Range)
                            continue;

                        float d = DistanceToLine(inputLine[i], inputLine[s.a], inputLine[s.b]);

                        if (d < tolerance*tolerance)
                        {
                            found = true;
                            dissalowed[i] = true;
                            removeCounter ++;
                            break;
                        }
                    }
                }

                int c1 = 0;
                for (int i = 0; i < inputLine.Count; i++)
                {
                    if (dissalowed[i] == false)
                    {
                        temp[c1] = inputLine[i];
                        c1++;
                    }
                }
                
                inputLine = new List<BurstVertex>(inputLine.Count - removeCounter);
                
                for (int i = 0; i < inputLine.Capacity; i++)
                {
                    inputLine[i] = temp[i];
                }

                counter++;
            }

            return inputLine;
        }
        
        public static List<Vertex2> ProcessWithLists(List<Vertex2> inputLine, float tolerance)
        {
            return ExecuteWithLists(inputLine.ToBurstVertex(), tolerance, Iterations).ToVertex2();
        }
        
        public static  List<BurstVertex> ExecuteWithLists(List<BurstVertex> inputLine, float tolerance, int loops)
         {
             var segments = new List<BurstVertexSegment>();
             List<bool> dissalowed;
             var temp = new List<BurstVertex>();

             // int range = 7;
             int maxLoops = loops;
             
             int counter = 0;
             bool found = true;

             while (found)
             {
                 dissalowed = Enumerable.Repeat(false,  inputLine.Count).ToList();
                 
                 if (counter > maxLoops - 1)
                     break;

                 found = false;


                 // Construct Segments

                 for (int i = 1; i < inputLine.Count; i++)
                 {
                     var segment = new BurstVertexSegment(i, i - 1);
                     segments.Add(segment);
                 }


                 // Find Dissalowed

                 int disallowedIndex = -1;


                 foreach (var s in segments)
                 {
                     for (int i = 0; i < inputLine.Count; i++)
                     {
                         if (i == s.a || i == s.b)
                             continue;

                         if (i < s.a && s.a - i < Range)
                             continue;

                         if (i > s.b && i - s.b < Range)
                             continue;

                         float d = DistanceToLine(inputLine[i], inputLine[s.a], inputLine[s.b]);

                         if (d < tolerance*tolerance)
                         {
                             found = true;
                             dissalowed[i] = true;
                             break;
                         }
                     }
                 }


                 for (int i = 0; i < inputLine.Count; i++)
                 {
                     if (dissalowed[i] == false)
                         temp.Add(inputLine[i]);
                 }

                 inputLine = new List<BurstVertex>(temp);

                 temp.Clear();
                 segments.Clear();

                 counter++;
             }

             // Debug.Log("counter: " + counter);

             return inputLine;
         }

        public static float DistanceToLine(BurstVertex point, BurstVertex a, BurstVertex b)
        {
            float __l2 = (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
            ;
            
            // var __dot = (point.X - a.X) * (b.X - a.X) + (point.Y - a.Y) * (b.Y - a.Y);

            var __dot = a.X * b.X + a.Y * b.Y;
            
            float __t = math.max(0, math.min(1, __dot / __l2));
            BurstVertex __projection = a + __t * (b - a);
            
            return math.distancesq(point.Pos, __projection.Pos);

            
            // return math.distance(point.Pos, __projection.Pos);
        }
    }
}

//
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using Unity.Collections;
// using Unity.Mathematics;
// using UnityEngine;
// using VectorTerrain.Scripts.Types;
// using VectorTerrain.Scripts.Types.Burst;
//
// namespace VectorTerrain.Scripts.Utils
// {
//     public static class VertexProximityFilter
//     {
//          public static List<Vertex2> Process(List<Vertex2> InputLine, float Tolerance)
//          {
//              return Execute(InputLine.ToBurstVertex(), Tolerance).ToVertex2();
//          }
//
//          public static  List<BurstVertex> Execute(List<BurstVertex> InputLine, float Tolerance)
//          {
//              var segments = new List<BurstVertexSegment>();
//              var dissalowed = new List<bool>();
//              var temp = new List<BurstVertex>();
//
//              int range = 7;
//              int maxLoops = 2500;
//              
//
//
//              int counter = 0;
//              bool found = true;
//
//              while (found)
//              {
//                  dissalowed = Enumerable.Repeat(false,  InputLine.Count).ToList();
//                  
//                  if (counter > maxLoops - 1)
//                      break;
//
//                  found = false;
//
//
//                  // Construct Segments
//
//                  for (int i = 1; i < InputLine.Count; i++)
//                  {
//                      var segment = new BurstVertexSegment(i, i - 1);
//                      segments.Add(segment);
//                  }
//
//
//                  // Find Dissalowed
//
//                  int disallowedIndex = -1;
//
//
//                  foreach (var s in segments)
//                  {
//                      for (int i = 0; i < InputLine.Count; i++)
//                      {
//                          if (i == s.a || i == s.b)
//                              continue;
//
//                          if (i < s.a && s.a - i < range)
//                              continue;
//
//                          if (i > s.b && i - s.b < range)
//                              continue;
//
//                          float d = DistanceToLine(InputLine[i], InputLine[s.a], InputLine[s.b]);
//
//                          if (d < Tolerance*Tolerance)
//                          {
//                              found = true;
//                              dissalowed[i] = true;
//                              break;
//                          }
//                      }
//                  }
//
//
//                  for (int i = 0; i < InputLine.Count; i++)
//                  {
//                      if (dissalowed[i] == false)
//                          temp.Add(InputLine[i]);
//                  }
//
//                  InputLine = new List<BurstVertex>(temp);
//
//                  temp.Clear();
//                  segments.Clear();
//
//                  counter++;
//              }
//
//              // Debug.Log("counter: " + counter);
//
//              return InputLine;
//          }
//
//          public static float DistanceToLine(BurstVertex point, BurstVertex a, BurstVertex b)
//          {
//              float __l2 = (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
//              ;
//              
//              // var __dot = (point.X - a.X) * (b.X - a.X) + (point.Y - a.Y) * (b.Y - a.Y);
//
//              var __dot = a.X * b.X + a.Y * b.Y;
//              
//              float __t = math.max(0, math.min(1, __dot / __l2));
//              BurstVertex __projection = a + __t * (b - a);
//              
//              return math.distancesq(point.Pos, __projection.Pos);
//
//              
//              // return math.distance(point.Pos, __projection.Pos);
//          }
//      }
// // }