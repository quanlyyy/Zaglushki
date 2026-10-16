using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazTrig : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GameObject.Find("Player").GetComponent<Player>().antiGazState = true;
        }
    }
}
