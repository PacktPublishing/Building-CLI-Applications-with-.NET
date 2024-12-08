

namespace bookmarkr.Exceptions;


public class PatNotFoundException : Exception
{
    public string Pat { get; }

    public PatNotFoundException(string pat)
        : base($"The specified PAT '{pat}' was not found.")
    {
        Pat = pat;
    }
}


public class PatInvalidException : Exception
{
    public string Pat { get; }

    public PatInvalidException(string pat)
        : base($"The specified PAT '{pat}' is invalid.")
    {
        Pat = pat;
    }
}


public class PatExpiredException : Exception
{
    public string Pat { get; }

    public PatExpiredException(string pat)
        : base($"The specified PAT '{pat}' is expired.")
    {
        Pat = pat;
    }
}