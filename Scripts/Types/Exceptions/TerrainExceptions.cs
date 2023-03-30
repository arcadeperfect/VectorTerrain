using System;

namespace VectorTerrain.Scripts.Types.Exceptions
{
    public class TerrainExceptions
    {
        public class InvalidSectorDataException : Exception
        {
            public InvalidSectorDataException(string message) : base(message)
            {
            }
        }

        public class NoValidInputException : Exception
        {
        }
        
        public class NotInitialisedException : Exception
        {
            public NotInitialisedException(string message) : base(message)
            {
            }
        }
    }
}