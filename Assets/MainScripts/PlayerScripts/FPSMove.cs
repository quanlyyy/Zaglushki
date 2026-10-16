using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Move")]
public class FPSMove : MonoBehaviour
{
    public float speed = 2.5f;
    public float gravity = -9.8f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //пока пробел блочим ходьбу
        if (Input.GetKey(KeyCode.Space))
            return;

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) move += Vector3.back;
        if (Input.GetKey(KeyCode.A)) move += Vector3.left;
        if (Input.GetKey(KeyCode.D)) move += Vector3.right;

        move = Vector3.ClampMagnitude(move, 1f);
        move = transform.TransformDirection(move);

        controller.Move(move * speed * Time.deltaTime);

        //физика
        if (controller.isGrounded)
            velocity.y = 0;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
