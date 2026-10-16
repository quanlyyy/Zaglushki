using UnityEngine;

public class HandItemButton : MonoBehaviour
{
    public HandItemType itemType;
    public GameObject icon;

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
        PlayerInventory.Instance.ToggleHandItem(itemType);
    }

    void UpdateIcon()
    {
        icon.SetActive(PlayerInventory.Instance.currentHandItem != itemType);
    }
}
