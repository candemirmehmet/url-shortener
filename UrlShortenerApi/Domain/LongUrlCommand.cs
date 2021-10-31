using System;

namespace UrlShortener.Application.Domain
{
    public record LongUrlCommand(string OriginalUrl, string? CustomAlias, TimeSpan? TimeToLive);

    public record ShortUrlResponse(string ShortUrl);

    public record OriginalUrlResponse(string LongUrl);
}