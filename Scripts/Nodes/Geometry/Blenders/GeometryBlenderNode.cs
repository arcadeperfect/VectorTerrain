using Sirenix.Utilities;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Types.Interfaces;
using XNode;

namespace VectorTerrain.Scripts.Nodes.Geometry.Blenders
{
    public abstract class GeometryBlenderNode : TerrainNode
    {
        public bool disable;
        
        [Input(typeConstraint = TypeConstraint.Strict,connectionType = ConnectionType.Override)]
        public GeometryNoodle Input1;
        [Input(typeConstraint = TypeConstraint.Strict,connectionType = ConnectionType.Override)]
        public GeometryNoodle Input2;
        [Input(typeConstraint = TypeConstraint.Strict,connectionType = ConnectionType.Override)]
        public MaskNoodle Mask;
        
        [Output] public GeometryNoodle Output;
    
        public override object GetValue(NodePort port) {
            return null; 
        }

        protected abstract SectorData Process(SectorData input1, SectorData input2);
        public virtual SectorData GetSectorData(TerrainGraphInput thisInput)
        {
            var v = GetGeometryInputNodes()[0];
            var w  = GetGeometryInputNodes()[1];
            
            int count = 0;
            
            if (v.GetType().ImplementsOpenGenericInterface(typeof(IReturnSectorData)))
            {
                if (disable)
                    return v.GetSectorData(thisInput);
                var returnMe = Process(v.GetSectorData(thisInput), w.GetSectorData(thisInput));
                // if (GetMaskInputPorts()[0].IsConnected) _plotsList.Add(GrabMaskRealQuick(returnMe.Verts.Count));
                
                return returnMe;
            }
            return null;
        }
    }
}

