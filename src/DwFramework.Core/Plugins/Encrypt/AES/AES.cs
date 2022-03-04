using System.Security.Cryptography;

namespace DwFramework.Core.Encrypt;

public static class AES
{
    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="type"></param>
    /// <param name="data"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <param name="mode"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    public static byte[] Encrypt(AESType type, byte[] data, byte[] key, byte[] iv = null, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.None)
    {
        if (data == null || data.Length <= 0) throw new ExceptionBase(ExceptionType.Parameter, 0, "data为空");
        if (key == null) throw new ExceptionBase(ExceptionType.Parameter, 0, "key为空");
        var keySize = (int)type;
        if (key.Length != keySize / 8) Array.Resize(ref key, keySize / 8);
        using var aes = Aes.Create();
        aes.KeySize = keySize;
        aes.Mode = mode;
        aes.Padding = padding;
        if (iv == null) iv = new byte[aes.BlockSize / 8];
        else Array.Resize(ref iv, aes.BlockSize / 8);
        using var encryptor = aes.CreateEncryptor(key, iv);
        return encryptor.TransformFinalBlock(data, 0, data.Length);
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="type"></param>
    /// <param name="data"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <param name="mode"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    public static byte[] Decrypt(AESType type, byte[] data, byte[] key, byte[] iv = null, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.None)
    {
        if (data == null || data.Length <= 0) throw new ExceptionBase(ExceptionType.Parameter, 0, "data为空");
        if (key == null) throw new ExceptionBase(ExceptionType.Parameter, 0, "key为空");
        var keySize = (int)type;
        if (key.Length != keySize / 8) Array.Resize(ref key, keySize / 8);
        using var aes = Aes.Create();
        aes.KeySize = keySize;
        aes.Mode = mode;
        aes.Padding = padding;
        if (iv == null) iv = new byte[aes.BlockSize / 8];
        else Array.Resize(ref iv, aes.BlockSize / 8);
        using var decryptor = aes.CreateDecryptor(key, iv);
        return decryptor.TransformFinalBlock(data, 0, data.Length);
    }
}