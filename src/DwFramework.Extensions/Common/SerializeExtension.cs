using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace DwFramework.Extensions;

public static class SerializeExtension
{
    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="options"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string ToJson<T>(this T obj, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// 尝试序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="json"></param>
    /// <param name="options"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryToJson<T>(this T obj, out string json, JsonSerializerOptions? options = null)
    {
        try
        {
            json = obj.ToJson(options);
            return true;
        }
        catch
        {
            json = string.Empty;
            return false;
        }
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="options"></param>
    /// <param name="encoding"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static byte[] ToJsonBytes<T>(this T obj, JsonSerializerOptions? options = null, Encoding? encoding = null)
    {
        return obj.ToJson(options).ToBytes(encoding);
    }

    /// <summary>
    /// 尝试序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="jsonBytes"></param>
    /// <param name="options"></param>
    /// <param name="encoding"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryToJsonBytes<T>(this T obj, out byte[] jsonBytes, JsonSerializerOptions? options = null, Encoding? encoding = null)
    {
        try
        {
            jsonBytes = obj.ToJsonBytes<T>(options, encoding);
            return true;
        }
        catch
        {
            jsonBytes = Array.Empty<byte>();
            return false;
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="str"></param>
    /// <param name="type"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static object? FromJson(this string str, Type type, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize(str, type, options);
    }

    /// <summary>
    /// 尝试反序列化
    /// </summary>
    /// <param name="str"></param>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static bool TryFromJson(this string str, Type type, out object? obj, JsonSerializerOptions? options = null)
    {
        try
        {
            obj = str.FromJson(type, options);
            return true;
        }
        catch
        {
            obj = null;
            return false;
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="str"></param>
    /// <param name="options"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? FromJson<T>(this string str, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(str, options);
    }

    /// <summary>
    /// 尝试反序列化
    /// </summary>
    /// <param name="str"></param>
    /// <param name="obj"></param>
    /// <param name="options"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryFromJson<T>(this string str, out T? obj, JsonSerializerOptions? options = null)
    {
        try
        {
            obj = str.FromJson<T>(options);
            return true;
        }
        catch
        {
            obj = default(T);
            return false;
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="type"></param>
    /// <param name="encoding"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static object? FromJsonBytes(this byte[] bytes, Type type, JsonSerializerOptions? options = null, Encoding? encoding = null)
    {
        return bytes.FromBytes(encoding).FromJson(type, options);
    }

    /// <summary>
    /// 尝试反序列化
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    /// <param name="options"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static bool TryFromJsonBytes(this byte[] bytes, Type type, out object? obj, JsonSerializerOptions? options = null, Encoding? encoding = null)
    {
        try
        {
            obj = bytes.FromJsonBytes(type, options);
            return true;
        }
        catch
        {
            obj = null;
            return false;
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bytes"></param>
    /// <param name="encoding"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static T? FromJsonBytes<T>(this byte[] bytes, JsonSerializerOptions? options = null, Encoding? encoding = null)
    {
        return bytes.FromBytes(encoding).FromJson<T>(options);
    }

    /// <summary>
    /// 尝试反序列化
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="obj"></param>
    /// <param name="options"></param>
    /// <param name="encoding"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryFromJsonBytes<T>(this byte[] bytes, out T? obj, JsonSerializerOptions? options = null, Encoding? encoding = null)
    {
        try
        {
            obj = bytes.FromJsonBytes<T>(options, encoding);
            return true;
        }
        catch
        {
            obj = default(T);
            return false;
        }
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static byte[] ToXmlBytes(this object obj, Type type, Encoding? encoding = null)
    {
        using var output = new MemoryStream();
        using var writer = new XmlTextWriter(output, (encoding ??= Encoding.UTF8));
        var serializer = new XmlSerializer(type);
        serializer.Serialize(writer, obj);
        return output.ToArray();
    }

    /// <summary>
    /// 尝试序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type"></param>
    /// <param name="xmlBytes"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static bool TryToXmlBytes(this object obj, Type type, out byte[] xmlBytes, Encoding? encoding = null)
    {
        try
        {
            xmlBytes = obj.ToXmlBytes();
            return true;
        }
        catch
        {
            xmlBytes = Array.Empty<byte>();
            return false;
        }
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static byte[] ToXmlBytes<T>(this T obj, Encoding? encoding = null)
    {
        if (obj is null) return Array.Empty<byte>();
        return obj.ToXmlBytes(typeof(T), encoding);
    }

    /// <summary>
    /// 尝试序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="xmlBytes"></param>
    /// <param name="encoding"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryToXmlBytes<T>(this T obj, out byte[] xmlBytes, Encoding? encoding = null)
    {
        try
        {
            xmlBytes = obj.ToXmlBytes<T>(encoding);
            return true;
        }
        catch
        {
            xmlBytes = Array.Empty<byte>();
            return false;
        }
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string ToXml(this object obj, Type type, Encoding? encoding = null)
    {
        return obj.ToXmlBytes(type, encoding).FromBytes(encoding);
    }

    /// <summary>
    /// 尝试序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type"></param>
    /// <param name="xml"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static bool TryToXml(this object obj, Type type, out string xml, Encoding? encoding = null)
    {
        try
        {
            xml = obj.ToXml(type, encoding);
            return true;
        }
        catch
        {
            xml = string.Empty;
            return false;
        }
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string ToXml<T>(this T obj, Encoding? encoding = null)
    {
        if (obj is null) return string.Empty;
        return obj.ToXml(typeof(T), encoding);
    }

    /// <summary>
    /// 尝试序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="xml"></param>
    /// <param name="encoding"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryToXml<T>(this T obj, out string xml, Encoding? encoding = null)
    {
        try
        {
            xml = obj.ToXml<T>(encoding);
            return true;
        }
        catch
        {
            xml = string.Empty;
            return false;
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object? FromXmlBytes(this byte[] bytes, Type type)
    {
        using var input = new MemoryStream(bytes);
        var serializer = new XmlSerializer(type);
        return serializer.Deserialize(input);
    }

    /// <summary>
    /// 尝试反序列化
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool TryFromXmlBytes(this byte[] bytes, Type type, out object? obj)
    {
        try
        {
            obj = bytes.FromXmlBytes(type);
            return true;
        }
        catch
        {
            obj = null;
            return false;
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static T? FromXmlBytes<T>(this byte[] bytes)
    {
        var obj = bytes.FromXmlBytes(typeof(T));
        return obj.To<T>();
    }

    /// <summary>
    /// 尝试反序列化
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryFromXmlBytes<T>(this byte[] bytes, out T? obj)
    {
        try
        {
            obj = bytes.FromXmlBytes<T>();
            return true;
        }
        catch
        {
            obj = default(T);
            return false;
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="str"></param>
    /// <param name="type"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static object? FromXml(this string str, Type type, Encoding? encoding = null)
    {
        return str.ToBytes(encoding).FromXmlBytes(type);
    }

    /// <summary>
    /// 尝试反序列化
    /// </summary>
    /// <param name="str"></param>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static bool TryFromXml(this string str, Type type, out object? obj, Encoding? encoding = null)
    {
        try
        {
            obj = str.FromXml(type, encoding);
            return true;
        }
        catch
        {
            obj = null;
            return false;
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="str"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static T? FromXml<T>(this string str, Encoding? encoding = null)
    {
        return str.ToBytes(encoding).FromXmlBytes<T>();
    }

    /// <summary>
    /// 尝试反序列化
    /// </summary>
    /// <param name="str"></param>
    /// <param name="obj"></param>
    /// <param name="encoding"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryFromXml<T>(this string str, out T? obj, Encoding? encoding = null)
    {
        try
        {
            obj = str.ToBytes(encoding).FromXmlBytes<T>();
            return true;
        }
        catch
        {
            obj = default(T);
            return false;
        }
    }
}