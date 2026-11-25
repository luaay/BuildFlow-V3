using System.Security.Cryptography;
using System.Text;

namespace BuildFlow.Identity.Application.Features.Users;

// Activation tokens are stored as SHA-256 hashes.
// The raw token only travels in the invitation link.
internal static class ActivationTokenHasher
{
    public static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}