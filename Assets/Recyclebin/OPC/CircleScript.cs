using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleScript : MonoBehaviour
{
    public float speed = 0.5f;
    public float radius = 0.5f;
    private float a, b;
    private int angle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // transform.RotateAround(Vector3.zero, new Vector3(a + radius * Mathf.Cos(angle), b + radius * Mathf.Sin(angle)), 0f) * speed;
    }
}
