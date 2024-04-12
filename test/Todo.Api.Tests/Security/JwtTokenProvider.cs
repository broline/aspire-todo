using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Todo.Api.Tests.Security;
public static class JwtTokenProvider
{
    public static string Issuer { get; } = Common.Constants.AppNames.ApiTests;
    public static SecurityKey SecurityKey { get; } =
        new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes("Not_a_secret_but_needs_to_be_long")
        );
    public static SigningCredentials SigningCredentials { get; } =
        new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    internal static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new();
}
