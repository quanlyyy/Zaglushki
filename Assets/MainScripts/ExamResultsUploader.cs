using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ExamResultUploader : MonoBehaviour
{
    [System.Serializable]
    public class AttemptInsert
    {
        public string user_id;
        public int score;
        public int max_score;
        public int time_spent_seconds;
        public string user_full_name;
    }

    [System.Serializable]
    public class AttemptResponse
    {
        public string id;
    }

    [System.Serializable]
    public class ValveInsert
    {
        public string attempt_id;
        public int valve_number;
        public bool is_correct;
    }

    [System.Serializable]
    private class AttemptResponseArray
    {
        public List<AttemptResponse> items;
    }

    public IEnumerator UploadResult(int score, int maxScore, int timeSpentSeconds, List<ValveUnit> valves)
    {
        var attempt = new AttemptInsert
        {
            user_id = UserSession.UserId,
            score = score,
            max_score = maxScore,
            time_spent_seconds = timeSpentSeconds,
            user_full_name = UserSession.FullName
        };

        string attemptJson = JsonUtility.ToJson(attempt);

        using var req = new UnityWebRequest(SupabaseConfig.SaveAttemptsUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(attemptJson));
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
        req.SetRequestHeader("Authorization", $"Bearer {UserSession.AccessToken}");
        req.SetRequestHeader("Prefer", "return=representation");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Ошибка сохранения попытки: " + req.downloadHandler.text);
            yield break;
        }

        string wrapped = "{\"items\":" + req.downloadHandler.text + "}";
        var response = JsonUtility.FromJson<AttemptResponseArray>(wrapped);

        if (response?.items == null || response.items.Count == 0)
            yield break;

        string attemptId = response.items[0].id;

        foreach (var valve in valves)
        {
            var valveData = new ValveInsert
            {
                attempt_id = attemptId,
                valve_number = valve.valveNumber,
                is_correct = valve.IsPlugCorrect()
            };

            string valveJson = JsonUtility.ToJson(valveData);

            using var valveReq = new UnityWebRequest(SupabaseConfig.SaveAttemptValvesUrl, "POST");
            valveReq.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(valveJson));
            valveReq.downloadHandler = new DownloadHandlerBuffer();

            valveReq.SetRequestHeader("Content-Type", "application/json");
            valveReq.SetRequestHeader("apikey", SupabaseConfig.AnonKey);
            valveReq.SetRequestHeader("Authorization", $"Bearer {UserSession.AccessToken}");

            yield return valveReq.SendWebRequest();

            if (valveReq.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Ошибка сохранения детали: " + valveReq.downloadHandler.text);
            }
        }
    }
}