

using System.Collections.Generic;
using UnityEngine;
using VectorTerrain.Scripts.Types;

public static class VertexRemoveIntersections
{
    //todo implement remove verts option
    
    public enum RremoveIntersectionsMode
    {
        swap,
        remove
    }
    
    public static List<Vertex2> Process(List<Vertex2> Verts, Mode mode)
    {
        
        bool intersectionFound = true;

        int counter = 0;
        
        while (intersectionFound)
        {
            if (counter > 5000)
            {
                Debug.Log("remove intersections did not succeed after 5000 width");
                break;
            }
            
            intersectionFound = false;

            for (int i = 1; i < Verts.Count; i++)
            {
                Edge edgeA = new Edge(Verts[i - 1], Verts[i]);

                for (int j = i; j < Verts.Count; j++)
                {
                    if (j < i + 2) continue;
                    Edge edgeB = new Edge(Verts[j], Verts[j - 1]);
                    bool intersecting = CheckIntersection(edgeA, edgeB);

                    if (intersecting)
                    {
                        intersectionFound = true;
                        if(mode == Mode.reorder)
                            Verts = ReverseRange(Verts, i, j);
                        else if (mode == Mode.remove)
                            Verts = RemoveRange(Verts, i, j);
                    }
                }
            }

            // if (intersectionFound == false)
            //     Debug.Log("removeIntersections took " + counter);
            counter++;
        }

        return Verts;
    }

    public static List<Vertex2> RemoveRange(List<Vertex2> verts, int startIndex, int endIndex)
    {
        List<Vertex2> returnMe = new();
        for (int i = 0; i < verts.Count; i++)
        {
            if (!(i >= startIndex && i <= endIndex))
            {
                returnMe.Add(verts[i]);
            }
        }

        return returnMe;
    }
    
    private static List<Vertex2> ReverseRange(List<Vertex2> verts, int startIndex, int endIndex)
    {
        var reversedList = verts.GetRange(startIndex, endIndex-startIndex);
        reversedList.Reverse();
        
        for (int i = startIndex; i < endIndex; i++)
        {
            int reversedListIndex = i - startIndex;

            verts[i] = reversedList[reversedListIndex];
        }

        return verts;
    }
    private class Edge
    {
        public Vertex2 a;
        public Vertex2 b;
        public Edge(Vertex2 a, Vertex2 b)
        {
            this.a = a;
            this.b = b;
        }
    }

    private static bool CheckIntersection(Edge edgeA, Edge edgeB)
    {
        return GeometeryUtils2D.FasterLineIntersection(edgeA.a, edgeA.b, edgeB.a, edgeB.b);
    }

    public enum Mode
    {
        reorder,
        remove
    }
    
}


// using System.Collections.Generic;
// using UnityEngine;
// using VectorTerrain.Scripts.Types;
//
// namespace VectorTerrain.Scripts.Utils
// {
//     public static class VertexRemoveIntersections
//     {
//         //todo implement remove verts option
//
//         public enum RremoveIntersectionsMode
//         {
//             swap,
//             remove
//         }
//
//         public static List<Vertex2> Process(List<Vertex2> Verts, Mode mode = Mode.remove)
//         {
//             bool intersectionFound = true;
//
//             int counter = 0;
//
//             while (intersectionFound)
//             {
//                 Debug.Log(counter);
//                 if (counter > 5000)
//                 {
//                     Debug.Log("remove intersections did not succeed after 5000 iterations");
//                     break;
//                 }
//
//                 intersectionFound = false;
//
//                 for (int i = 1; i < Verts.Count; i++)
//                 {
//                     Edge edgeA = new Edge(Verts[i - 1], Verts[i]);
//
//                     for (int j = i; j < Verts.Count; j++)
//                     {
//                         if (j < i + 2) continue;
//                         Edge edgeB = new Edge(Verts[j], Verts[j - 1]);
//                         bool intersecting = CheckIntersection(edgeA, edgeB);
//
//                         if (intersecting)
//                         {
//                             intersectionFound = true;
//                             if (mode == Mode.reorder)
//                                 Verts = ReverseRange(Verts, i, j);
//                             else if (mode == Mode.remove)
//                                 Verts = RemoveRange(Verts, i, j);
//                         }
//                     }
//                 }
//
//                 if (intersectionFound == false)
//                 {
//                     // Debug.Log("removeIntersections took " + counter);
//                     break;
//                 }
//                 counter++;
//             }
//
//             return Verts;
//         }
//
//         public static List<Vertex2> RemoveRange(List<Vertex2> verts, int startIndex, int endIndex)
//         {
//             List<Vertex2> returnMe = new();
//             for (int i = 0; i < verts.Count; i++)
//             {
//                 if (!(i >= startIndex && i <= endIndex))
//                 {
//                     returnMe.Add(verts[i]);
//                 }
//             }
//
//             return returnMe;
//         }
//
//         private static List<Vertex2> ReverseRange(List<Vertex2> verts, int startIndex, int endIndex)
//         {
//             var reversedList = verts.GetRange(startIndex, endIndex - startIndex);
//             reversedList.Reverse();
//
//             for (int i = startIndex; i < endIndex; i++)
//             {
//                 int reversedListIndex = i - startIndex;
//
//                 verts[i] = reversedList[reversedListIndex];
//             }
//
//             return verts;
//         }
//
//         private class Edge
//         {
//             public Vertex2 a;
//             public Vertex2 b;
//
//             public Edge(Vertex2 a, Vertex2 b)
//             {
//                 this.a = a;
//                 this.b = b;
//             }
//         }
//
//         private static bool CheckIntersection(Edge edgeA, Edge edgeB)
//         {
//             return GeometeryUtils2D.FasterLineIntersection(edgeA.a, edgeA.b, edgeB.a, edgeB.b);
//         }
//
//         public enum Mode
//         {
//             reorder,
//             remove
//         }
//     }
// }