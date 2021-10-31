using System.Threading.Tasks;
using UrlShortener.Application.Domain;

namespace UrlShortener.Application.Services
{
    public interface IUrlShortener
    {
        public Task<string> ShortenUrl(LongUrlCommand longUrlCommand);
        public Task<string?> GetOriginalUrl(string shortUrl);
    }
}
