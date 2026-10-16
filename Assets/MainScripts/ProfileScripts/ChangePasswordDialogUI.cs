using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ChangePasswordDialogUI : MonoBehaviour
{
    [Header("Root")]
    public GameObject root;

    [Header("Inputs")]
    public TMP_InputField currentPasswordInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField confirmPasswordInput;

    [Header("Status")]
    public TMP_Text statusText;

    [System.Serializable]
    private class ChangePasswordRequest
    {
        public string current_password;
        public string new_password;
    }

    void Start()
    {
        if (root != null)
            root.SetActive(false);
    }

    public void Open()
    {
        if (root != null)
            root.SetActive(true);

        ClearFields();
    }

    public void Close()
    {
        if (root != null)
            root.SetActive(false);

        ClearFields();
    }

    public void ChangePassword()
    {
        StartCoroutine(ChangePasswordRoutine());
    }

    private IEnumerator ChangePasswordRoutine()
    {
        string currentPassword = currentPasswordInput != null ? currentPasswordInput.text.Trim() : "";
        string newPassword = newPasswordInput != null ? newPasswordInput.text.Trim() : "";
        string confirmPassword = confirmPasswordInput != null ? confirmPasswordInput.text.Trim() : "";

        if (!ValidateInputs(currentPassword, newPassword, confirmPassword))
            yield break;

        if (string.IsNullOrWhiteSpace(UserSession.AccessToken))
        {
            SetStatus("Ошибка авторизации. Войдите заново.");
            yield break;
        }

        SetStatus("Смена пароля...");

        var body = new ChangePasswordRequest
        {
            current_password = currentPassword,
            new_password = newPassword
        };

        string json = JsonUtility.ToJson(body);

        using var req = new UnityWebRequest(SupabaseConfig.ChangeMyPasswordFunctionUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
        req.SetRequestHeader("Authorization", $"Bearer {UserSession.AccessToken}");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(req.downloadHandler.text);
            SetStatus("Ошибка смены пароля. Проверьте текущий пароль.");
            yield break;
        }

        SetStatus("Пароль успешно изменён");

        yield return new WaitForSeconds(1.2f);

        Close();
    }
    
    //валидация
    private bool ValidateInputs(string currentPassword, string newPassword, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(currentPassword))
        {
            SetStatus("Введите текущий пароль");
            return false;
        }

        if (string.IsNullOrWhiteSpace(newPassword))
        {
            SetStatus("Введите новый пароль");
            return false;
        }

        if (newPassword.Length < 6)
        {
            SetStatus("Новый пароль должен быть не менее 6 символов");
            return false;
        }

        if (newPassword == currentPassword)
        {
            SetStatus("Новый пароль не должен совпадать с текущим");
            return false;
        }

        if (string.IsNullOrWhiteSpace(confirmPassword))
        {
            SetStatus("Подтвердите новый пароль");
            return false;
        }

        if (newPassword != confirmPassword)
        {
            SetStatus("Пароли не совпадают");
            return false;
        }

        return true;
    }

    //очищение полей ввода
    private void ClearFields()
    {
        if (currentPasswordInput != null)
            currentPasswordInput.text = "";

        if (newPasswordInput != null)
            newPasswordInput.text = "";

        if (confirmPasswordInput != null)
            confirmPasswordInput.text = "";

        SetStatus("");
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }
}