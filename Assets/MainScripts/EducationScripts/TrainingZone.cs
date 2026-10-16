using UnityEngine;

public class TrainingZone : MonoBehaviour
{
    public ValveUnit targetValve;
    public bool isCompleted;

    [Header("Visual")]
    public MeshRenderer zoneRenderer;

    [Header("Special Logic")]
    public bool keepTriggerForReturn = false; // для 13 задвижки

    private bool firstEnterDone = false;

    private void Awake()
    {
        if (zoneRenderer == null)
            zoneRenderer = GetComponent<MeshRenderer>();
    }

    //при входе игрока в зону
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isCompleted) return;

        if (TrainingManager.Instance.currentZone != null &&
        TrainingManager.Instance.currentZone != this)
        {
            return;
        }

        //при первой активации зоны
        if (!firstEnterDone)
        {
            firstEnterDone = true;

            TrainingManager.Instance.EnterZone(this);

            // не выколючаем триггер у 13 только визуал
            if (keepTriggerForReturn)
            {
                if (zoneRenderer != null)
                    zoneRenderer.enabled = false;
            }
            else
            {
                gameObject.SetActive(false);
            }

            return;
        }

        //повторный вход для 13 задвижки
        TrainingManager.Instance.EnterZone(this);
    }

    //выполнено
    public void MarkCompleted()
    {
        isCompleted = true;
        gameObject.SetActive(false);
    }
}