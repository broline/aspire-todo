using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Todo.Api.Tests.Security;
public class TestJwtToken
{
    public List<Claim> Claims { get; } = new();
    private int _expiresInMinutes = 30;

    public TestJwtToken WithRole(string roleName)
    {
        Claims.RemoveAll(x => x.Type == ClaimTypes.Role);

        Claims.Add(new Claim(ClaimTypes.Role, roleName));
        return this;
    }

    public TestJwtToken WithRoles(params string[] roles)
    {
        Claims.RemoveAll(x => x.Type == ClaimTypes.Role);

        Claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        return this;
    }

    public TestJwtToken WithUserName(string username)
    {
        Claims.RemoveAll(x => x.Type == ClaimTypes.Upn);

        Claims.Add(new Claim(ClaimTypes.Upn, username));
        return this;
    }

    public TestJwtToken WithEmail(string email)
    {
        Claims.RemoveAll(x => x.Type == ClaimTypes.Email);

        Claims.Add(new Claim(ClaimTypes.Email, email));
        return this;
    }

    public TestJwtToken WithExpiration(int expiresInMinutes)
    {
        _expiresInMinutes = expiresInMinutes;
        return this;
    }

    public string Build()
    {
        var token = new JwtSecurityToken(
            JwtTokenProvider.Issuer,
            JwtTokenProvider.Issuer,
            Claims,
            expires: DateTime.Now.AddMinutes(_expiresInMinutes),
            signingCredentials: JwtTokenProvider.SigningCredentials
        );
        return JwtTokenProvider.JwtSecurityTokenHandler.WriteToken(token);
    }
}