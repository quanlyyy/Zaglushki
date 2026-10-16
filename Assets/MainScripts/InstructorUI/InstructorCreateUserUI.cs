using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class InstructorCreateUserUI : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField fullNameInput;
    public TMP_Text statusText;

    [System.Serializable]
    private class CreateUserRequest
    {
        public string email;
        public string full_name;
    }

    public void CreateUser()
    {
        StartCoroutine(CreateUserRoutine());
    }

    private IEnumerator CreateUserRoutine()
    {
        string token = UserSession.AccessToken?.Trim();

        if (string.IsNullOrWhiteSpace(token))
        {
            statusText.text = "Нет access token. Войдите заново.";
            yield break;
        }

        if (token.StartsWith("Bearer "))
            token = token.Substring("Bearer ".Length).Trim();

        string email = emailInput.text.Trim();
        string fullName = fullNameInput.text.Trim();

        //валидация
        if (string.IsNullOrWhiteSpace(fullName))
        {
            statusText.text = "Введите ФИО";
            yield break;
        }

        if (!InputValidator.IsValidFullName(fullName))
        {
            statusText.text = "ФИО должно содержать не менее 5 символов";
            yield break;
        }

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

        var body = new CreateUserRequest
        {
            email = email,
            full_name = fullName
        };

        statusText.text = "Создание пользователя...";

        string json = JsonUtility.ToJson(body);

        using var req = new UnityWebRequest(SupabaseConfig.CreateUserFunctionUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
        req.SetRequestHeader("Authorization", $"Bearer {token}");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            statusText.text = "Ошибка создания пользователя. Пользователь уже создан";
            Debug.LogError(req.downloadHandler.text);
            yield break;
        }

        statusText.text = "Пользователь создан, письмо отправлено";

        emailInput.text = "";
        fullNameInput.text = "";
    }
}