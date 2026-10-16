using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CameraPlugRaycaster : MonoBehaviour
{
    private Camera cam;

    public float distance = 3f;

    private PlugPoint current;

    void Awake()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (cam == null)
            return;

        //обновление подсветки
        if (current != null)
        {
            current.SetHighlight(false);
            current = null;
        }

        //создание луча
        Ray ray = (Cursor.lockState == CursorLockMode.Locked)
            ? cam.ViewportPointToRay(new Vector3(0.5f, 0.5f))
            : cam.ScreenPointToRay(Input.mousePosition);

        //проверка поподания лучв
        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            PlugPoint point = hit.collider.GetComponentInParent<PlugPoint>();

            if (point == null)
                return;

            if (!point.CanHover())
                return;

            current = point;

            current.SetHighlight(true);

            if (LeftClickDown())
                current.Install();
        }

        //проверка клика мыши
        if (LeftClickDown())
        {
            if (current.CanInstall())
                current.Install();
            else
                Debug.Log("не выполнены условия, заглушка не в рукках");
        }
    }

    //проверка нажатия левой кнопки мыши
    bool LeftClickDown()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }
}