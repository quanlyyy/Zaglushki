using UnityEngine;

[AddComponentMenu("Control Script/Mouse Look")]
public class MouseLook : MonoBehaviour
{
    public enum RotationAxes
    {
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxes axes = RotationAxes.MouseX;

    public float sensitivity = 3.0f;
    public float minimumVert = -80.0f;
    public float maximumVert = 80.0f;

    private float rotationX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
            body.freezeRotation = true;
    }

    void Update()
    {
        //взаимодействие через пробел
        if (Input.GetKey(KeyCode.Space))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (axes == RotationAxes.MouseX)
        {
            float rotY = Input.GetAxisRaw("Mouse X") * sensitivity;
            transform.Rotate(0, rotY, 0);
        }
        else if (axes == RotationAxes.MouseY)
        {
            rotationX -= Input.GetAxisRaw("Mouse Y") * sensitivity;
            rotationX = Mathf.Clamp(rotationX, minimumVert, maximumVert);
            transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }
}
