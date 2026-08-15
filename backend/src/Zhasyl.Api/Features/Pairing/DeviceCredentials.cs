using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace Zhasyl.Api.Features.Pairing;

public static class DeviceCredentials
{
    private const string PairingAlphabet = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";

    public static string CreatePairingCode()
    {
        Span<char> characters = stackalloc char[8];
        for (var index = 0; index < characters.Length; index++)
        {
            characters[index] = PairingAlphabet[RandomNumberGenerator.GetInt32(PairingAlphabet.Length)];
        }

        return $"{new string(characters[..4])}-{new string(characters[4..])}";
    }

    public static string CreateSessionToken() =>
        WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

    public static string NormalizePairingCode(string code) =>
        code.Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .ToUpperInvariant();

    public static string Hash(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}
