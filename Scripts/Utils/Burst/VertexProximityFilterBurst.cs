using System.Collections.Generic;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Types.Burst;
using Debug = UnityEngine.Debug;

namespace VectorTerrain.Scripts.Utils.Burst
{
    public static class VertexProximityFilterBurst
    {
        // public static List<BurstVertex2> Process(List<BurstVertex2> vertexes,  float tolerance)
        // {
        //     public NativeArray<BurstVertex2> InputLine;
        //     // public NativeList<BurstVertex2> OutputLine;
        //     // public NativeArray<int> Dissalowed;
        //     //
        //     NativeArray<BurstVertex2> inputLine = new NativeArray<BurstVertex2>(vertexes.Count, Allocator.Persistent);
        //     
        //     // return new List<BurstVertex2>();
        // }

        public static List<Vertex2> Process(List<Vertex2> vertexes, float tolerance)
        {
            var burstVerts = new List<BurstVertex>();

            for (int i = 0; i < vertexes.Count; i++)
            {
                BurstVertex bv = vertexes[i];
                burstVerts.Add(bv);
            }
            

            var result = Process(burstVerts, tolerance);


            var returnMe = new List<Vertex2>();

            for (int i = 0; i < result.Count; i++)
            {
                Vertex2 v = result[i];
                returnMe.Add(result[i]);
            }

            return returnMe;
        }

        public static List<BurstVertex> Process(List<BurstVertex> vertexes, float tolerance)
        {
            var InputLine = new NativeList<BurstVertex>(vertexes.Count, Allocator.Persistent);

            for (int i = 0; i < vertexes.Count; i++)
            {
                InputLine.Add(vertexes[i]);
            }

            var job = new FilterJob()
            {
                Tolerance = tolerance,
                InputLine = InputLine,
            };

            var handle = job.Schedule();
            handle.Complete();

            var returnMe = new List<BurstVertex>();

            for (int i = 0; i < InputLine.Length; i++)
            {
                returnMe.Add(InputLine[i]);
            }

            InputLine.Dispose();

            return returnMe;
        }


        [BurstCompile]
        struct FilterJob : IJob
        {
            public float Tolerance;

            public NativeList<BurstVertex> InputLine;
            // public NativeList<BurstVertex2> OutputLine;
            // public NativeList<int> Dissalowed;
            // public NativeList<int> Indexes;
            
            public void Execute()
            {
                var segments = new NativeList<BurstVertexSegment>(Allocator.Temp);
                var dissalowed = new NativeList<bool>(Allocator.Temp);
                var temp = new NativeList<BurstVertex>(Allocator.Temp);
                
                int range = 7;
                int maxLoops = 2500;
                
                
                for (int i = 0; i < InputLine.Length; i++)
                {
                    dissalowed.Add(false);
                }

                int counter = 0;
                bool found = true;

                while (found)
                {
                    if (counter > maxLoops-1)
                        break;

                    found = false;
                    
                    
                    // Construct Segments

                    for (int i = 1; i < InputLine.Length; i++)
                    {
                        var segment = new BurstVertexSegment(i, i - 1);
                        segments.Add(segment);
                    }

                    
                    // Find Dissalowed

                    int disallowedIndex = -1;

     
                    
                    foreach (var s in segments)
                    {
                        for (int i = 0; i < InputLine.Length; i++)
                        {
                            if (i == s.a || i == s.b)
                                continue;

                            if (i < s.a && s.a - i < range)
                                continue;
                            
                            if (i > s.b && i - s.b < range)
                                continue;

                            float d = DistanceToLine(InputLine[i], InputLine[s.a], InputLine[s.b]);
                            
                            if (d < Tolerance)
                            {
                                // Debug.Log(d);
                                found = true;
                                dissalowed[i] = true;
                                break;
                            }
                        }
                    }


                    for (int i = 0; i < InputLine.Length; i++)
                    {
                        if(dissalowed[i] == false)
                            temp.Add(InputLine[i]);
                    }
                    InputLine.CopyFrom(temp);
                    temp.Clear();
                    segments.Clear();

                    counter++;
                }
                Debug.Log("counter: " + counter);
                segments.Dispose();
                dissalowed.Dispose();
                temp.Dispose();
            }
            
            public static float DistanceToLine(BurstVertex point, BurstVertex a, BurstVertex b) {
            
                float __l2 = (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);;
                var __dot = a.X * b.X +a.Y *b.Y;
                float __t = math.max(0, math.min(1, __dot / __l2));
                BurstVertex __projection = a + __t * (b - a); 
                return math.distance(point.Pos, __projection.Pos);
            }
        }
    }
}