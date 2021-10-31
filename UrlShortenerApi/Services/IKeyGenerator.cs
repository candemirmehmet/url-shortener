namespace UrlShortener.Application.Services
{
    /// <summary>
    /// An interface that generates long keys for encoded urls
    /// These keys will be used to create bijection between encoded url and original url
    /// Generated keys will further be used as primary key of shortened url entities
    /// A better approach would be to generate keys offline and store them in DB
    /// </summary>
    public interface IKeyGenerator
    {
        long GenerateKey(string url);
    }
}