using System.Text.RegularExpressions;

namespace DwFramework.Extensions;

public static class StringExtension
{
    /// <summary>
    /// 字符转int
    /// </summary>
    /// <param name="char"></param>
    /// <returns></returns>
    private static int ToBase32Value(this char @char)
    {
        var value = @char.To<int>();
        return value switch
        {
            > 49 and < 56 => value - 24,
            > 64 and < 91 => value - 65,
            > 96 and < 123 => value - 97,
            _ => throw new Exception($"{@char} is not base32 char")
        };
    }

    /// <summary>
    /// int转字符
    /// </summary>
    /// <param name="byte"></param>
    /// <returns></returns>
    private static char ToBase32Char(this byte @byte)
    {
        return @byte switch
        {
            < 26 => (char)(@byte + 65),
            > 26 and < 32 => (char)(@byte + 24),
            _ => throw new Exception($"{@byte} is not base32 char value")
        };
    }

    /// <summary>
    /// 转Base32字符串
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToBase32(this byte[] bytes)
    {
        var charCount = (int)Math.Ceiling(bytes.Length / 5d) * 8;
        var returnArray = new char[charCount];
        var nextChar = (byte)0;
        var bitsRemaining = (byte)5;
        var arrayIndex = 0;
        foreach (var item in bytes)
        {
            nextChar = (byte)(nextChar | (item >> (8 - bitsRemaining)));
            returnArray[arrayIndex++] = ToBase32Char(nextChar);

            if (bitsRemaining < 4)
            {
                nextChar = (byte)((item >> (3 - bitsRemaining)) & 31);
                returnArray[arrayIndex++] = ToBase32Char(nextChar);
                bitsRemaining += 5;
            }

            bitsRemaining -= 3;
            nextChar = (byte)((item << bitsRemaining) & 31);
        };
        if (arrayIndex != charCount)
        {
            returnArray[arrayIndex++] = ToBase32Char(nextChar);
            while (arrayIndex != charCount) returnArray[arrayIndex++] = '=';
        }
        return new string(returnArray);
    }

    /// <summary>
    /// 转Base32字符数组
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static byte[] FromBase32(this string str)
    {
        str = str.TrimEnd('=');
        var byteCount = str.Length * 5 / 8;
        var returnArray = new byte[byteCount];
        var curByte = (byte)0;
        var bitsRemaining = (byte)8;
        var arrayIndex = 0;
        foreach (var item in str)
        {
            var cValue = ToBase32Value(item);
            int mask;
            if (bitsRemaining > 5)
            {
                mask = cValue << (bitsRemaining - 5);
                curByte = (byte)(curByte | mask);
                bitsRemaining -= 5;
            }
            else
            {
                mask = cValue >> (5 - bitsRemaining);
                curByte = (byte)(curByte | mask);
                returnArray[arrayIndex++] = curByte;
                curByte = (byte)(cValue << (3 + bitsRemaining));
                bitsRemaining += 3;
            }
        };
        if (arrayIndex != byteCount)
        {
            returnArray[arrayIndex] = curByte;
        }
        return returnArray;
    }

    /// <summary>
    /// 转Base64字符串
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToBase64(this byte[] bytes)
    {
        return Convert.ToBase64String(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Base64转字节数组
    /// </summary>
    /// <param name="base64String"></param>
    /// <returns></returns>
    public static byte[] FromBase64(this string base64String)
    {
        return Convert.FromBase64String(base64String);
    }

    /// <summary>
    /// 字节数组转Hex
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToHex(this byte[] bytes)
    {
        return Convert.ToHexString(bytes);
    }

    /// <summary>
    /// Hex转字节数组
    /// </summary>
    /// <param name="hexString"></param>
    /// <returns></returns>
    public static byte[] FromHex(this string hexString)
    {
        return Convert.FromHexString(hexString);
    }

    /// <summary>
    /// 是否为邮箱地址
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsEmailAddress(this string str)
    {
        var match = new Regex(@"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$").Match(str);
        return match.Success;
    }

    /// <summary>
    /// 计算相似度
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static double ComputeSimilarity(this string source, string target)
    {
        var len1 = source.Length;
        var len2 = target.Length;
        int[,] diff = new int[len1 + 1, len2 + 1];
        diff[0, 0] = 0;
        for (var i = 1; i <= len1; i++) diff[i, 0] = i;
        for (var i = 1; i <= len2; i++) diff[0, i] = i;
        var ch1 = source.ToCharArray();
        var ch2 = target.ToCharArray();
        for (var i = 1; i <= len1; i++)
        {
            for (var j = 1; j <= len2; j++)
            {
                var min = new int[] { diff[i - 1, j - 1], diff[i - 1, j], diff[i, j - 1] }.Min();
                diff[i, j] = ch1[i - 1] == ch2[j - 1] ? min : min + 1;
            }
        }
        return 1 - (double)diff[len1, len2] / Math.Max(len1, len2);
    }

    /// <summary>
    /// 是否为全角字符
    /// </summary>
    /// <param name="char"></param>
    /// <returns></returns>
    public static bool IsSBC(this char @char)
    {
        var pattern = @"^[\uFF00-\uFFFF]$";
        if (Regex.IsMatch(@char.ToString(), pattern)) return true;
        return false;
    }
}