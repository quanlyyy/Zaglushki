using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InstructorUsersUI : MonoBehaviour
{
    [System.Serializable]
    public class UserDto
    {
        public string user_id;
        public string email;
        public string full_name;
        public string role;
        public string created_at;
    }

    [System.Serializable]
    private class UserArrayWrapper
    {
        public List<UserDto> items;
    }

    [Header("UI")]
    public Transform contentRoot;
    public UserListItemUI itemPrefab;
    public TMP_InputField searchInput;
    public ConfirmDialogUI confirmDialog;

    private List<UserDto> loadedUsers = new List<UserDto>();
    private List<UserDto> filteredUsers = new List<UserDto>();
    private bool isLoaded = false;

    void Start()
    {
        if (contentRoot == null)
        {
            Debug.LogError("контент не назначен");
            return;
        }

        if (itemPrefab == null)
        {
            Debug.LogError("префаб не назначен");
            return;
        }

        if (confirmDialog == null)
        {
            Debug.LogError("диалог не назначен");
            return;
        }

        if (searchInput != null)
            searchInput.onValueChanged.AddListener(OnSearchChanged);

        StartCoroutine(LoadUsers());
    }

    public IEnumerator LoadUsers()
    {
        using var req = new UnityWebRequest(SupabaseConfig.RpcUsersUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes("{}"));
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
        req.SetRequestHeader("Authorization", $"Bearer {UserSession.AccessToken}");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("LoadUsers error: " + req.downloadHandler.text);
            yield break;
        }

        string wrapped = "{\"items\":" + req.downloadHandler.text + "}";
        var data = JsonUtility.FromJson<UserArrayWrapper>(wrapped);

        loadedUsers.Clear();

        if (data?.items != null)
            loadedUsers = data.items;

        Debug.Log("«агружено пользователей: " + loadedUsers.Count);

        isLoaded = true;
        ApplySearchAndRebuild();
    }
    
    //поиск
    public void OnSearchChanged(string value)
    {
        ApplySearchAndRebuild();
    }

    //обновление фильтров
    public void ResetFilters()
    {
        if (searchInput != null)
            searchInput.text = "";

        ApplySearchAndRebuild();
    }

    //созраниение посика и обновление списка
    private void ApplySearchAndRebuild()
    {
        if (!isLoaded)
            return;

        IEnumerable<UserDto> query = loadedUsers;

        if (searchInput != null && !string.IsNullOrWhiteSpace(searchInput.text))
        {
            string search = searchInput.text.Trim().ToLower();
            query = query.Where(x =>
                !string.IsNullOrWhiteSpace(x.full_name) &&
                x.full_name.ToLower().Contains(search));
        }

        filteredUsers = query
            .OrderByDescending(x => ParseDateSafe(x.created_at))
            .ToList();

        RebuildList();
    }

    private DateTimeOffset ParseDateSafe(string value)
    {
        if (DateTimeOffset.TryParse(value, out var dt))
            return dt;

        return DateTimeOffset.MinValue;
    }

    //обновление списка
    private void RebuildList()
    {
        if (contentRoot == null || itemPrefab == null)
            return;

        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        foreach (var user in filteredUsers)
        {
            var item = Instantiate(itemPrefab, contentRoot);
            item.Setup(this, user);
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot as RectTransform);
    }

    //проверка удалени€
    public void AskDeleteUser(UserDto user)
    {
        if (user == null)
            return;

        Debug.Log("AskDeleteUser: " + user.full_name);

        if (user.role == "instructor")
        {
            confirmDialog.ShowInfo("Ќельз€ удалить пользовател€ с ролью instructor.");
            return;
        }

        confirmDialog.ShowConfirm(
            $"¬ы точно хотите удалить пользовател€ {user.full_name}?",
            () => StartCoroutine(DeleteUserRoutine(user))
        );
    }

    //проверка обновлени€ парол€
    public void AskResetPassword(UserDto user)
    {
        if (user == null)
            return;

        Debug.Log("AskResetPassword: " + user.full_name);

        if (user.role == "instructor")
        {
            confirmDialog.ShowInfo("Ќельз€ сбросить пароль пользователю с ролью instructor.");
            return;
        }

        confirmDialog.ShowConfirm(
            $"¬ы точно хотите восстановить пароль пользователю {user.full_name}?",
            () => StartCoroutine(ResetPasswordRoutine(user))
        );
    }

    //удаление пользователей
    private IEnumerator DeleteUserRoutine(UserDto user)
    {
        Debug.Log("DeleteUserRoutine START: " + user.full_name);

        string json = "{\"user_id\":\"" + user.user_id + "\"}";

        using var req = new UnityWebRequest(SupabaseConfig.DeleteUserFunctionUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
        req.SetRequestHeader("Authorization", $"Bearer {UserSession.AccessToken}");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("DeleteUserRoutine error: " + req.downloadHandler.text);
            confirmDialog.ShowInfo("ќшибка удалени€ пользовател€.");
            yield break;
        }

        //удал€ем из локального списка сразу
        loadedUsers.RemoveAll(x => x.user_id == user.user_id);
        filteredUsers.RemoveAll(x => x.user_id == user.user_id);
        RebuildList();

        confirmDialog.ShowInfo("ѕользователь удалЄн.");
        Debug.Log("DeleteUserRoutine END");
    }

    //восстановление парол€
    private IEnumerator ResetPasswordRoutine(UserDto user)
    {
        Debug.Log("ResetPasswordRoutine START: " + user.full_name);

        string json = "{\"user_id\":\"" + user.user_id + "\"}";

        using var req = new UnityWebRequest(SupabaseConfig.ResetUserPasswordFunctionUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
        req.SetRequestHeader("Authorization", $"Bearer {UserSession.AccessToken}");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("ResetPasswordRoutine error: " + req.downloadHandler.text);
            confirmDialog.ShowInfo("ќшибка восстановлени€ парол€.");
            yield break;
        }

        confirmDialog.ShowInfo("Ќовый пароль отправлен пользователю на почту.");
        Debug.Log("ResetPasswordRoutine END");
    }
}