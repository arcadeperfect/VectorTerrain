using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Types.Burst;

namespace VectorTerrain.Scripts.Utils.Burst
{
    public static class VertexRemoveIntersectionsBurst
    {
        public static  List<Vertex2> Process(List<Vertex2> vertexes)
        {
            var points = new NativeList<RemoveIntersectionsJob.point>(vertexes.Count, Allocator.Persistent);
            var indices = new NativeArray<int>(vertexes.Count, Allocator.Persistent);

            for (int i = 0; i < vertexes.Count; i++)
            {
                points.Add(new RemoveIntersectionsJob.point()
                {
                    Vertex = vertexes[i],
                    index = i
                });
            }
            
            var job = new RemoveIntersectionsJob()
            {
                Points = points,
                Indices = indices
            };
            
            var handle = job.Schedule();
            handle.Complete();
            
            var returnMe = new List<Vertex2>();
            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i] != -1)
                {
                    returnMe.Add(vertexes[indices[i]]);
                }
            }
            
            points.Dispose();
            indices.Dispose();

            return returnMe;
        }

        [BurstCompile]
        struct RemoveIntersectionsJob : IJob
        {
            public NativeArray<int> Indices;
            public NativeList<point> Points;
            public void Execute()
            {
                for(int i = 0; i < Points.Length; i++)
                {
                    Indices[i] = i;
                }
                
                bool intersectionFound = true;
                int counter = 0;

                while (intersectionFound)
                {
                    if (counter > 5000) break;
                
                    intersectionFound = false;
                    
                    for (int i = 1; i < Points.Length; i++)
                    {
                        if(Indices[i] == -1) continue;
                        for (int j = i; j < Points.Length; j++)
                        {
                            if(Indices[j] == -1) continue;
                            if (j < i + 2) continue;
                            bool intersecting = CheckIntersection(Points[i - 1].Vertex.Pos, Points[i].Vertex.Pos, Points[j].Vertex.Pos, Points[j - 1].Vertex.Pos);
                            if (intersecting)
                            {
                                intersectionFound = true;
                                for (int k = i; k < j; k++)
                                {
                                    Indices[Points[k].index] = -1;
                                }
                            }
                        }
                    }
                    counter++;
                }
            }
            private static bool CheckIntersection(float2 a1, float2 a2, float2 b1, float2 b2) => GeometeryUtils2D.FasterLineIntersection(a1, a2, b1, b2);
            public struct point
            {
                public BurstVertex Vertex;
                public int index;
            }
        }
    }
}
//
// using System.Collections.Generic;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Jobs;
// using Unity.Mathematics;
// using VectorTerrain.Scripts.Types;
//
// namespace VectorTerrain.Scripts.Utils.Burst
// {
//     public static class VertexRemoveIntersectionsBurst
//     {
//         public static  List<Vertex2> Process(List<Vertex2> vertexes)
//         {
//             var points = new NativeList<RemoveIntersectionsJob.point>(vertexes.Count, Allocator.Persistent);
//             var indices = new NativeArray<int>(vertexes.Count, Allocator.Persistent);
//
//             for (int i = 0; i < vertexes.Count; i++)
//             {
//                 points.Add(new RemoveIntersectionsJob.point()
//                 {
//                     pos = vertexes[i].Pos,
//                     index = i
//                 });
//             }
//             
//             var job = new RemoveIntersectionsJob()
//             {
//                 Points = points,
//                 Indices = indices
//             };
//             
//             var handle = job.Schedule();
//             handle.Complete();
//             
//             var returnMe = new List<Vertex2>();
//             for (int i = 0; i < indices.Length; i++)
//             {
//                 if (indices[i] != -1)
//                 {
//                     returnMe.Add(vertexes[indices[i]]);
//                 }
//             }
//             
//             points.Dispose();
//             indices.Dispose();
//
//             return returnMe;
//         }
//
//         [BurstCompile]
//         struct RemoveIntersectionsJob : IJob
//         {
//             public NativeArray<int> Indices;
//             public NativeList<point> Points;
//             public void Execute()
//             {
//                 for(int i = 0; i < Points.Length; i++)
//                 {
//                     Indices[i] = i;
//                 }
//                 
//                 bool intersectionFound = true;
//                 int counter = 0;
//
//                 while (intersectionFound)
//                 {
//                     if (counter > 5000) break;
//                 
//                     intersectionFound = false;
//                     
//                     for (int i = 1; i < Points.Length; i++)
//                     {
//                         if(Indices[i] == -1) continue;
//                         for (int j = i; j < Points.Length; j++)
//                         {
//                             if(Indices[j] == -1) continue;
//                             if (j < i + 2) continue;
//                             bool intersecting = CheckIntersection(Points[i - 1].pos, Points[i].pos, Points[j].pos, Points[j - 1].pos);
//                             if (intersecting)
//                             {
//                                 intersectionFound = true;
//                                 for (int k = i; k < j; k++)
//                                 {
//                                     Indices[Points[k].index] = -1;
//                                 }
//                             }
//                         }
//                     }
//                     counter++;
//                 }
//             }
//             private static bool CheckIntersection(float2 a1, float2 a2, float2 b1, float2 b2) => GeometeryUtils2D.FasterLineIntersection(a1, a2, b1, b2);
//             public struct point
//             {
//                 public float2 pos;
//                 public int index;
//             }
//         }
//     }
// }

