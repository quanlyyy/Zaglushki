using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    [System.Serializable]
    private class LoginRequest
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    private class UserInfo
    {
        public string id;
        public string email;
    }

    [System.Serializable]
    private class SessionInfo
    {
        public string access_token;
        public string refresh_token;
        public UserInfo user;
    }

    [System.Serializable]
    private class ProfileDto
    {
        public string id;
        public string email;
        public string full_name;
        public string role;
    }

    public void Login()
    {
        StartCoroutine(LoginRoutine());
    }

    private IEnumerator LoginRoutine()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        //валидация
        if (string.IsNullOrWhiteSpace(email))
        {
            statusText.text = "Введите email";
            yield break;
        }

        if (!InputValidator.IsValidEmail(email))
        {
            statusText.text = "Некорректный email";
            yield break;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            statusText.text = "Введите пароль";
            yield break;
        }

        if (!InputValidator.IsValidPassword(password))
        {
            statusText.text = "Пароль должен быть не менее 6 символов";
            yield break;
        }

        statusText.text = "Вход...";

        var body = new LoginRequest
        {
            email = email,
            password = password
        };

        string json = JsonUtility.ToJson(body);

        using var req = new UnityWebRequest(SupabaseConfig.AuthUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("apikey", SupabaseConfig.AnonKey);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            statusText.text = "Неверный логин или пароль";
            Debug.LogError(req.downloadHandler.text);
            yield break;
        }

        var session = JsonUtility.FromJson<SessionInfo>(req.downloadHandler.text);

        if (session == null || session.user == null || string.IsNullOrWhiteSpace(session.access_token))
        {
            statusText.text = "Некорректный ответ сервера";
            yield break;
        }

        UserSession.AccessToken = session.access_token;
        UserSession.RefreshToken = session.refresh_token;
        UserSession.UserId = session.user.id;
        UserSession.Email = session.user.email;

        bool loaded = false;
        yield return StartCoroutine(LoadProfile(result => loaded = result));

        if (!loaded)
        {
            statusText.text = "Ошибка загрузки профиля";
            yield break;
        }

        SceneManager.LoadScene("Menu");
    }

    private IEnumerator LoadProfile(System.Action<bool> onDone)
    {
        string url = $"{SupabaseConfig.ProfilesUrl}?id=eq.{UserSession.UserId}&select=id,email,full_name,role";

        using var req = UnityWebRequest.Get(url);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
        req.SetRequestHeader("Authorization", $"Bearer {UserSession.AccessToken}");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Ошибка загрузки профиля: " + req.downloadHandler.text);
            onDone?.Invoke(false);
            yield break;
        }

        string json = req.downloadHandler.text;
        if (json.StartsWith("[") && json.Length > 2)
        {
            string obj = json.Substring(1, json.Length - 2);
            var profile = JsonUtility.FromJson<ProfileDto>(obj);

            if (profile == null)
            {
                onDone?.Invoke(false);
                yield break;
            }

            UserSession.FullName = profile.full_name;
            UserSession.Role = profile.role;

            onDone?.Invoke(true);
            yield break;
        }

        onDone?.Invoke(false);
    }
}