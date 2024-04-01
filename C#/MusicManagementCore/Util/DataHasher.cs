using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MusicManagementCore.Util;

/// <summary>
/// A helper class to generate SHA256 hashes of strings or files.
/// </summary>
public class DataHasher
{
    /// <summary>
    /// Compute a SHA256 hash of the given string.
    /// </summary>
    /// <param name="data">The string to hash.</param>
    /// <returns>A hex string of the computed hash.</returns>
    public static string ComputeOfString(string data)
    {
        using (SHA256 hasher = SHA256.Create()) {
            var digest = hasher.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(digest);
        }
    }

    /// <summary>
    /// Compute a SHA256 hash of a file's contents.
    /// </summary>
    /// <param name="fileName">The file's location (relative or absolute).</param>
    /// <returns>A hex string of the computed hash.</returns>
    public static string ComputeOfFile(string fileName)
    {
        using (var stream = new FileStream(fileName, FileMode.Open)) {
            using (SHA256 hasher = SHA256.Create()) {
                var digest = hasher.ComputeHash(stream);
                return Convert.ToHexString(digest);
            }
        }
    }
}
