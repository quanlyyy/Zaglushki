using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 4f;
    public float gravity = -9.8f;

    CharacterController controller;
    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //при пробеле не ходим
        if (Input.GetKey(KeyCode.Space))
            return;

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            move += transform.forward;

        if (Input.GetKey(KeyCode.S))
            move -= transform.forward;

        if (Input.GetKey(KeyCode.A))
            move -= transform.right;

        if (Input.GetKey(KeyCode.D))
            move += transform.right;

        move.Normalize();

        controller.Move(move * speed * Time.deltaTime);

        //физика
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
