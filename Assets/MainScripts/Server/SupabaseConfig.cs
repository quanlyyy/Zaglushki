public static class SupabaseConfig
{
    public const string SupabaseUrl = "https://YOUR_PROJECT.supabase.co";
    public const string AnonKey = "YOUR_SUPABASE_ANON_KEY";

    public static string AuthUrl => $"{SupabaseUrl}/auth/v1/token?grant_type=password";
    public static string ProfilesUrl => $"{SupabaseUrl}/rest/v1/profiles";

    public static string RpcMyAttemptsUrl => $"{SupabaseUrl}/rest/v1/rpc/get_my_attempts";
    public static string RpcMyAttemptDetailsUrl => $"{SupabaseUrl}/rest/v1/rpc/get_my_attempt_valves";

    public static string ChangeMyPasswordFunctionUrl => $"{SupabaseUrl}/functions/v1/change-my-password";

    public static string CreateUserFunctionUrl => $"{SupabaseUrl}/functions/v1/create-user";
    public static string DeleteUserFunctionUrl => $"{SupabaseUrl}/functions/v1/delete-user";
    public static string ResetUserPasswordFunctionUrl => $"{SupabaseUrl}/functions/v1/reset-user-password";
    public static string RpcUsersUrl => $"{SupabaseUrl}/rest/v1/rpc/get_users_for_instructor";

    public static string RpcAttemptsUrl => $"{SupabaseUrl}/rest/v1/rpc/get_attempts_for_instructor";
    public static string RpcAttemptDetailsUrl => $"{SupabaseUrl}/rest/v1/rpc/get_attempt_valves_for_instructor";

    public static string SaveAttemptsUrl => $"{SupabaseUrl}/rest/v1/exam_attempts";
    public static string SaveAttemptValvesUrl => $"{SupabaseUrl}/rest/v1/exam_attempt_valves";
    public static string RpcAnalyticsSummaryUrl => $"{SupabaseUrl}/rest/v1/rpc/get_analytics_summary";
    public static string RpcHardestValvesUrl => $"{SupabaseUrl}/rest/v1/rpc/get_hardest_valves";
    public static string RpcTopUsersUrl => $"{SupabaseUrl}/rest/v1/rpc/get_top_users";
}
