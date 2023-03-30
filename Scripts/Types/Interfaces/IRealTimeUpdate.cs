using System;

namespace VectorTerrain.Scripts.Types.Interfaces
{
    public interface IRealTimeUpdate
    {
        public event Action NodeUpdateEvent;
    }
}

