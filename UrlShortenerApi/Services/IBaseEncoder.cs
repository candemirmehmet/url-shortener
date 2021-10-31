namespace UrlShortener.Application.Services
{
    public interface IBaseEncoder
    {
        string Encode(long value);
        long Decode(string encodedText);
    }
}