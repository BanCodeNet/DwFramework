using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace DwFramework.Core.Encrypt;

public static class RSA
{
    /// <summary>
    /// 填充位数
    /// </summary>
    private static readonly Dictionary<RSAPadding, RSAEncryptionPadding> PaddingMapper = new Dictionary<RSAPadding, RSAEncryptionPadding>()
    {
        [RSAPadding.Pkcs1] = RSAEncryptionPadding.Pkcs1,
        [RSAPadding.OaepSHA1] = RSAEncryptionPadding.OaepSHA1,
        [RSAPadding.OaepSHA256] = RSAEncryptionPadding.OaepSHA256,
        [RSAPadding.OaepSHA384] = RSAEncryptionPadding.OaepSHA384,
        [RSAPadding.OaepSHA512] = RSAEncryptionPadding.OaepSHA512
    };

    /// <summary>
    /// 生成RSA密钥对
    /// </summary>
    /// <param name="keySize"></param>
    /// <param name="format"></param>
    /// <param name="usePem"></param>
    /// <returns></returns>
    public static (string PrivateKey, string PublicKey) GenerateKeyPair(int keySize, RSAFormat format, bool usePem = false)
    {
        return format switch
        {
            RSAFormat.Pkcs1 => ExportPkcs1Key(keySize, usePem),
            RSAFormat.Pkcs8 => ExportPkcs8Key(keySize, usePem),
            RSAFormat.Xml => ExportXmlKey(keySize),
            _ => throw new ExceptionBase(ExceptionType.Parameter, 0, "未知格式")
        };
    }

    /// <summary>
    /// 使用Pem格式
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string EnablePublicKeyPem(string key)
    {
        if (key.StartsWith("-----BEGIN PUBLIC KEY-----")) return key;
        var res = new List<string>();
        res.Add("-----BEGIN PUBLIC KEY-----");
        int pos = 0;
        while (pos < key.Length)
        {
            var count = key.Length - pos < 64 ? key.Length - pos : 64;
            res.Add(key.Substring(pos, count));
            pos += count;
        }
        res.Add("-----END PUBLIC KEY-----");
        return string.Join("\n", res);
    }

