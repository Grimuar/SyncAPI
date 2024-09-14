using System.Security.Cryptography;
using System.Text;

namespace DreamsSyncronizer.Common;

public static class PasswordHandler
{
    public static string ComputeSha256(string password)
    {
        //Compute hash of the taken string
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(password));

        //convert byte to string format
        return Convert.ToHexString(hash);
    }
}