using VectorTerrain.Scripts.Graph;

namespace VectorTerrain.Scripts.Types.Interfaces
{
    public interface IReturnSectorData
    {
        SectorData GetSectorData(TerrainGraphInput thisInput);
    }
}