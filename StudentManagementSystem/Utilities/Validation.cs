namespace StudentManagementSystem.Helpers;

public static class Validation
{
    public static bool IsValidTckn(long tckn)
    {
        return tckn >= 10000000000 && tckn <= 99999999999;
    }
    //public static bool IsValidPassword(string password, out string errorpass)
    //{
    //    errorpass = "";
    //    if (password.Length < 8 || password.Length > 32)
    //    {
    //        errorpass = "Şifreniz 8 ile 32 karakter arasında olmalıdır!";
    //        return false;
    //    }

    //    bool passUpper = false, passLower = false, passDigit = false, passSpecial = false, passWhiteSpace = false;
    //    string specialChar = "!@_-+?=&*#$'\"";
    //    foreach (var c in password)
    //    {
    //        if (char.IsUpper(c)) passUpper = true;
    //        else if (char.IsLower(c)) passLower = true;
    //        else if (char.IsDigit(c)) passDigit = true;
    //        else if (specialChar.Contains(c.ToString())) passSpecial = true;
    //        if (char.IsWhiteSpace(c)) passWhiteSpace = true;

    //    }
    //    if (!passUpper)
    //    {
    //        errorpass = "Şifreniz en az 1 büyük harf (A-Z) içermelidir.";
    //        return false;
    //    }

    //    if (!passLower)
    //    {
    //        errorpass = "Şifreniz en az 1 küçük harf (a-z) içermelidir.";
    //        return false;
    //    }

    //    if (!passDigit)
    //    {
    //        errorpass = "Şifreniz en az 1 rakam (0-9)içermelidir.";
    //        return false;
    //    }

    //    if (!passSpecial)
    //    {
    //        errorpass = "Şifreniz en az 1 özel karakter (!@_-+?=&*#$'\") içermelidir.";
    //        return false;
    //    }

    //    if (passWhiteSpace)
    //    {
    //        errorpass = "Şifreniz boşluk içeremez!";
    //        return false;
    //    }
    //    return true;
    //}
}