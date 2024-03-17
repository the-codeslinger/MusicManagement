using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MusicManagementCore.Util
{
    public class DataHasher
    {
        public static string ComputeOfString(string data)
        {
            using (SHA256 hasher = SHA256.Create())
            {
                var digest = hasher.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToHexString(digest);
            }
        }

        public static string ComputeOfFile(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                using (SHA256 hasher = SHA256.Create())
                {
                    var digest = hasher.ComputeHash(stream);
                    return Convert.ToHexString(digest);
                }
            }
        }
    }
}
