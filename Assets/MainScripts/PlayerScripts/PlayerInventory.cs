using UnityEngine;
using System;

public enum HandItemType
{
    None,
    Wrench,
    Plug
}

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Hand Items")]
    public HandItemType currentHandItem = HandItemType.None;
    public GameObject wrenchObject;
    public GameObject plugObject;

    [Header("Equipment")]
    public bool isGasMaskOn;
    public GameObject gasMaskObject;

    [Header("Mask")]
    public bool gasMaskOn;

    public event Action OnInventoryChanged;
    public event Action OnGasMaskEquipped;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        DisableHandItems();
        gasMaskObject.SetActive(false);
    }

    //ручные предметы
    public void ToggleHandItem(HandItemType item)
    {
        if (currentHandItem == item)
        {
            DisableHandItems();
            currentHandItem = HandItemType.None;
        }
        else
        {
            DisableHandItems();
            currentHandItem = item;

            if (item == HandItemType.Wrench) wrenchObject.SetActive(true);
            if (item == HandItemType.Plug) plugObject.SetActive(true);
        }
        if (TrainingManager.Instance != null && currentHandItem == HandItemType.Wrench)
        {
            TrainingManager.Instance.OnWrenchTaken();
        }

        if (TrainingManager.Instance != null && currentHandItem == HandItemType.Plug)
        {
            TrainingManager.Instance.OnPlugTaken();
        }

        OnInventoryChanged?.Invoke();
    }

    void DisableHandItems()
    {
        wrenchObject.SetActive(false);
        plugObject.SetActive(false);
    }

    public bool HasHandItem(HandItemType item)
    {
        return currentHandItem == item;
    }

    //противогаз
    public void ToggleGasMask()
    {
        isGasMaskOn = !isGasMaskOn;
        gasMaskObject.SetActive(isGasMaskOn);

        OnGasMaskEquipped?.Invoke();
        OnInventoryChanged?.Invoke();
    }

    public void EquipGasMaskPermanent()
    {
        gasMaskOn = true;
        Debug.Log("Противогаз надет (навсегда)");
    }
}