    /// <summary>
    /// 禁用Pem格式
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string DisablePublicKeyPem(string key)
    {
        if (!key.StartsWith("-----BEGIN PUBLIC KEY-----")) return key;
        return key.Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "").Replace("\n", "");
    }

    /// <summary>
    /// 使用Pkcs1Pem格式
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string EnablePrivateKeyPkcs1Pem(string key)
    {
        if (key.StartsWith("-----BEGIN RSA PRIVATE KEY-----")) return key;
        var res = new List<string>();
        res.Add("-----BEGIN RSA PRIVATE KEY-----");
        int pos = 0;
        while (pos < key.Length)
        {
            var count = key.Length - pos < 64 ? key.Length - pos : 64;
            res.Add(key.Substring(pos, count));
            pos += count;
        }
        res.Add("-----END RSA PRIVATE KEY-----");
        return string.Join("\n", res);
    }

    /// <summary>
    /// 禁用Pkcs1Pem格式
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string DisablePrivateKeyPkcs1Pem(string key)
    {
        if (!key.StartsWith("-----BEGIN RSA PRIVATE KEY-----")) return key;
        return key.Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "").Replace("\n", "");
    }

    /// <summary>
    /// 使用Pkcs8Pem格式
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string EnablePrivateKeyPkcs8Pem(string key)
    {
        if (key.StartsWith("-----BEGIN PRIVATE KEY-----")) return key;
        var res = new List<string>();
        res.Add("-----BEGIN PRIVATE KEY-----");
        int pos = 0;
        while (pos < key.Length)
        {
            var count = key.Length - pos < 64 ? key.Length - pos : 64;
            res.Add(key.Substring(pos, count));
            pos += count;
        }
        res.Add("-----END PRIVATE KEY-----");
        return string.Join("\n", res);
    }

    /// <summary>
    /// 禁用Pkcs8Pem格式
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string DisablePrivateKeyPkcs8Pem(string key)
    {
        if (!key.StartsWith("-----BEGIN PRIVATE KEY-----")) return key;
        return key.Replace("-----BEGIN PRIVATE KEY-----", "").Replace("-----END PRIVATE KEY-----", "").Replace("\n", "");
    }

    /// <summary>
    /// 导出Pkcs1密钥
    /// </summary>
    /// <param name="keySize"></param>
    /// <param name="usePem"></param>
    /// <returns></returns>
    private static (string PrivateKey, string PublicKey) ExportPkcs1Key(int keySize, bool usePem = false)
    {
        var kpGen = GeneratorUtilities.GetKeyPairGenerator("RSA");
        kpGen.Init(new KeyGenerationParameters(new SecureRandom(), keySize));
        var keyPair = kpGen.GenerateKeyPair();
        using var sw = new StringWriter();
        var pWrt = new PemWriter(sw);
        pWrt.WriteObject(keyPair.Private);
        pWrt.Writer.Close();
        var privateKey = sw.ToString();
        if (!usePem) privateKey = DisablePrivateKeyPkcs1Pem(privateKey);
        using var swpub = new StringWriter();
        var pWrtpub = new PemWriter(swpub);
        pWrtpub.WriteObject(keyPair.Public);
        pWrtpub.Writer.Close();
        string publicKey = swpub.ToString();
        if (!usePem) publicKey = DisablePrivateKeyPkcs1Pem(publicKey);
        return (privateKey, publicKey);
    }

    /// <summary>
    /// 导出Pkcs8密钥
    /// </summary>
    /// <param name="keySize"></param>
    /// <param name="usePem"></param>
    /// <returns></returns>
    private static (string PrivateKey, string PublicKey) ExportPkcs8Key(int keySize, bool usePem = false)
    {
        var kpGen = GeneratorUtilities.GetKeyPairGenerator("RSA");
        kpGen.Init(new KeyGenerationParameters(new SecureRandom(), keySize));
        var keyPair = kpGen.GenerateKeyPair();
        using var swpri = new StringWriter();
        var pWrtpri = new PemWriter(swpri);
        var pkcs8 = new Pkcs8Generator(keyPair.Private);
        pWrtpri.WriteObject(pkcs8);
        pWrtpri.Writer.Close();
        string privateKey = swpri.ToString();
        if (!usePem) privateKey = DisablePrivateKeyPkcs8Pem(privateKey);
        using var swpub = new StringWriter();
        var pWrtpub = new PemWriter(swpub);
        pWrtpub.WriteObject(keyPair.Public);
        pWrtpub.Writer.Close();
        string publicKey = swpub.ToString();
        if (!usePem) publicKey = DisablePrivateKeyPkcs8Pem(publicKey);
        return (privateKey, publicKey);
    }

    /// <summary>
    /// 导出XML密钥
    /// </summary>
    /// <param name="keySize"></param>
    /// <returns></returns>
    private static (string PrivateKey, string PublicKey) ExportXmlKey(int keySize = 1024)
    {
        using var rsa = System.Security.Cryptography.RSA.Create();
        rsa.KeySize = keySize;
        var rsap = rsa.ExportParameters(true);

        var privatElement = new XElement("RSAKeyValue");
        //Modulus
        var primodulus = new XElement("Modulus", rsap.Modulus?.ToBase64());
        //Exponent
        var priexponent = new XElement("Exponent", rsap.Exponent?.ToBase64());
        //P
        var prip = new XElement("P", rsap.P?.ToBase64());
        //Q
        var priq = new XElement("Q", rsap.Q?.ToBase64());
        //DP
        var pridp = new XElement("DP", rsap.DP?.ToBase64());
        //DQ
        var pridq = new XElement("DQ", rsap.DQ?.ToBase64());
        //InverseQ
        var priinverseQ = new XElement("InverseQ", rsap.InverseQ?.ToBase64());
        //D
        var prid = new XElement("D", rsap.D?.ToBase64());

        privatElement.Add(primodulus);
        privatElement.Add(priexponent);
        privatElement.Add(prip);
        privatElement.Add(priq);
        privatElement.Add(pridp);
        privatElement.Add(pridq);
        privatElement.Add(priinverseQ);
        privatElement.Add(prid);

        var publicElement = new XElement("RSAKeyValue");
        //Modulus
        var pubmodulus = new XElement("Modulus", rsap.Modulus?.ToBase64());
        //Exponent
        var pubexponent = new XElement("Exponent", rsap.Exponent?.ToBase64());

        publicElement.Add(pubmodulus);
        publicElement.Add(pubexponent);

        return (privatElement.ToString(), publicElement.ToString());
    }

    /// <summary>
    /// Pkcs1私钥转Pkcs8
    /// </summary>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public static string PrivateKeyPkcs1ToPkcs8(string privateKey, bool usePem = false)
    {
        privateKey = EnablePrivateKeyPkcs1Pem(privateKey);
        var pr = new PemReader(new StringReader(privateKey));
        var kp = pr.ReadObject() as AsymmetricCipherKeyPair;
        using var sw = new StringWriter();
        var pWrt = new PemWriter(sw);
        var pkcs8 = new Pkcs8Generator(kp?.Private);
        pWrt.WriteObject(pkcs8);
        pWrt.Writer.Close();
        return usePem ? sw.ToString() : DisablePrivateKeyPkcs8Pem(sw.ToString());
    }

    /// <summary>
    /// Pkcs1私钥转XML
    /// </summary>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public static string PrivateKeyPkcs1ToXml(string privateKey)
    {
        privateKey = EnablePrivateKeyPkcs1Pem(privateKey);
        var pr = new PemReader(new StringReader(privateKey));
        if (!(pr.ReadObject() is AsymmetricCipherKeyPair asymmetricCipherKeyPair))
            throw new ExceptionBase(ExceptionType.Parameter, 0, "私钥格式错误");
        var rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(
            PrivateKeyInfoFactory.CreatePrivateKeyInfo(asymmetricCipherKeyPair.Private)
        );
        var privatElement = new XElement("RSAKeyValue");
        //Modulus
        var primodulus = new XElement("Modulus", rsaPrivateCrtKeyParameters.Modulus.ToByteArrayUnsigned().ToBase64());
        //Exponent
        var priexponent = new XElement("Exponent", rsaPrivateCrtKeyParameters.PublicExponent.ToByteArrayUnsigned().ToBase64());
        //P
        var prip = new XElement("P", rsaPrivateCrtKeyParameters.P.ToByteArrayUnsigned().ToBase64());
        //Q
        var priq = new XElement("Q", rsaPrivateCrtKeyParameters.Q.ToByteArrayUnsigned().ToBase64());
        //DP
        var pridp = new XElement("DP", rsaPrivateCrtKeyParameters.DP.ToByteArrayUnsigned().ToBase64());
        //DQ
        var pridq = new XElement("DQ", rsaPrivateCrtKeyParameters.DQ.ToByteArrayUnsigned().ToBase64());
        //InverseQ
        var priinverseQ = new XElement("InverseQ", rsaPrivateCrtKeyParameters.QInv.ToByteArrayUnsigned().ToBase64());
        //D
        var prid = new XElement("D", rsaPrivateCrtKeyParameters.Exponent.ToByteArrayUnsigned().ToBase64());
        privatElement.Add(primodulus);
        privatElement.Add(priexponent);
        privatElement.Add(prip);
        privatElement.Add(priq);
        privatElement.Add(pridp);
        privatElement.Add(pridq);
        privatElement.Add(priinverseQ);
        privatElement.Add(prid);
        return privatElement.ToString();
    }

    /// <summary>
    /// Pkcs8私钥转Pkcs1
    /// </summary>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    private static string PrivateKeyPkcs8ToPkcs1(string privateKey, bool usePem = false)
    {
        privateKey = EnablePrivateKeyPkcs8Pem(privateKey);
        var pr = new PemReader(new StringReader(privateKey));
        var kp = pr.ReadObject() as RsaPrivateCrtKeyParameters;
        var keyParameter = PrivateKeyFactory.CreateKey(PrivateKeyInfoFactory.CreatePrivateKeyInfo(kp));
        using var sw = new StringWriter();
        var pWrt = new PemWriter(sw);
        pWrt.WriteObject(keyParameter);
        pWrt.Writer.Close();
        return usePem ? sw.ToString() : DisablePrivateKeyPkcs1Pem(sw.ToString());
    }

    /// <summary>
    /// Pkcs8私钥转XML
    /// </summary>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    private static string PrivateKeyPkcs8ToXml(string privateKey)
    {
        privateKey = DisablePrivateKeyPkcs8Pem(privateKey);
        var privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(privateKey.FromBase64());
        var privatElement = new XElement("RSAKeyValue");
        //Modulus
        var primodulus = new XElement("Modulus", privateKeyParam.Modulus.ToByteArrayUnsigned().ToBase64());
        //Exponent
        var priexponent = new XElement("Exponent", privateKeyParam.PublicExponent.ToByteArrayUnsigned().ToBase64());
        //P
        var prip = new XElement("P", privateKeyParam.P.ToByteArrayUnsigned().ToBase64());
        //Q
        var priq = new XElement("Q", privateKeyParam.Q.ToByteArrayUnsigned().ToBase64());
        //DP
        var pridp = new XElement("DP", privateKeyParam.DP.ToByteArrayUnsigned().ToBase64());
        //DQ
        var pridq = new XElement("DQ", privateKeyParam.DQ.ToByteArrayUnsigned().ToBase64());
        //InverseQ
        var priinverseQ = new XElement("InverseQ", privateKeyParam.QInv.ToByteArrayUnsigned().ToBase64());
        //D
        var prid = new XElement("D", privateKeyParam.Exponent.ToByteArrayUnsigned().ToBase64());
        privatElement.Add(primodulus);
        privatElement.Add(priexponent);
        privatElement.Add(prip);
        privatElement.Add(priq);
        privatElement.Add(pridp);
        privatElement.Add(pridq);
        privatElement.Add(priinverseQ);
        privatElement.Add(prid);
        return privatElement.ToString();
    }

    /// <summary>
    /// XML私钥转Pkcs1
    /// </summary>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    private static string PrivateKeyXmlToPkcs1(string privateKey, bool usePem = false)
    {
        var root = XElement.Parse(privateKey);
        //Modulus
        var modulus = root.Element("Modulus");
        //Exponent
        var exponent = root.Element("Exponent");
        //P
        var p = root.Element("P");
        //Q
        var q = root.Element("Q");
        //DP
        var dp = root.Element("DP");
        //DQ
        var dq = root.Element("DQ");
        //InverseQ
        var inverseQ = root.Element("InverseQ");
        //D
        var d = root.Element("D");
        var rsaPrivateCrtKeyParameters = new RsaPrivateCrtKeyParameters(
            new BigInteger(1, modulus?.Value.FromBase64()),
            new BigInteger(1, exponent?.Value.FromBase64()),
            new BigInteger(1, d?.Value.FromBase64()),
            new BigInteger(1, p?.Value.FromBase64()),
            new BigInteger(1, q?.Value.FromBase64()),
            new BigInteger(1, dp?.Value.FromBase64()),
            new BigInteger(1, dq?.Value.FromBase64()),
            new BigInteger(1, inverseQ?.Value.FromBase64())
        );
        using var sw = new StringWriter();
        var pWrt = new PemWriter(sw);
        pWrt.WriteObject(rsaPrivateCrtKeyParameters);
        pWrt.Writer.Close();
        return usePem ? sw.ToString() : DisablePrivateKeyPkcs1Pem(sw.ToString());
    }

    /// <summary>
    /// XML私钥转Pkcs8
    /// </summary>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    private static string PrivateKeyXmlToPkcs8(string privateKey, bool usePem = false)
    {
        var root = XElement.Parse(privateKey);
        //Modulus
        var modulus = root.Element("Modulus");
        //Exponent
        var exponent = root.Element("Exponent");
        //P
        var p = root.Element("P");
        //Q
        var q = root.Element("Q");
        //DP
        var dp = root.Element("DP");
        //DQ
        var dq = root.Element("DQ");
        //InverseQ
        var inverseQ = root.Element("InverseQ");
        //D
        var d = root.Element("D");
        var rsaPrivateCrtKeyParameters = new RsaPrivateCrtKeyParameters(
            new BigInteger(1, modulus?.Value.FromBase64()),
            new BigInteger(1, exponent?.Value.FromBase64()),
            new BigInteger(1, d?.Value.FromBase64()),
            new BigInteger(1, p?.Value.FromBase64()),
            new BigInteger(1, q?.Value.FromBase64()),
            new BigInteger(1, dp?.Value.FromBase64()),
            new BigInteger(1, dq?.Value.FromBase64()),
            new BigInteger(1, inverseQ?.Value.FromBase64())
        );
        using var swpri = new StringWriter();
        var pWrtpri = new PemWriter(swpri);
        var pkcs8 = new Pkcs8Generator(rsaPrivateCrtKeyParameters);
        pWrtpri.WriteObject(pkcs8);
        pWrtpri.Writer.Close();
        return usePem ? swpri.ToString() : DisablePrivateKeyPkcs8Pem(swpri.ToString());
    }

    /// <summary>
    /// Pem公钥转XML
    /// </summary>
    /// <param name="publicKey"></param>
    /// <returns></returns>
    public static string PublicKeyPemToXml(string publicKey)
    {
        publicKey = EnablePublicKeyPem(publicKey);
        var pr = new PemReader(new StringReader(publicKey));
        var obj = pr.ReadObject();
        if (!(obj is RsaKeyParameters rsaKey)) throw new ExceptionBase(ExceptionType.Internal, 0, "无法读取公钥信息");
        var publicElement = new XElement("RSAKeyValue");
        //Modulus
        var pubmodulus = new XElement("Modulus", rsaKey.Modulus.ToByteArrayUnsigned().ToBase64());
        //Exponent
        var pubexponent = new XElement("Exponent", rsaKey.Exponent.ToByteArrayUnsigned().ToBase64());
        publicElement.Add(pubmodulus);
        publicElement.Add(pubexponent);
        return publicElement.ToString();
    }

    /// <summary>
    /// XML公钥转Pem
    /// </summary>
    /// <param name="publicKey"></param>
    /// <param name="usePem"></param>
    /// <returns></returns>
    public static string PublicKeyXmlToPem(string publicKey, bool usePem = false)
    {
        var root = XElement.Parse(publicKey);
        //Modulus
        var modulus = root.Element("Modulus");
        //Exponent
        var exponent = root.Element("Exponent");
        var rsaKeyParameters = new RsaKeyParameters(false, new BigInteger(1, modulus?.Value.FromBase64()), new BigInteger(1, exponent?.Value.FromBase64()));
        using var sw = new StringWriter();
        var pWrt = new PemWriter(sw);
        pWrt.WriteObject(rsaKeyParameters);
        pWrt.Writer.Close();
        return usePem ? sw.ToString() : DisablePublicKeyPem(sw.ToString());
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="data"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    private static byte[] Encrypt(System.Security.Cryptography.RSA rsa, byte[] data, RSAPadding padding)
    {
        var bufferSize = (rsa.KeySize / 8) - 11; // 单块最大长度
        var buffer = new byte[bufferSize];
        using var inputStream = new MemoryStream(data);
        using var outputStream = new MemoryStream();
        while (true) // 分段加密
        {
            int readSize = inputStream.Read(buffer, 0, bufferSize);
            if (readSize <= 0) break;
            var temp = new byte[readSize];
            Array.Copy(buffer, 0, temp, 0, readSize);
            var encryptedBytes = rsa.Encrypt(temp, PaddingMapper[padding]);
            outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
        }
        return outputStream.ToArray();
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="rsa"></param>
    /// <param name="data"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    private static byte[] Decrypt(System.Security.Cryptography.RSA rsa, byte[] data, RSAPadding padding)
    {
        int bufferSize = rsa.KeySize / 8;
        var buffer = new byte[bufferSize];
        using var inputStream = new MemoryStream(data);
        using var outputStream = new MemoryStream();
        while (true)
        {
            int readSize = inputStream.Read(buffer, 0, bufferSize);
            if (readSize <= 0) break;
            var temp = new byte[readSize];
            Array.Copy(buffer, 0, temp, 0, readSize);
            var rawBytes = rsa.Decrypt(temp, PaddingMapper[padding]);
            outputStream.Write(rawBytes, 0, rawBytes.Length);
        }
        return outputStream.ToArray();
    }

    /// <summary>
    /// 公钥加密
    /// </summary>
    /// <param name="format"></param>
    /// <param name="publicKey"></param>
    /// <param name="data"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    public static byte[] EncryptWithPublicKey(RSAFormat format, string publicKey, byte[] data, RSAPadding padding)
    {
        using var rsa = System.Security.Cryptography.RSA.Create();
        switch (format)
        {
            case RSAFormat.Pkcs1:
            case RSAFormat.Pkcs8:
                rsa.ImportSubjectPublicKeyInfo(DisablePublicKeyPem(publicKey).FromBase64(), out _);
                break;
            case RSAFormat.Xml:
                var rsap = new RSAParameters();
                var root = XElement.Parse(publicKey);
                //Modulus
                var modulus = root.Element("Modulus");
                //Exponent
                var exponent = root.Element("Exponent");
                rsap.Modulus = modulus?.Value.FromBase64();
                rsap.Exponent = exponent?.Value.FromBase64();
                rsa.ImportParameters(rsap);
                break;
        }
        return Encrypt(rsa, data, padding);
    }

    /// <summary>
    /// 私钥加密
    /// </summary>
    /// <param name="format"></param>
    /// <param name="privateKey"></param>
    /// <param name="data"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    public static byte[] EncryptWithPrivateKey(RSAFormat format, string privateKey, byte[] data, RSAPadding padding)
    {
        using var rsa = System.Security.Cryptography.RSA.Create();
        switch (format)
        {
            case RSAFormat.Pkcs1:
                rsa.ImportRSAPrivateKey(DisablePrivateKeyPkcs1Pem(privateKey).FromBase64(), out _);
                break;
            case RSAFormat.Pkcs8:
                rsa.ImportPkcs8PrivateKey(DisablePrivateKeyPkcs8Pem(privateKey).FromBase64(), out _);
                break;
            case RSAFormat.Xml:
                var rsap = new RSAParameters();
                var root = XElement.Parse(privateKey);
                //Modulus
                var modulus = root.Element("Modulus");
                //Exponent
                var exponent = root.Element("Exponent");
                //P
                var p = root.Element("P");
                //Q
                var q = root.Element("Q");
                //DP
                var dp = root.Element("DP");
                //DQ
                var dq = root.Element("DQ");
                //InverseQ
                var inverseQ = root.Element("InverseQ");
                //D
                var d = root.Element("D");
                rsap.Modulus = modulus?.Value.FromBase64();
                rsap.Exponent = exponent?.Value.FromBase64();
                rsap.P = p?.Value.FromBase64();
                rsap.Q = q?.Value.FromBase64();
                rsap.DP = dp?.Value.FromBase64();
                rsap.DQ = dq?.Value.FromBase64();
                rsap.InverseQ = inverseQ?.Value.FromBase64();
                rsap.D = d?.Value.FromBase64();
                rsa.ImportParameters(rsap);
                break;
        }
        return Encrypt(rsa, data, padding);
    }

    /// <summary>
    /// 私钥解密
    /// </summary>
    /// <param name="data"></param>
    /// <param name="type"></param>
    /// <param name="privateKey"></param>
    /// <param name="isPem"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    public static byte[] Decrypt(RSAFormat format, string privateKey, byte[] data, RSAPadding padding)
    {
        using var rsa = System.Security.Cryptography.RSA.Create();
        switch (format)
        {
            case RSAFormat.Pkcs1:
                rsa.ImportRSAPrivateKey(DisablePrivateKeyPkcs1Pem(privateKey).FromBase64(), out _);
                break;
            case RSAFormat.Pkcs8:
                rsa.ImportPkcs8PrivateKey(DisablePrivateKeyPkcs8Pem(privateKey).FromBase64(), out _);
                break;
            case RSAFormat.Xml:
                var rsap = new RSAParameters();
                var root = XElement.Parse(privateKey);
                //Modulus
                var modulus = root.Element("Modulus");
                //Exponent
                var exponent = root.Element("Exponent");
                //P
                var p = root.Element("P");
                //Q
                var q = root.Element("Q");
                //DP
                var dp = root.Element("DP");
                //DQ
                var dq = root.Element("DQ");
                //InverseQ
                var inverseQ = root.Element("InverseQ");
                //D
                var d = root.Element("D");
                rsap.Modulus = modulus?.Value.FromBase64();
                rsap.Exponent = exponent?.Value.FromBase64();
                rsap.P = p?.Value.FromBase64();
                rsap.Q = q?.Value.FromBase64();
                rsap.DP = dp?.Value.FromBase64();
                rsap.DQ = dq?.Value.FromBase64();
                rsap.InverseQ = inverseQ?.Value.FromBase64();
                rsap.D = d?.Value.FromBase64();
                rsa.ImportParameters(rsap);
                break;
        }
        return Decrypt(rsa, data, padding);
    }
}