using System.Text;
using System.Security.Cryptography;

namespace DwFramework.Core.Encrypt;

public static class SHA
{
    /// <summary>
    /// 编码
    /// </summary>
    /// <param name="type"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string? Encode(SHAType type, byte[] data)
    {
        byte[]? bytes = null;
        switch (type)
        {
            case SHAType.SHA1:
                var sha1 = SHA1.Create();
                bytes = sha1.ComputeHash(data, 0, data.Length);
                sha1.Dispose();
                break;
            case SHAType.SHA256:
                var sha256 = SHA256.Create();
                bytes = sha256.ComputeHash(data, 0, data.Length);
                sha256.Dispose();
                break;
            case SHAType.SHA384:
                var sha384 = SHA384.Create();
                bytes = sha384.ComputeHash(data, 0, data.Length);
                sha384.Dispose();
                break;
            case SHAType.SHA512:
                var sha512 = SHA512.Create();
                bytes = sha512.ComputeHash(data, 0, data.Length);
                sha512.Dispose();
                break;
        }
        if (bytes == null || bytes.Length <= 0) return null;
        var builder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
        return builder.ToString();
    }
}