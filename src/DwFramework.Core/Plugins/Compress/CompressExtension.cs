using K4os.Compression.LZ4.Streams;
using System.IO.Compression;

namespace DwFramework.Core.Compress;

public static class CompressExtension
{
    /// <summary>
    /// 压缩
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static async Task<byte[]> Compress(this byte[] bytes, CompressType type)
    {
        using var sourceStream = new MemoryStream(bytes);
        using var targetStream = new MemoryStream();
        using dynamic compressionStream = type switch
        {
            CompressType.Brotli => new BrotliStream(targetStream, CompressionMode.Compress),
            CompressType.GZip => new GZipStream(targetStream, CompressionMode.Compress),
            CompressType.LZ4 => LZ4Stream.Encode(targetStream),
            _ => throw new Exception("未知压缩类型")
        };
        await sourceStream.CopyToAsync(compressionStream);
        compressionStream.Close();
        return targetStream.ToArray();
    }

    /// <summary>
    /// 解压
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static async Task<byte[]> Decompress(this byte[] bytes, CompressType type)
    {
        using var soureStream = new MemoryStream(bytes);
        using var targetStream = new MemoryStream();
        using dynamic decompressionStream = type switch
        {
            CompressType.Brotli => new BrotliStream(soureStream, CompressionMode.Decompress),
            CompressType.GZip => new GZipStream(soureStream, CompressionMode.Decompress),
            CompressType.LZ4 => LZ4Stream.Decode(soureStream),
            _ => throw new Exception("未知压缩类型")
        };
        await decompressionStream.CopyToAsync(targetStream);
        return targetStream.ToArray();
    }
}