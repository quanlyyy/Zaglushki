using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InstructorResultsUI : MonoBehaviour
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

    private enum SortMode
    {
        Date,
        Score
    }

    [Header("UI")]
    public Transform contentRoot;
    public AttemptListItemUI itemPrefab;

    [Header("Controls")]
    public TMP_Text scoreSortButtonText;
    public TMP_Text dateSortButtonText;
    public TMP_InputField searchInput;

    private List<AttemptDto> loadedAttempts = new List<AttemptDto>();
    private List<AttemptDto> filteredAttempts = new List<AttemptDto>();

    private bool scoreDescending = true;
    private bool dateLatestFirst = true;
    private bool isDataLoaded = false;

    private SortMode currentSortMode = SortMode.Date;

    void Start()
    {
        if (contentRoot == null)
        {
            Debug.LogError("не назначен контент");
            return;
        }

        if (itemPrefab == null)
        {
            Debug.LogError("Не назначен префаб");
            return;
        }

        if (searchInput != null)
            searchInput.onValueChanged.AddListener(OnSearchChanged);

        StartCoroutine(LoadAttempts());
    }

    public IEnumerator LoadAttempts()
    {
        using var req = new UnityWebRequest(SupabaseConfig.RpcAttemptsUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes("{}"));
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

        string wrappedJson = "{\"items\":" + req.downloadHandler.text + "}";
        var data = JsonUtility.FromJson<AttemptArrayWrapper>(wrappedJson);

        loadedAttempts.Clear();

        if (data?.items != null)
            loadedAttempts = data.items;

        isDataLoaded = true;
        ApplyFiltersAndRefresh();
    }

    public void ToggleScoreSort()
    {
        if (currentSortMode == SortMode.Score)
            scoreDescending = !scoreDescending;
        else
            currentSortMode = SortMode.Score;

        ApplyFiltersAndRefresh();
    }

    public void ToggleDateSort()
    {
        if (currentSortMode == SortMode.Date)
            dateLatestFirst = !dateLatestFirst;
        else
            currentSortMode = SortMode.Date;

        ApplyFiltersAndRefresh();
    }

    public void ResetFilters()
    {
        scoreDescending = true;
        dateLatestFirst = true;
        currentSortMode = SortMode.Date;

        if (searchInput != null)
            searchInput.text = "";

        ApplyFiltersAndRefresh();
    }

    public void OnSearchChanged(string value)
    {
        ApplyFiltersAndRefresh();
    }

    private void ApplyFiltersAndRefresh()
    {
        if (!isDataLoaded)
            return;

        IEnumerable<AttemptDto> query = loadedAttempts;

        //поиск по фио
        if (searchInput != null && !string.IsNullOrWhiteSpace(searchInput.text))
        {
            string search = searchInput.text.Trim().ToLower();
            query = query.Where(x =>
                !string.IsNullOrWhiteSpace(x.full_name) &&
                x.full_name.ToLower().Contains(search));
        }

        IOrderedEnumerable<AttemptDto> orderedQuery;

        if (currentSortMode == SortMode.Date)
        {
            if (dateLatestFirst)
                orderedQuery = query.OrderByDescending(x => ParseDateSafe(x.finished_at));
            else
                orderedQuery = query.OrderBy(x => ParseDateSafe(x.finished_at));

            //вторичная сортировка по очкам
            orderedQuery = orderedQuery.ThenByDescending(x => x.score);
        }
        else
        {
            if (scoreDescending)
                orderedQuery = query.OrderByDescending(x => x.score);
            else
                orderedQuery = query.OrderBy(x => x.score);

            // вторичная сортировка по дате
            orderedQuery = orderedQuery.ThenByDescending(x => ParseDateSafe(x.finished_at));
        }

        filteredAttempts = orderedQuery.ToList();

        RebuildList();
    }

    private void RebuildList()
    {
        if (contentRoot == null)
        {
            Debug.LogError("не назначен контент");
            return;
        }

        if (itemPrefab == null)
        {
            Debug.LogError("Не назначен префаб");
            return;
        }

        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        foreach (var attempt in filteredAttempts)
        {
            var item = Instantiate(itemPrefab, contentRoot);

            item.Setup(
                this,
                attempt.attempt_id,
                attempt.finished_at,
                attempt.full_name,
                attempt.score,
                attempt.max_score,
                attempt.time_spent_seconds
            );
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot as RectTransform);
    }


    private DateTimeOffset ParseDateSafe(string value)
    {
        if (DateTimeOffset.TryParse(value, out var dt))
            return dt;

        return DateTimeOffset.MinValue;
    }

    public void LoadDetails(AttemptListItemUI item, string attemptId)
    {
        StartCoroutine(LoadDetailsRoutine(item, attemptId));
    }

    private IEnumerator LoadDetailsRoutine(AttemptListItemUI item, string attemptId)
    {
        string body = "{\"p_attempt_id\":\"" + attemptId + "\"}";

        using var req = new UnityWebRequest(SupabaseConfig.RpcAttemptDetailsUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
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
}