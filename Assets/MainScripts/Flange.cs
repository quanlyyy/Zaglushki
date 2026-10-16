using UnityEngine;
using System.Collections.Generic;

public enum FlangeSide
{
    Left,
    Right
}
public class Flange : MonoBehaviour
{
    [Header("Setup")]
    public FlangeSide side;
    public ValveUnit valveUnit;
    public List<Bolt> bolts;

    [Header("Runtime")]
    public bool isCompleted;
    private int unscrewedCount;

    public System.Action OnAllBoltsUnscrewed;
    public PlugPreview plugPreview;

    [Header("Scoring Order")]
    public int[] correctBoltOrder = new int[] { 4, 1, 5, 2, 3, 6 };

    private int unscrewOrderIndex = 0;
    private int tightenOrderIndex = 0;

    void Awake()
    {
        unscrewedCount = 0;
        isCompleted = false;
    }

    public void OnBoltUnscrewed(Bolt bolt)
    {
        if (valveUnit != null)
            valveUnit.RegisterFlangeUsage(this);

        ExamController.Instance.LockFlange(this);

        unscrewedCount++;

        if (!isCompleted && unscrewedCount >= bolts.Count)
        {
            isCompleted = true;
            if (valveUnit != null)
                valveUnit.NotifyFlangeFullyUnscrewed(this);
            if (plugPreview != null)
                plugPreview.SetAvailable(true);
            Debug.Log($"Фланец {name} ({side}) — все болты откручены");
            OnAllBoltsUnscrewed?.Invoke();

            if (TrainingManager.Instance != null)
                TrainingManager.Instance.OnBoltsUnscrewed(this);
        }
    }

    //блокировка соседнего фланца
    public void SetBoltsInteractable(bool value)
    {
        foreach (var bolt in bolts)
        {
            if (bolt == null) continue;
            bolt.SetInteractable(value);
        }
    }

    public bool CanTightenNow()
    {
        if (!isCompleted) return false;

        if (valveUnit == null) return false;
        return valveUnit.plugInstalledOn == this;
    }

    private int tightenedCount = 0;

    public void OnBoltTightened()
    {
        tightenedCount++;

        if (tightenedCount < bolts.Count)
            return;
            

        Debug.Log($"Фланец {name}: все болты закручены");

        if (valveUnit == null)
        {
            Debug.LogError("Flange: valveUnit == null, невозможно проверить экзамен");
            return;
        }

        if (valveUnit.plugInstalledOn == null)
        {
            Debug.LogError($"Задвижка {valveUnit.name}: ЗАГЛУШКА НЕ УСТАНОВЛЕНА");
            return;
        }

        bool correct = valveUnit.IsPlugCorrect();

        if (correct)
        {
            Debug.Log(
                $"ЗАДВИЖКА {valveUnit.name}: " +
                $"ЗАГЛУШКА УСТАНОВЛЕНА ВЕРНО ({valveUnit.plugInstalledOn.side})"
            );
        }
        else
        {
            Debug.LogError(
                $"ЗАДВИЖКА {valveUnit.name}: " +
                $"ОШИБКА! Заглушка установлена на стороне {valveUnit.plugInstalledOn.side}, " +
                $"а должна быть {valveUnit.correctSide}"
            );
        }

        if (tightenedCount >= bolts.Count)
        {
            Debug.Log($"Фланец {name}: все болты закручены");

            //сброс активной задвижки и фланца
            if (ExamController.Instance != null && valveUnit != null)
                ExamController.Instance.FinishCurrentValve(valveUnit);

            if (TrainingManager.Instance != null && valveUnit != null)
                TrainingManager.Instance.OnValveFinished(valveUnit);
        }
    }


    public void CheckUnscrewOrder(Bolt bolt)
    {
        if (unscrewOrderIndex >= correctBoltOrder.Length)
            return;

        int expectedBolt = correctBoltOrder[unscrewOrderIndex];

        if (bolt.boltNumber == expectedBolt)
        {
            ExamController.Instance.AddScore(1, $"Откручен правильный болт #{bolt.boltNumber} на фланце {name}");
        }
        else
        {
            ExamController.Instance.AddZeroScore(
                $"Нарушен порядок откручивания: откручен #{bolt.boltNumber}, ожидался #{expectedBolt} на фланце {name}"
            );
        }

        unscrewOrderIndex++;
    }

    public void CheckTightenOrder(Bolt bolt)
    {
        if (tightenOrderIndex >= correctBoltOrder.Length)
            return;

        int expectedBolt = correctBoltOrder[tightenOrderIndex];

        if (bolt.boltNumber == expectedBolt)
        {
            ExamController.Instance.AddScore(1, $"Закручен правильный болт #{bolt.boltNumber} на фланце {name}");
        }
        else
        {
            ExamController.Instance.AddZeroScore(
                $"Нарушен порядок закручивания: закручен #{bolt.boltNumber}, ожидался #{expectedBolt} на фланце {name}"
            );
        }

        tightenOrderIndex++;
    }

}


