using System;
using System.Linq;

namespace UrlShortener.Application.Services
{
    public class Base62Encoder : IBaseEncoder
    {
        /// <summary>
        /// List of acceptable characters in our shortenedUrls
        /// Notice that our alphabet does start with capital letters
        /// it will increase readability for small values
        /// for instance 'AAAAA1' is equal to 1
        /// It would be represented as '000001' if the alphabet starts with digits
        /// </summary>
        private const string Base62Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + "0123456789" + "abcdefghijklmnopqrstuvwxyz";
        
        private static readonly int TargetBase = Base62Alphabet.Length;
        
        /// <summary>
        /// Converts a base10 number to base 62
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Encode(long value)
        {
            string encodedText = string.Empty;
            while (value > 0)
            {
                encodedText = $"{Base62Alphabet[(int)(value % TargetBase)]}{encodedText}";
                value /= TargetBase;
            }

            // padding with first digit in base62 (decimal 1 => AAAAA1 in base62)
            // it is not clear whether shortened URL can be less than 6 chars
            // according to first example http://sho.com/a1b2 => it is allowed

            //private static readonly char PaddingChar = Base62Alphabet.First();
            //private const int EncodedTextMinWidth = 6;

            //if (encodedText.Length < EncodedTextMinWidth)
            //{
            //    encodedText = encodedText.PadLeft(6, PaddingChar);
            //}

            return encodedText;
        }

        /// <summary>
        /// Decodes a shortened URL
        /// </summary>
        /// <param name="encodedText"></param>
        /// <returns>numerical value (base10) of the string in base62</returns>
        public long Decode(string encodedText)
        {
            return encodedText
                .Reverse() // collection of chars in reverse order
                .Select(t => Base62Alphabet.IndexOf(t)) // find each characters digit value i.e. each character's position in Base62Alphabet
                .Select((digitValue, exponent) => digitValue * (long) Math.Pow(TargetBase, exponent)) // compute each characters numerical value
                .Sum(); // sum all character's numerical value
        }
    }
}