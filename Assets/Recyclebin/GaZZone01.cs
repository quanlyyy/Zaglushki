using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GaZZone01 : MonoBehaviour
{
    Transform target;
    Transform player;
    float x = 0;
    float y = 0;
    Vector3 delta;

    float gaz = 0.0f;

    [SerializeField]
    public bool gazAnalizator = false;

    float distance;

    private void Start()
    {
        target = GameObject.Find("Bolts").transform;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out PlayerHealth health))
            health.ApplyGas();
    }

    private void Update()
    {
        player = GameObject.Find("Player").transform;
        
        delta = target.position - player.position;
        distance = delta.magnitude;

        GameObject.Find("H25").GetComponent<Text>().text = Mathf.Round(gaz).ToString();
        GameObject.Find("Ch4").GetComponent<Text>().text = Mathf.Round(gaz).ToString();
        if (distance > 20)
        {
            gazAnalizator = false;
        }

        if (gazAnalizator && distance < 20 && gaz < 35)
        {
            for (float i = 0; i < 35; i += 5.0f) {
                gaz += 5.0f;
            }
        }
        if(gaz > 0.5f && !gazAnalizator)
        {
            gaz -= 5.0f;
        }

        if(gaz > 8.0f)
        {
            GameObject.Find("Audio Source").GetComponent<AudioSource>().volume = 1.0f;
        }
        else
        {
            GameObject.Find("Audio Source").GetComponent<AudioSource>().volume = 0.0f;
        }
    }
}
