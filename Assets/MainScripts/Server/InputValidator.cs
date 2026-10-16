using System.Text.RegularExpressions;

public static class InputValidator
{
    private static readonly Regex EmailRegex =
        new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        email = email.Trim();
        return EmailRegex.IsMatch(email);
    }

    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        return password.Length >= 6;
    }

    public static bool IsValidFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return false;

        fullName = fullName.Trim();

        if (fullName.Length < 5)
            return false;

        return true;
    }
}