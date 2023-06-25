using Sirenix.Utilities;
using VectorTerrain.Scripts.Graph;
using VectorTerrain.Scripts.Types;
using VectorTerrain.Scripts.Types.Interfaces;

namespace VectorTerrain.Scripts.Nodes.Filters
{
    public abstract class GeometryModifierNode : TerrainNode, IReturnSectorData//, IReturnVizPlots
    {
        public bool disable;
        
        [Input(typeConstraint = TypeConstraint.Strict,connectionType = ConnectionType.Override)]
        public GeometryNoodle Input;
        
        [Output] 
        public GeometryNoodle Output;
        protected abstract SectorData Process(SectorData input);
        public virtual SectorData GetSectorData(TerrainGraphInput thisInput)
        {
            var inputNode = GetGeometryInputNodes()[0];
            if (inputNode.GetType().ImplementsOpenGenericInterface(typeof(IReturnSectorData)))
            {
                if (disable) return inputNode.GetSectorData(thisInput);
                return Process(inputNode.GetSectorData(thisInput));
            }
            return null;
        }
    }
}