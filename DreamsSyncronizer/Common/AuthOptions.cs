using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DreamsSyncronizer.Common;

public class AuthOptions
{
    public const string AUDIENCE = "Audience";
    public const string ISSUER = "Issuer";
    private const string KEY = "SecretKeyHere";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}