using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UrlShortener.Application.Services
{
    public class HashKeyGenerator : IKeyGenerator
    {
        /// <summary>
        /// About ~56.8 billion unique tiny URL possible
        /// </summary>
        private static readonly long MaxPossibilitiesBase62 = (long)Math.Pow(62, 6);

        // Warning: There will be collisions because
        // Hash is 32 Bytes and we take only 8 bytes 
        // in addition MaxPossibilitiesBase62 = 56800235584 (~56.8 billion) and is only 6 bytes
        public long GenerateKey(string url)
        {
            // SHA256 creates 32 bytes and long requires only 8
            int notRequiredBytes = 32 - sizeof(long);
            byte[] urlAsBytes = Encoding.UTF8.GetBytes(url);

            // two same urls will generate the same hash value
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(urlAsBytes)
                .Skip(notRequiredBytes) // takes last 8 bytes, collision possible
                .ToArray();

            // convert to long 
            return BitConverter.ToInt64(hashBytes, 0) & (MaxPossibilitiesBase62 - 1);
        }

    }
}