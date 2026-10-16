using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfileSceneUI : MonoBehaviour
{
    [System.Serializable]
    public class AttemptDto
    {
        public string attempt_id;
        public string finished_at;
        public string full_name;
        public int score;
        public int max_score;
        public int time_spent_seconds;
    }

    [System.Serializable]
    public class ValveDto
    {
        public int valve_number;
        public bool is_correct;
    }

    [System.Serializable]
    private class AttemptArrayWrapper
    {
        public List<AttemptDto> items;
    }

    [System.Serializable]
    private class ValveArrayWrapper
    {
        public List<ValveDto> items;
    }

    [Header("Profile UI")]
    public TMP_Text userNameText;

    [Header("Attempts UI")]
    public Transform contentRoot;
    public AttemptListItemUI itemPrefab;

    [Header("Change Password Dialog")]
    public ChangePasswordDialogUI changePasswordDialog;

    void Start()
    {
        if (userNameText != null)
            userNameText.text = string.IsNullOrWhiteSpace(UserSession.FullName)
                ? UserSession.Email
                : UserSession.FullName;

        StartCoroutine(LoadMyAttempts());
    }

    // Загрузка списка попыток пользователя
    public IEnumerator LoadMyAttempts()
    {
        //создаем POST запрос
        using var req = new UnityWebRequest(SupabaseConfig.RpcMyAttemptsUrl, "POST");

        //устанавливаем тело запроса
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes("{}"));

        //обработчик ответа
        req.downloadHandler = new DownloadHandlerBuffer();

        //устанавливаем заголовки
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
        req.SetRequestHeader("Authorization", $"Bearer {UserSession.AccessToken}");

        // отправляем запрос
        yield return req.SendWebRequest();

        // проверяем ошибки
        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(req.downloadHandler.text);
            yield break;
        }

        // оборачиваем JSON в объект для JsonUtility
        string wrappedJson = "{\"items\":" + req.downloadHandler.text + "}";

        // преобразуем JSON в объект
        var data = JsonUtility.FromJson<AttemptArrayWrapper>(wrappedJson);

        // удаляем старые элементы списка
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        // проверяем наличие данных
        if (data?.items == null)
            yield break;

        foreach (var attempt in data.items)
        {
            var item = Instantiate(itemPrefab, contentRoot);

            item.Setup(
                null,
                attempt.attempt_id,
                attempt.finished_at,
                attempt.full_name,
                attempt.score,
                attempt.max_score,
                attempt.time_spent_seconds
            );

            item.SetProfileOwner(this);
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot as RectTransform);
    }
    public void LoadDetails(AttemptListItemUI item, string attemptId)
    {
        StartCoroutine(LoadDetailsRoutine(item, attemptId));
    }

    // Coroutine загрузки деталей попытки
    private IEnumerator LoadDetailsRoutine(AttemptListItemUI item, string attemptId)
    {
        // Формируем JSON тело запроса
        string body = "{\"p_attempt_id\":\"" + attemptId + "\"}";

        // Создаем POST запрос
        using var req = new UnityWebRequest(SupabaseConfig.RpcMyAttemptDetailsUrl, "POST");

        // Передаем данные
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));

        // Обработчик ответа
        req.downloadHandler = new DownloadHandlerBuffer();

        // Заголовки запроса
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
        req.SetRequestHeader("Authorization", $"Bearer {UserSession.AccessToken}");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(req.downloadHandler.text);
            yield break;
        }

        string wrapped = "{\"items\":" + req.downloadHandler.text + "}";

        var data = JsonUtility.FromJson<ValveArrayWrapper>(wrapped);

        var sb = new StringBuilder();

        if (data?.items != null)
        {
            int count = 0;

            foreach (var valve in data.items)
            {
                string result = valve.is_correct
                    ? "<color=#00C853>Верно</color>"
                    : "<color=#D50000>Неверно</color>";

                sb.Append($"<b>{valve.valve_number}</b>: {result}");

                count++;

                if (count < data.items.Count)
                {
                    if (count % 4 == 0)
                        sb.AppendLine();
                    else
                        sb.Append("    |    ");
                }
            }
        }

        item.SetDetails(sb.ToString());

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot as RectTransform);
    }

    // Открытие окна смены пароля
    public void OpenChangePasswordDialog()
    {
        if (changePasswordDialog != null)
            changePasswordDialog.Open();
    }
}