using UnityEngine;

public class CameraBoltRaycaster : MonoBehaviour
{
    private Camera cam;
    public float distance = 3f;

    private Bolt currentBolt;

    void Awake()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam == null)
        {
            Debug.LogError("камеры нет");
        }
    }

    void Update()
    {
        if (cam == null)
            return;

        //выключение подсветки пред болта
        if (currentBolt != null)
        {
            currentBolt.DisableHighlight();
            currentBolt = null;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance))
        {
            Bolt bolt = hit.collider.GetComponentInParent<Bolt>();

            if (bolt != null)
            {
                currentBolt = bolt;
                currentBolt.EnableHighlight();

                if (Input.GetMouseButtonDown(0))
                {
                    currentBolt.TryUnscrew();
                }
            }
        }
    }
}
