using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Domain;
using UrlShortener.Application.Services;

namespace UrlShortener.Application.Controllers
{
    [ApiController]
    [Route("")]
    public class UrlShortenerController : ControllerBase
    {
        private readonly ILogger<UrlShortenerController> _logger;
        private readonly IUrlShortener _urlShortenerService;

        public UrlShortenerController(
            ILogger<UrlShortenerController> logger,
            IUrlShortener urlShortenerService)
        {
            _logger = logger;
            _urlShortenerService = urlShortenerService;
        }

        [HttpPost]
        [Route("shortenUrl")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ShortUrlResponse>> CreateShortUrl(LongUrlCommand longUrlCommand)
        {
            string shortenedUrl = await _urlShortenerService.ShortenUrl(longUrlCommand);
            _logger.LogDebug($"URL [{longUrlCommand.OriginalUrl}] mapped to [{shortenedUrl}]");
            return Created($"/{shortenedUrl}", new ShortUrlResponse(shortenedUrl));
        }

        [HttpGet]
        [Route("{shortUrl}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<OriginalUrlResponse>> GetOriginalUrl(string shortUrl)
        {
            string? originalUrl = await _urlShortenerService.GetOriginalUrl(shortUrl);

            if (originalUrl != null)
            {
                return Ok(new OriginalUrlResponse(originalUrl));
            }

            string errorMessage = "No saved URL found for alias { shortUrl}";
            _logger.LogError(errorMessage);
            return NotFound(errorMessage);

        }

        // this method is useful when redirecting to original url in browser is required
        //[HttpGet]
        //[Route("/{shortUrl}")]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status302Found)]
        //public async Task<IActionResult> GetOriginalUrl(string shortUrl)
        //{
        //    string? originalUrl = await _urlShortenerService.GetOriginalUrl(shortUrl);

        //    if (originalUrl is null)
        //    {
        //        string errorMessage = "No saved URL found for alias { shortUrl}";
        //        _logger.LogError(errorMessage);
        //        return NotFound(errorMessage);
        //    }

        //    _logger.LogDebug($"Redirecting to : [{originalUrl}]");
        //    // 302 Found, not "301 page moved"
        //    return Redirect(originalUrl);
        //}
    }
}