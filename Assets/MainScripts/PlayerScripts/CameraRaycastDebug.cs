using UnityEngine;

public class CameraRaycastDebug : MonoBehaviour
{
    public float distance = 5f;
    private Outline currentOutline;

    void Update()
    {
        Ray ray;

        //вывод луча из цетра
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        }
        else
        {
            //вывод луча из курсора
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            Outline outline = hit.collider.GetComponentInParent<Outline>();

            if (outline != null)
            {
                if (currentOutline != outline)
                {
                    Clear();
                    currentOutline = outline;
                    outline.OutlineWidth = 4f;
                }
                return;
            }
        }

        Clear();
    }

    void Clear()
    {
        if (currentOutline != null)
        {
            currentOutline.OutlineWidth = 0f;
            currentOutline = null;
        }
    }
}
