using System.Text;

namespace DwFramework.Core.Encrypt;

public static class MD5
{
    /// <summary>
    /// 编码
    /// </summary>
    /// <param name="type"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Encode(MD5Type type, byte[] data)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var bytes = md5.ComputeHash(data);
        var builder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
        string result = null;
        switch (type)
        {
            case MD5Type.MD5_16:
                result = builder.ToString()[8..24];
                break;
            case MD5Type.MD5_32:
                result = builder.ToString();
                break;
        }
        return result;
    }
}