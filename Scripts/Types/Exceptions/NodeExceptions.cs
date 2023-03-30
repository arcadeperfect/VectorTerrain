using System;

public class RequiredInputNotConnected : Exception
{
    public RequiredInputNotConnected(string message) : base(message)
    {
    }
}

public class NoGraphException : Exception
{
    public NoGraphException(string message) : base(message)
    {
    }
}


public class NullSectorDataException : Exception
{
}

public class NoOutputNodeException : Exception
{
}

public class GuidMismatchException : Exception
{
}

public class NullSeedException : Exception
{
}