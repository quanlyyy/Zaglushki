using UnityEngine;

public class CameraBoltClicker : MonoBehaviour
{
    public float distance = 3f;

    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray;

        //луч из центра если курсора нет
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        }
        else
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            Bolt bolt = hit.collider.GetComponentInParent<Bolt>();
            if (bolt != null)
            {
                bolt.TryUnscrew();
            }
        }
    }
}
