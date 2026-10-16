using UnityEngine;
using System;

public class GasMaskButton : MonoBehaviour
{
    public GameObject icon;
    public GasReleaseController gasController;

    void Start()
    {
        PlayerInventory.Instance.OnInventoryChanged += UpdateIcon;
        UpdateIcon();
    }

    void OnDestroy()
    {
        PlayerInventory.Instance.OnInventoryChanged -= UpdateIcon;
    }

    public void OnClick()
    {
        if (PlayerInventory.Instance.isGasMaskOn)
            return;

        PlayerInventory.Instance.ToggleGasMask();

        if (gasController != null)
            gasController.DisableGasZone();

        if (TrainingManager.Instance != null)
            TrainingManager.Instance.OnGasMaskEquipped();

        // после надевания маски кнопка исчезает
        gameObject.SetActive(false);
    }

    void UpdateIcon()
    {
        // иконка скрыта, если маска надета
        icon.SetActive(!PlayerInventory.Instance.isGasMaskOn);
    }
}
