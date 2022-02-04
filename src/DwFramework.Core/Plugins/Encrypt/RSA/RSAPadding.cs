namespace DwFramework.Core.Encrypt;

public enum RSAPadding
{
    Pkcs1 = 11,
    OaepSHA1 = 42,
    OaepSHA256 = 66,
    OaepSHA384 = 98,
    OaepSHA512 = 130
}