using UnityEngine;

[RequireComponent(typeof(Outline))]
public class BoltHighlight : MonoBehaviour
{
    private Outline outline;
    private bool isActive;

    void Awake()
    {
        outline = GetComponent<Outline>();
        outline.OutlineWidth = 0f;
    }

    public void Highlight(bool state)
    {
        if (isActive == state) return;

        isActive = state;
        outline.OutlineWidth = state ? 3f : 0f;
    }
}
