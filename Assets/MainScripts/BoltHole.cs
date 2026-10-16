using UnityEngine;

public class BoltHole : MonoBehaviour
{
    public Flange flange;
    public Bolt targetBolt;

    public Outline outline;
    public float outlineWidth = 3f;

    void Awake()
    {
        if (outline == null) outline = GetComponent<Outline>();
        SetHighlight(false);
    }

    void OnMouseEnter()
    {
        if (CanHover()) SetHighlight(true);
    }

    void OnMouseExit()
    {
        SetHighlight(false);
    }

    void OnMouseDown()
    {
        if (!CanHover()) return;

        // запускаем закручивание и удаляем цилиндр после завершения анимации
        targetBolt.Tighten(() => Destroy(gameObject));
    }

    bool CanHover()
    {
        if (flange == null || targetBolt == null) return false;

        if (TrainingManager.Instance != null)
        {
            if (!TrainingManager.Instance.CanWorkWithFlange(flange))
                return false;
        }

        // после заглушки
        if (!flange.CanTightenNow()) return false;

        // нужен ключ
        if (!PlayerInventory.Instance.HasHandItem(HandItemType.Wrench)) return false;

        // болт должен быть снят
        if (!targetBolt.isRemoved) return false;

        // если уже закручен навсегда
        if (targetBolt.isLockedAfterTighten) return false;

        // если болт сейчас проигрывает анимацию
        if (targetBolt.isBusy) return false;

        return true;
    }

    void SetHighlight(bool value)
    {
        if (outline != null)
            outline.OutlineWidth = value ? outlineWidth : 0f;
    }
}