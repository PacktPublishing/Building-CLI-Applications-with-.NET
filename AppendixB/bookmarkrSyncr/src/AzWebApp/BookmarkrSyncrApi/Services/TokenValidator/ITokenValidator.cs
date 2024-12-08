

public interface ITokenValidator
{
    bool IsValid(string token);
    bool IsExpired(string token);
}