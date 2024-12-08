

public class TokenValidator : ITokenValidator
{

    private readonly List<PatToken> _tokens = new();

    public TokenValidator()
    {
        // we are simulating a token store here... 
        _tokens.Add(new PatToken { Value = "4de3b2b9-afaf-406c-ab0d-d59ac534411d", IsExpired = false });
        _tokens.Add(new PatToken { Value = "16652977-c654-431e-8f84-bd53b4ccd47d", IsExpired = true });
    }

    public bool IsExpired(string token)
    {
        var retrievedToken = _tokens.FirstOrDefault(t => t.Value.Equals(token, StringComparison.OrdinalIgnoreCase));
        if(retrievedToken == null) return false;
        return retrievedToken.IsExpired;
    }

    public bool IsValid(string token)
    {
        var retrievedToken = _tokens.FirstOrDefault(t => t.Value.Equals(token, StringComparison.OrdinalIgnoreCase));
        return retrievedToken != null;
    }
}