using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExamController : MonoBehaviour
{
    public static ExamController Instance;

    [Header("Setup")]
    public List<ValveUnit> allValves;

    [Header("Runtime")]
    public ValveUnit activeValveUnit;
    public Flange activeFlange;

    [Header("Score")]
    public int score = 0;

    [Header("Results UI")]
    public ResultsUIManager resultsUIManager;

    [Header("Result Upload")]
    public ExamResultUploader examResultUploader;

    [Header("Exam Timer")]
    private float elapsedTime = 0f;
    private bool isExamRunning = true;
    private bool resultUploaded = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isExamRunning)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void AddScore(int value, string reason)
    {
        score += value;
        Debug.Log($"[ОЧКИ] +{value} | {reason} | Текущий счёт: {score}");
    }

    public void AddZeroScore(string reason)
    {
        Debug.Log($"[ОЧКИ] +0 | {reason} | Текущий счёт: {score}");
    }

    public void LockFlange(Flange flange)
    {
        if (activeValveUnit != null)
            return;

        activeValveUnit = flange.valveUnit;
        activeFlange = flange;
    }

    public bool CanUnscrewOnFlange(Flange flange)
    {
        if (activeValveUnit == null)
            return true;

        return flange != null &&
               flange.valveUnit == activeValveUnit &&
               flange == activeFlange;
    }

    public void FinishCurrentValve(ValveUnit valve)
    {
        if (valve == null)
            return;

        valve.isFinished = true;

        Debug.Log($"Задвижка завершена: {valve.name}");

        if (activeValveUnit == valve)
        {
            activeValveUnit = null;
            activeFlange = null;
        }

        CheckAllValvesFinished();
    }

    public void CheckAllValvesFinished()
    {
        foreach (var valve in allValves)
        {
            if (valve == null || !valve.isFinished)
                return;
        }

        Debug.Log(" Все задвижки завершены");

        // останавливаем таймер
        isExamRunning = false;

        // загружаем результат на сервер 
        if (!resultUploaded && examResultUploader != null)
        {
            resultUploaded = true;
            StartCoroutine(examResultUploader.UploadResult(
                score,
                176,
                Mathf.RoundToInt(elapsedTime),
                allValves
            ));
        }

        if (resultsUIManager != null)
            resultsUIManager.ShowResults();
        else
            Debug.LogError("результат менеджер не назначен");
    }

    public int GetElapsedTimeSeconds()
    {
        return Mathf.RoundToInt(elapsedTime);
    }

    public void RegisterError(string msg)
    {
        Debug.LogError("ОШИБКА ЭКЗАМЕНА: " + msg);
    }

    public void FailExam_GasTimeout()
    {
        Debug.LogError("Экзамен завершён: игрок не покинул газовую зону за 60 секунд");
        isExamRunning = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void FailExam_TimeOut()
    {
        Debug.LogError(" Экзамен завершён: время вышло");
        isExamRunning = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}