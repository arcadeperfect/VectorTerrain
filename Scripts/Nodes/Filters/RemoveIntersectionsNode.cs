using System.Diagnostics;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Utils;
using VectorTerrain.Scripts.Utils.Burst;
using VectorTools;
using XNode;
using Debug = UnityEngine.Debug;

namespace VectorTerrain.Scripts.Nodes.Filters
{
    [CreateNodeMenu("Filters/RemoveIntersections")]

    public class RemoveIntersectionsNode : GeometryModifierNode
    {
        public bool burst;
        protected override SectorData Process(SectorData input)
        {
          // input.Verts = VertexRemoveIntersections.Process(input.Verts);
          
          Stopwatch timer = new Stopwatch();
          
          
          timer.Start();
          
          if(burst)
              input.Verts = VertexRemoveIntersectionsBurst.Process(input.Verts);
          else
              input.Verts = VertexRemoveIntersections.Process(input.Verts);
          
          timer.Stop();

          // Debug.Log(timer.ElapsedMilliseconds);
          
          return input;
        }
    }
}