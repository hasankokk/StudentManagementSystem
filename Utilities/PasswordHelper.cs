using System.Security.Cryptography;

namespace StudentManagementSystem.Helpers;

public class PasswordHelper
{
    public static string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[16];
        rng.GetBytes(salt); // Rastgele 16Bytelik bir veri üretiliyor.

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32); // 256-bit

        string saltBase64 = Convert.ToBase64String(salt);
        string hashBase64 = Convert.ToBase64String(hash);

        return $"{saltBase64}.{hashBase64}"; //Rastgele üretilen salt ile 13.satırda girdiğim şifremin hashi karıştırılarak daha güvenlikli bir hash oluşturuluyor.
    }

    //Listeden çektiğim ve input olarak verdiğim hashli veriler.
    //Salt kısmından arındırılarak sadece password hash kısmı ile karşılaştırma yapılıp giriş sağlıyor.
    public static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.'); 
        if (parts.Length != 2)
            return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] originalHash = Convert.FromBase64String(parts[1]);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);

        return hash.SequenceEqual(originalHash);
    }
    public static string GenerateTemporaryPassword(int length = 8)
    {
        const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?";
        var random = new Random();
        return new string(Enumerable.Repeat(validChars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}