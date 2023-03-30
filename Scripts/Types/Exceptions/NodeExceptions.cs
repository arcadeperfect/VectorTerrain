using System;



public class RequiredInputNotConnected : Exception
{
    public RequiredInputNotConnected(string message) : base(message)
    {
        
    }
}

public class NullSectorDataException : Exception
{
    public NullSectorDataException()
    {
    }
}

public class NoOutputNodeException : Exception
{
    public NoOutputNodeException()
    {
    }
}

public class GuidMismatchException : Exception
{
    public GuidMismatchException()
    {
    }
}

public class NullSeedException : Exception
{
    public NullSeedException()
    {
    }
}