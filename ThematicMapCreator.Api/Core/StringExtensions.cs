using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ThematicMapCreator.Api.Core
{
    public static class StringExtensions
    {
        public static string GetSha256(this string input)
        {
            using SHA256 hashAlgorithm = SHA256.Create();
            return input.GetSha256(hashAlgorithm);
        }

        public static string GetSha256(this string input, SHA256 hashAlgorithm)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            return data.Aggregate(
                    new StringBuilder(),
                    (builder, b) => builder.Append(b.ToString("x2")))
                .ToString();
        }
    }
}
