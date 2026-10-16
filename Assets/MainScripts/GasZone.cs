using System.Collections;
using UnityEngine;

public class GasZone : MonoBehaviour
{
    private GasReleaseController controller;
    private Coroutine timerRoutine;

    public void Init(GasReleaseController c) => controller = c;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        controller?.OnPlayerEnterGasZone();

        if (timerRoutine != null) StopCoroutine(timerRoutine);
        timerRoutine = StartCoroutine(StayTimer());
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        controller?.OnPlayerExitGasZone();

        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }
    }

    IEnumerator StayTimer()
    {
        float t = 60f;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            yield return null;
        }

        // если не вышел за минуту
        if (ExamController.Instance != null)
            ExamController.Instance.FailExam_GasTimeout();
    }
}