using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.DbContext;
using UrlShortener.Application.Domain;

namespace UrlShortener.Application.Services
{
    public class UrlShortenerService : IUrlShortener
    {
        private readonly IKeyGenerator _keyGenerator;
        private readonly IBaseEncoder _baseEncoder;
        private readonly IServiceProvider _serviceProvider;

        // A better approach would be to make it configurable via appsettings.json 
        private const string RootDomainName = @"http://sho.com/";

        public UrlShortenerService(
            IKeyGenerator keyGenerator,
            IBaseEncoder baseEncoder,
            IServiceProvider serviceProvider)
        {
            _keyGenerator = keyGenerator;
            _baseEncoder = baseEncoder;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> ShortenUrl(LongUrlCommand longUrlCommand)
        {
            // https://xxx.com/abc-xyz?id=qwert
            // https%3A%2F%2Fxxx.com%2Fabc-xyz%3Fid%3Dqwert
            // both urls are the same url
            string urlUtf8Format = HttpUtility.UrlDecode(longUrlCommand.OriginalUrl);

            long urlId = _keyGenerator.GenerateKey(urlUtf8Format);
            bool useCustomAlias = !string.IsNullOrWhiteSpace(longUrlCommand.CustomAlias);
            DateTime? expirationTime = longUrlCommand.TimeToLive == null ? null : DateTime.Now + longUrlCommand.TimeToLive;

            return await ExecuteInScope(urlId, async (serviceScope, urlIdAsParameter) =>
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<UrlShortenerContext>();
                ShortUrl? existingShortUrl = await context.ShortUrls.SingleOrDefaultAsync(u => u.UrlId == urlIdAsParameter);

                if (existingShortUrl != null)
                    return $"{RootDomainName}{existingShortUrl.CustomAlias ?? existingShortUrl.EncodedUrl}";

                var shortUrl = new ShortUrl
                {
                    UrlId = urlIdAsParameter,
                    OriginalUrl = longUrlCommand.OriginalUrl,
                    EncodedUrl = _baseEncoder.Encode(urlIdAsParameter), // custom urls have id as well because this column has a unique index
                    CustomAlias = useCustomAlias ? longUrlCommand.CustomAlias : null, // no unique index, just index
                    ExpirationDate = expirationTime
                };

                await context.ShortUrls.AddAsync(shortUrl);
                await context.SaveChangesAsync();
                return $"{RootDomainName}{shortUrl.CustomAlias ?? shortUrl.EncodedUrl}";
            });

        }

        public async Task<string?> GetOriginalUrl(string shortUrl)
        {
            string urlUtf8Format = HttpUtility.UrlDecode(shortUrl).Replace(RootDomainName, string.Empty);

            long urlId = _baseEncoder.Decode(urlUtf8Format);

            return await ExecuteInScope(urlId, async (serviceScope, urlIdAsParameter) =>
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<UrlShortenerContext>();
                ShortUrl? shortenedUrl = await context.ShortUrls.SingleOrDefaultAsync(u => u.UrlId == urlIdAsParameter)
                                   ?? await context.ShortUrls.SingleOrDefaultAsync(u => u.CustomAlias == urlUtf8Format); // either shortUrl or customAlias

                // either custom alias or encodedUrl
                return shortenedUrl?.OriginalUrl;
            });
        }

        private async Task<T> ExecuteInScope<TE, T>(TE parameter, Func<IServiceScope, TE, Task<T>> func)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return await func(scope, parameter).ConfigureAwait(false);
            }
        }

    }
}