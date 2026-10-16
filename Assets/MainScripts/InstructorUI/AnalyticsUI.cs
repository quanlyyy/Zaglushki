using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AnalyticsUI : MonoBehaviour
{
    [System.Serializable]
    private class AnalyticsSummaryDto
    {
        public int users_count;
        public int attempts_count;
        public int avg_score;
        public int avg_time;
        public int min_time;
        public int max_time;
        public int dist_0_50;
        public int dist_51_100;
        public int dist_101_150;
        public int dist_151_176;
    }

    [System.Serializable]
    private class ValveStatDto
    {
        public int valve_number;
        public float error_percent;
    }

    [System.Serializable]
    private class TopUserDto
    {
        public string full_name;
        public int best_score;
    }

    [System.Serializable]
    private class ValveStatArrayWrapper
    {
        public List<ValveStatDto> items;
    }

    [System.Serializable]
    private class TopUserArrayWrapper
    {
        public List<TopUserDto> items;
    }

    [Header("Controls")]
    public TMP_Dropdown periodDropdown;
    public TMP_Text usersCountText;
    public TMP_Text attemptsCountText;
    public TMP_Text avgScoreText;
    public TMP_Text avgTimeText;

    [Header("Middle")]
    public TMP_Text distributionText;
    public TMP_Text timeStatsText;

    [Header("Bottom")]
    public TMP_Text hardValvesText;
    public TMP_Text topUsersText;

    void Start()
    {
        LoadAnalytics();
    }

    public void LoadAnalytics()
    {
        StartCoroutine(LoadAnalyticsRoutine());
    }

    private IEnumerator LoadAnalyticsRoutine()
    {
        int? days = GetSelectedDays();

        yield return StartCoroutine(LoadSummary(days));
        yield return StartCoroutine(LoadHardValves(days));
        yield return StartCoroutine(LoadTopUsers(days));
    }

    private int? GetSelectedDays()
    {
        if (periodDropdown == null)
            return null;

        switch (periodDropdown.value)
        {
            case 1: return 7;
            case 2: return 30;
            default: return null;
        }
    }
    //выгрзука балло
    private IEnumerator LoadSummary(int? days)
    {
        string body = days.HasValue
            ? "{\"p_days\":" + days.Value + "}"
            : "{}";

        using var req = new UnityWebRequest(SupabaseConfig.RpcAnalyticsSummaryUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
        req.SetRequestHeader("Authorization", $"Bearer {UserSession.AccessToken}");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(req.downloadHandler.text);
            yield break;
        }

        var summary = JsonUtility.FromJson<AnalyticsSummaryDto>(req.downloadHandler.text);

        usersCountText.text = summary.users_count.ToString();
        attemptsCountText.text = summary.attempts_count.ToString();
        avgScoreText.text = summary.avg_score.ToString();
        avgTimeText.text = FormatTime(summary.avg_time);

        distributionText.text =
            $"0–50 баллов: {summary.dist_0_50}\n" +
            $"51–100 баллов: {summary.dist_51_100}\n" +
            $"101–150 баллов: {summary.dist_101_150}\n" +
            $"151–176 баллов: {summary.dist_151_176}";

        timeStatsText.text =
            $"Минимальное: {FormatTime(summary.min_time)}\n" +
            $"Среднее: {FormatTime(summary.avg_time)}\n" +
            $"Максимальное: {FormatTime(summary.max_time)}";
    }

    //выгрузка задвижек
    private IEnumerator LoadHardValves(int? days)
    {
        string body = days.HasValue
            ? "{\"p_days\":" + days.Value + "}"
            : "{}";

        using var req = new UnityWebRequest(SupabaseConfig.RpcHardestValvesUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();

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
        var data = JsonUtility.FromJson<ValveStatArrayWrapper>(wrapped);

        var sb = new StringBuilder();

        if (data?.items != null)
        {
            foreach (var valve in data.items)
                sb.AppendLine($"Задвижка {valve.valve_number} — {valve.error_percent}% ошибок");
        }

        hardValvesText.text = sb.ToString();
    }

    private IEnumerator LoadTopUsers(int? days)
    {
        string body = days.HasValue
            ? "{\"p_days\":" + days.Value + "}"
            : "{}";

        using var req = new UnityWebRequest(SupabaseConfig.RpcTopUsersUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();

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
        var data = JsonUtility.FromJson<TopUserArrayWrapper>(wrapped);

        var sb = new StringBuilder();

        if (data?.items != null)
        {
            for (int i = 0; i < data.items.Count; i++)
            {
                var user = data.items[i];
                sb.AppendLine($"{i + 1}. {user.full_name} — {user.best_score}");
            }
        }

        topUsersText.text = sb.ToString();
    }

    private string FormatTime(int totalSeconds)
    {
        int mins = totalSeconds / 60;
        int secs = totalSeconds % 60;
        return $"{mins:00}:{secs:00}";
    }
}