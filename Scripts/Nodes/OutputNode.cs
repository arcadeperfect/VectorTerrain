using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Nodes;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Types.Interfaces;
using VectorTerrain.Scripts.Utils;

namespace Nodez.Nodes
{
    [CreateNodeMenu("Output")]
    [DisallowMultipleNodes]
    public class OutputNode : TerrainNode
    {
        public bool nodePaused;
        
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        public SectorDataNoodle Input;


        public SectorData GetSectorData(TerrainGraphInput terrainGraphInput)
        {
            foreach (var plot in Graph.graphPlotList)
            {
                plot.Xvals.Clear();
                plot.YVals.Clear();
            }

            if (nodePaused) return null;
            
            var port = GetPort(nameof(Input));
            
            if (!port.IsConnected) return null;
            
            var inputNode = port.GetConnection(0).node;
            
            if (!inputNode.GetType().ImplementsOpenGenericInterface(typeof(IReturnSectorData))) return null;
            
            var returnSectorDataNode = inputNode as IReturnSectorData;
            var sectorData = returnSectorDataNode.GetSectorData(terrainGraphInput);
            
            if (sectorData == null) return null;
            
            if (sectorData.Verts.Count < 2) throw new SectorData.SectorDataEmptyException();

            sectorData.ProcessNormals();
            var y = OffsetToOriginalStartPoint(sectorData.Verts);
            sectorData.Verts = y;
            
            return sectorData;

        }

        private List<Vertex2> OffsetToOriginalStartPoint(List<Vertex2> Verts)
        {
            Vector2 delta = TerrainGraphInput.StartPos - Verts[0];
            if (delta == Vector2.zero) return Verts;
            return VertexProcessing.Offset(Verts, delta);
        }
    }
}