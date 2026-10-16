using UnityEngine;

public class Crouch : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;

    [Header("Heights")]
    public float standHeight = 1.8f;
    public float crouchHeight = 1.0f;

    [Header("Camera Y")]
    public float standCamY = 1.6f;
    public float crouchCamY = 0.9f;

    [Header("Check")]
    public float headCheckRadius = 0.25f;
    public LayerMask obstacleMask;

    private bool isCrouching;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            TryCrouch();
        else
            TryStand();
    }

    void TryCrouch()
    {
        if (isCrouching) return;

        isCrouching = true;

        controller.height = crouchHeight;
        controller.center = new Vector3(0, crouchHeight / 2f, 0);

        SetCameraY(crouchCamY);
    }

    void TryStand()
    {
        if (!isCrouching) return;

        //проверка есть ли место над головой
        if (Physics.SphereCast(
            transform.position + Vector3.up * crouchHeight,
            headCheckRadius,
            Vector3.up,
            out _,
            standHeight - crouchHeight,
            obstacleMask))
        {
            return; // нельзя встать
        }

        isCrouching = false;

        controller.height = standHeight;
        controller.center = new Vector3(0, standHeight / 2f, 0);

        SetCameraY(standCamY);
    }

    void SetCameraY(float y)
    {
        Vector3 pos = cameraTransform.localPosition;
        pos.y = y;
        cameraTransform.localPosition = pos;
    }
}
