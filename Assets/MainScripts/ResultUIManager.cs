using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsUIManager : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject gameplayCanvas;
    public GameObject resultsCanvas;

    [Header("Texts")]
    public TMP_Text titleText;
    public TMP_Text resultsText;
    public TMP_Text totalScoreText;
    public TMP_Text timeText;

    [Header("Score")]
    public int maxScore = 176;

    [Header("Player Freeze")]
    public GameObject playerRoot;
    public MonoBehaviour[] scriptsToDisable;

    private bool resultsShown;

    void Start()
    {
        if (resultsCanvas != null)
            resultsCanvas.SetActive(false);
    }

    void Update()
    {
        if (resultsShown)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ShowResults()
    {
        if (ExamController.Instance == null)
        {
            Debug.LogError("ResultsUIManager: ExamController.Instance == null");
            return;
        }

        resultsShown = true;

        StringBuilder sb = new StringBuilder();

        foreach (var valve in ExamController.Instance.allValves)
        {
            bool isCorrect = valve.IsPlugCorrect();

            string resultWord = isCorrect
                ? "<color=#00C853><b>Верно</b></color>"
                : "<color=#D50000><b>Неверно</b></color>";

            sb.AppendLine($"Задвижка №{valve.valveNumber}: {resultWord}");
        }

        if (titleText != null)
            titleText.text = "<b>Результаты экзамена</b>";

        if (resultsText != null)
            resultsText.text = sb.ToString();

        if (totalScoreText != null)
            totalScoreText.text = $"<b>Итого очков:</b> {ExamController.Instance.score}/{maxScore}";

        if (ExamTimer.Instance != null)
        {
            ExamTimer.Instance.StopTimer();
            ExamTimer.Instance.UpdateResultsUI();

            if (timeText != null)
                timeText.text = $"<b>Время прохождения:</b> {ExamTimer.Instance.GetFormattedTimeSpent()}";
        }

        if (gameplayCanvas != null)
            gameplayCanvas.SetActive(false);

        if (resultsCanvas != null)
            resultsCanvas.SetActive(true);

        FreezeGame();
    }

    private void FreezeGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (scriptsToDisable != null)
        {
            foreach (var script in scriptsToDisable)
            {
                if (script != null)
                    script.enabled = false;
            }
        }

        Time.timeScale = 0f;
    }

    public void ExitToMainMenu(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}