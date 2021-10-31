using System;
using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Application.Domain
{
    /// <summary>
    /// ShortUrl entity
    /// </summary>
    public class ShortUrl
    {
        [Key]
        public long UrlId { get; init; }
        
        [Required]
        [MaxLength(2048)]
        public string? OriginalUrl { get; set; }
        
        [Required]
        [MaxLength(6)]
        public string? EncodedUrl { get; set; }

        /// <summary>
        /// Separate column for custom URL in order to keep DB size optimal
        /// nvarchar(256)
        /// </summary>
        [MaxLength(256)]
        public string? CustomAlias{ get; set; }

        /// <summary>
        /// Will be filled automatically on insert
        /// </summary>
        public DateTime CreationDateTime { get; set; }

        /// <summary>
        /// will be set if a TTL is set
        /// </summary>
        public DateTime? ExpirationDate { get; set; }
    }
}
