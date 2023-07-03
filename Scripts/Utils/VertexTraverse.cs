using System.Collections.Generic;
using UnityEngine;
using VectorTerrain.Scripts.Types;
using static VectorTerrain.Scripts.Utils.VertexProcessing;

namespace VectorTerrain.Scripts.Utils
{
    public static class VertexTraverse
    {
        public static Vertex2 Traverse(List<Vertex2> verts, float targetDistance, bool normalized)
        {
            
            var totalDistance = VertexPathLength(verts);

            if (targetDistance < 0)
                throw new SectorData.TraversException("target distance was negative  -> " + targetDistance + " : " +
                                                      totalDistance);

            if (targetDistance > totalDistance)
                throw new SectorData.TraversException("target distance was greater than totalDistance  -> " + targetDistance +
                                                      " : " + totalDistance);

            if (normalized)
                targetDistance = totalDistance * targetDistance;

            targetDistance = Mathf.Clamp(targetDistance, 0, totalDistance);

            var lerped = new Vertex2();

            float distanceSoFar = 0;
            for (var i = 0; i < verts.Count - 1; i++)
            {
                if (i < verts.Count - 1 && verts[i + 1].Dist == null)
                    throw new SectorData.TraversException("Next vertex distance was null");

                var thisDist = (float) verts[i].Dist;
                var nextDist = (float) verts[i + 1].Dist;

                distanceSoFar += thisDist;

                // continue looping until we know the target point is closer than the next vert
                var z = distanceSoFar + nextDist;
                if (z < targetDistance)
                    continue;

                // if we are here,the target point lies before the next vert
                var remainingDistance = targetDistance - distanceSoFar;
                var lerpIndex = remainingDistance / nextDist;

                lerped = Vertex2.Lerp(verts[i], verts[i + 1], lerpIndex);

                break;
            }

            return lerped;
        }
    }
}