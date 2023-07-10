using System.Diagnostics;
using UnityEngine;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;
using VectorTerrain.Scripts.Utils.Burst;

using Debug = UnityEngine.Debug;

namespace VectorTerrain.Scripts.Nodes.Filters
{
    [CreateNodeMenu("Filters/RemoveClosePoints")]
    public class RemoveClosePoints : GeometryModifierNode
    {
        [Range(0.01f, 10f)]
        public float MinDistance = 0.1f;

        public bool burst;
        protected override SectorData Process(SectorData input)
        {
            // input.Verts = VertexRemovePointsCloseToSegments.Process(input.Verts, MinDistance);

            //
            // if(burst)
            //     input.Verts = VertexProximityFilterBurst.Process(input.Verts, MinDistance);
            // else 
                
            // Stopwatch sw = new Stopwatch();
            
            // sw.Start();
            // input.Verts = VertexProximityFilter.ProcessWithArrays(input.Verts, MinDistance);
            // sw.Stop();
            // Debug.Log("Arrays " + sw.ElapsedMilliseconds);
            
            // sw = new Stopwatch();
            // sw.Start();
            input.Verts = VertexProximityFilter.ProcessWithLists(input.Verts, MinDistance);
            // sw.Stop();
            
            // Debug.Log("Lists " + sw.ElapsedMilliseconds);
            
            // sw = new Stopwatch();
            // sw.Start();
            // input.Verts = VertexProximityFilterBurst.Process(input.Verts, MinDistance);
            // sw.Stop();
            
            
            
            return input;
        }
    }
}