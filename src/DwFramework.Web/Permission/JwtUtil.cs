using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DwFramework.Web.Permission;

public static class JwtUtil
{
    /// <summary>
    /// 生成Token
    /// </summary>
    /// <param name="securityKey"></param>
    /// <param name="issuer"></param>
    /// <param name="audience"></param>
    /// <param name="claims"></param>
    /// <param name="notBefore"></param>
    /// <param name="expires"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    public static string Generate(
        SecurityKey securityKey,
        string issuer = null,
        string audience = null,
        IEnumerable<Claim> claims = null,
        DateTime? notBefore = null,
        DateTime? expires = null,
        Dictionary<string, object> properties = null
    )
    {
        JwtSecurityTokenHandler tokenHandler = new();
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            notBefore,
            expires,
            signingCredentials
        );
        // 扩展字段
        if (properties is not null) foreach (var property in properties)
                jwtSecurityToken.Payload[property.Key] = property.Value;
        var token = tokenHandler.WriteToken(jwtSecurityToken);
        return token;
    }

    /// <summary>
    /// 解析Token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static JwtSecurityToken Decode(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        if (!tokenHandler.CanReadToken(token)) throw new Exception("invaild token");
        return tokenHandler.ReadJwtToken(token);
    }

    /// <summary>
    /// 读取Claims信息
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static JwtPayload ReadClaims(string token)
    {
        return Decode(token).Payload;
    }

    /// <summary>
    /// 读取Claims信息
    /// </summary>
    /// <param name="token"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static object ReadClaim(string token, string key)
    {
        return Decode(token).Payload[key];
    }
}