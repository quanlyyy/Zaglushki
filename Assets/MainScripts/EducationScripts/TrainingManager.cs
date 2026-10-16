using System.Collections.Generic;
using UnityEngine;
public enum TrainingStep
{
    None,
    GoToValve,
    TakeWrenchForUnscrew,
    UnscrewBolts,

    LeaveDangerZone,
    PutOnGasMask,
    ReturnToValve,

    TakePlug,
    InstallPlug,
    TakeWrenchForTighten,
    TightenBolts,

    Finished
}
public class TrainingManager : MonoBehaviour
{
    public static TrainingManager Instance;

    [Header("UI")]
    public TrainingUIManager uiManager;

    [Header("Zones")]
    public List<TrainingZone> allZones;

    [Header("Runtime")]
    public TrainingZone currentZone;
    private int completedZones = 0;

    public TrainingStep currentStep = TrainingStep.None;

    void Awake()
    {
        Instance = this;
    }

    public void StartTraining()
    {
        currentStep = TrainingStep.GoToValve;
        uiManager.ShowHint("Подойдите к задвижке");
    }

    public void EnterZone(TrainingZone zone)
    {
        if (zone == null) return;

        //для обычной зоны
        if (currentZone == null)
            currentZone = zone;

        //возращение к 13 задвижке после противогаза
        if (currentStep == TrainingStep.ReturnToValve)
        {
            if (zone.targetValve != null && zone.targetValve.valveNumber == 13)
            {
                currentZone = zone;

                if (PlayerInventory.Instance != null &&
                    PlayerInventory.Instance.currentHandItem == HandItemType.Plug)
                {
                    currentStep = TrainingStep.InstallPlug;
                    uiManager.ShowHint("Установите заглушку");
                }
                else
                {
                    currentStep = TrainingStep.TakePlug;
                    uiManager.ShowHint("Возьмите заглушку");
                }
            }

            return;
        }

        //обычная логика 
        if (PlayerInventory.Instance != null &&
            PlayerInventory.Instance.currentHandItem == HandItemType.Wrench)
        {
            currentStep = TrainingStep.UnscrewBolts;
            uiManager.ShowHint("Открутите все болты");
        }
        else
        {
            currentStep = TrainingStep.TakeWrenchForUnscrew;
            uiManager.ShowHint("Возьмите гаечный ключ");
        }
    }

    //после ключа
    public void OnWrenchTaken()
    {
        if (currentZone == null) return;

        if (currentStep == TrainingStep.TakeWrenchForUnscrew)
        {
            currentStep = TrainingStep.UnscrewBolts;
            uiManager.ShowHint("Открутите все болты");
        }
        else if (currentStep == TrainingStep.TakeWrenchForTighten)
        {
            currentStep = TrainingStep.TightenBolts;
            uiManager.ShowHint("Закрутите все болты");
        }
    }

    //после болтов
    public void OnBoltsUnscrewed(Flange flange)
    {
        if (currentZone == null) return;
        if (currentStep != TrainingStep.UnscrewBolts) return;

        //для 13 задвижки
        if (flange != null && flange.valveUnit != null && flange.valveUnit.valveNumber == 13)
        {
            currentStep = TrainingStep.LeaveDangerZone;
            uiManager.ShowHint("Выйдите за каре");
            return;
        }

        //обыная логика
        if (PlayerInventory.Instance != null &&
            PlayerInventory.Instance.currentHandItem == HandItemType.Plug)
        {
            currentStep = TrainingStep.InstallPlug;
            uiManager.ShowHint("Установите заглушку");
        }
        else
        {
            currentStep = TrainingStep.TakePlug;
            uiManager.ShowHint("Возьмите заглушку");
        }
    }

    //при входе в сейва триггер
    public void OnEnteredEquipZone()
    {
        if (currentStep != TrainingStep.LeaveDangerZone)
            return;

        currentStep = TrainingStep.PutOnGasMask;
        uiManager.ShowHint("Наденьте противогаз");
    }

    //при надетов противогазе
    public void OnGasMaskEquipped()
    {
        if (currentStep != TrainingStep.PutOnGasMask)
            return;

        currentStep = TrainingStep.ReturnToValve;
        uiManager.ShowHint("Вернитесь к задвижке");
    }

    public void OnPlugTaken()
    {
        if (currentZone == null) return;
        if (currentStep != TrainingStep.TakePlug) return;

        currentStep = TrainingStep.InstallPlug;
        uiManager.ShowHint("Установите заглушку");
    }

    public void OnPlugInstalled(Flange flange)
    {
        if (currentZone == null) return;
        if (currentStep != TrainingStep.InstallPlug) return;

        if (PlayerInventory.Instance != null &&
            PlayerInventory.Instance.currentHandItem == HandItemType.Wrench)
        {
            currentStep = TrainingStep.TightenBolts;
            uiManager.ShowHint("Закрутите все болты");
        }
        else
        {
            currentStep = TrainingStep.TakeWrenchForTighten;
            uiManager.ShowHint("Возьмите гаечный ключ");
        }
    }

    public void OnValveFinished(ValveUnit valve)
    {
        if (currentZone == null) return;

        if (currentZone.targetValve == valve && !currentZone.isCompleted)
        {
            currentZone.MarkCompleted();
            completedZones++;

            if (completedZones >= allZones.Count)
            {
                currentStep = TrainingStep.Finished;
                uiManager.ShowFinishCanvas();
            }
            else
            {
                currentZone = null;
                currentStep = TrainingStep.GoToValve;
                uiManager.ShowHint("Подойдите к следующей задвижке");
            }
        }
    }

    //проверка активна ли зона или нет
    public bool HasActiveZone()
    {
        return currentZone != null;
    }

    //проверяет может ли пользователь взаимодействовать с задвижкой
    public bool CanWorkWithValve(ValveUnit valve)
    {
        if (currentZone == null) return false;
        if (valve == null) return false;

        return currentZone.targetValve == valve;
    }

    // проверяет можно ли взаимодействовать с фланцем
    public bool CanWorkWithFlange(Flange flange)
    {
        if (flange == null) return false;
        return CanWorkWithValve(flange.valveUnit);
    }
}