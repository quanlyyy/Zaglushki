using UnityEngine;
using TMPro;

public class ExamTimer : MonoBehaviour
{
    public static ExamTimer Instance;

    [Header("Timer")]
    public float totalTimeSeconds = 600f;
    private float currentTime;
    private bool isRunning = true;

    [Header("UI")]
    public TMP_Text gameplayTimerText;
    public TMP_Text resultsTimeText;

    void Awake()
    {
        Instance = this;
        currentTime = totalTimeSeconds;
    }

    void Update()
    {
        if (!isRunning)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime < 0f)
            currentTime = 0f;

        UpdateGameplayUI();

        if (currentTime <= 0f)
        {
            isRunning = false;

            if (ExamController.Instance != null)
            {
                Debug.LogError("Время экзамена вышло");
                ExamController.Instance.FailExam_TimeOut();
            }
        }
    }

    void UpdateGameplayUI()
    {
        if (gameplayTimerText != null)
            gameplayTimerText.text = FormatTime(currentTime);
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetTimeSpent()
    {
        return totalTimeSeconds - currentTime;
    }

    public string GetFormattedTimeSpent()
    {
        return FormatTime(GetTimeSpent());
    }

    public string GetFormattedTimeLeft()
    {
        return FormatTime(currentTime);
    }

    public void UpdateResultsUI()
    {
        if (resultsTimeText != null)
            resultsTimeText.text = $"<b>Время прохождения:</b> {GetFormattedTimeSpent()}";
    }

    private string FormatTime(float seconds)
    {
        int mins = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{mins:00}:{secs:00}";
    }
}