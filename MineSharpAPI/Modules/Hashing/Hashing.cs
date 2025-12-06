using System.Security.Cryptography;

namespace MineSharpAPI.Modules.Hashing;

public class HashingUtils
{
    //Valori per l'hash
    private const int salt_size = 16;
    private const int hash_size = 32;

    private const int iterations = 100000;

    //algorimto con chiave simmetrica
    private static readonly HashAlgorithmName algorithm = HashAlgorithmName.SHA512;


    public static string HashString(string toHash)
    {
        var salt = RandomNumberGenerator.GetBytes(salt_size);
        var hash = Rfc2898DeriveBytes.Pbkdf2(toHash, salt, iterations, algorithm, hash_size);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }
}