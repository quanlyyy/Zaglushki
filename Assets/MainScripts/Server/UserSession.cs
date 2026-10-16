public static class UserSession
{
    public static string AccessToken;
    public static string RefreshToken;
    public static string UserId;
    public static string Email;
    public static string FullName;
    public static string Role;

    public static bool IsAuthorized => !string.IsNullOrEmpty(UserId);
    public static bool IsInstructor => Role == "instructor";

    public static void Clear()
    {
        AccessToken = null;
        RefreshToken = null;
        UserId = null;
        Email = null;
        FullName = null;
        Role = null;
    }
}