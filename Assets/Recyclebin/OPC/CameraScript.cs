using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//скрипт управляет движением персонажа по сцене (пока что персонаж это просто камера)
public class CameraScript : MonoBehaviour
{
    public float movingSpeed = 0.002f; //скорость движения камеры при нажатии WASD

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //при нажатии на WASD меняем позицию камеры
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = new Vector3(transform.position.x - movingSpeed, transform.position.y - movingSpeed / 2, transform.position.z - movingSpeed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position = new Vector3(transform.position.x + movingSpeed, transform.position.y + movingSpeed / 2, transform.position.z + movingSpeed);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(transform.position.x + movingSpeed, transform.position.y, transform.position.z - movingSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position = new Vector3(transform.position.x - movingSpeed, transform.position.y, transform.position.z + movingSpeed);
        }
    }
}
