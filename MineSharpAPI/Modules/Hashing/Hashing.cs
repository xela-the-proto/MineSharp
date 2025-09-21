using System.Security.Cryptography;
using MineSharpAPI.Api;

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
        byte[] salt = RandomNumberGenerator.GetBytes(salt_size);
        byte[] hash  = Rfc2898DeriveBytes.Pbkdf2(toHash, salt, iterations, algorithm, hash_size);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }
    

    public static bool VerifyHash(string password, string hashedPass)
    {
  
  
        string[] parts = hashedPass.Split("-");
        byte[] salt = Convert.FromHexString(parts[1]);
        byte[] hash = Convert.FromHexString(parts[0]); 
        
        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, algorithm, hash_size);
        
        return CryptographicOperations.FixedTimeEquals(hash, inputHash);
    }
}