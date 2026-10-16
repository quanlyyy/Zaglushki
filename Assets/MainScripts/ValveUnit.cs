using UnityEngine;

public class ValveUnit : MonoBehaviour
{
    public FlangeSide correctSide;

    public Flange leftFlange;
    public Flange rightFlange;
    public int valveNumber = 0;

    [Header("Runtime")]
    public Flange usedFlange;
    public Flange plugInstalledOn;

    public bool isFinished;

    public void RegisterPlugInstallation(Flange flange)
    {
        if (plugInstalledOn != null)
            return;

        plugInstalledOn = flange;
    }

    //проверка правильности установки заглушки
    public bool IsPlugCorrect()
    {
        if (plugInstalledOn == null)
            return false;

        //сверка стороны
        return plugInstalledOn.side == correctSide;
    }

    public void RegisterFlangeUsage(Flange flange)
    {
        if (usedFlange != null)
            return;

        usedFlange = flange;

        //отключение соседнего фланца
        if (flange == leftFlange)
            rightFlange.SetBoltsInteractable(false);

        else if (flange == rightFlange)
            leftFlange.SetBoltsInteractable(false);
    }

    public bool IsCorrect()
    {
        if (usedFlange == null)
            return false;

        //проверка стороны
        return usedFlange.side == correctSide;
    }

    //уведолмение о полном откручивании фланца
    public void NotifyFlangeFullyUnscrewed(Flange flange)
    {
        SendMessage(
            "OnFlangeFullyUnscrewed",
            flange,
            SendMessageOptions.DontRequireReceiver
        );
    }
}