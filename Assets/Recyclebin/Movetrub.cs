 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Movetrub : MonoBehaviour
{
    public GameObject trubL;
    public GameObject trubR;

    public GameObject trub;

    public bool enabledTrubs = false;

    // Start is called before the first frame update
    void Start()
    {
        trubL.SetActive(false);
        trubR.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void moveTrub(string rl)
    {
        if (rl == "L" && !enabledTrubs)
        {
            trub.SetActive(false) ;
            trubL .SetActive(true) ;
            enabledTrubs = true ;
        }
        else if (rl == "R" && !enabledTrubs)
        {
            trub.SetActive(false ) ;   
            trubR .SetActive(true) ;
            enabledTrubs = true ;
        }
        else
        {
            Debug.LogError("Ошибка определения стороны для установки задвижки");
        }
        


    }
}
