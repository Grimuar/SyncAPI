using System.Security.Cryptography;
using System.Text;

namespace DreamsSyncronizer.Common;
/// <summary>
/// Обработчик для хеширования строки, проверить возможность применения
/// к моделям в БД
/// </summary>
public static class MD5Handler
{
    public static string GetHash(string input)
    {
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
 
        return Convert.ToBase64String(hash);
    }
    
}