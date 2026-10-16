using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Space]
    public GameObject wrenchObj;
    public GameObject wrenchButton;
    [Space]
    public GameObject antiGazBtn;
    public GameObject antiGazObj;
    [Space]
    private Transform gazSpawnPosL;
    private Transform gazSpawnPosR;
    public GameObject gazPref;

    [Space]

    private bool gazSpawned = false;
    public bool wrenchState = false;
    public bool antiGazState = false;
    public bool dangerZoneExit = false;
    public bool zaglState = false;
    [Space]
 
    public GameObject zaglBtn;
    public GameObject zaglObj;
    [Space]
    public GameObject zaglPrefL;
    public GameObject zaglPrefR;
    [Space]
    public AudioSource groundClip;
    public AudioSource stairsClip;
    public AudioSource ItemClip;
    public AudioSource AntiGazClip;
    [Space]
    public GameObject boltGameObject;
    public CameraOutline camOutline;

    bool firstTake = true;


    void Start()
    {
        wrenchButton = GameObject.Find("Mednoe");
        wrenchObj = GameObject.Find("WrenchObj");

        gazSpawnPosL = GameObject.Find("GazLeft").transform;
        gazSpawnPosR = GameObject.Find("GazRight").transform;

        antiGazBtn = GameObject.Find("AntiGazBtn").gameObject;

        antiGazObj.SetActive(false);

        zaglPrefL.SetActive(false);
        zaglPrefR.SetActive(false);

        groundClip = GameObject.Find("TerrainAudio").GetComponent<AudioSource>();
        stairsClip = GameObject.Find("StairsAudio").GetComponent<AudioSource>();
    }

    void Update()
    {
        wrenchObj.SetActive(wrenchState);
        antiGazBtn.SetActive(antiGazState);
    }
        
    

    public void WrenchClickBtn()
    {
        wrenchState = !wrenchState;
        wrenchButton.SetActive(false);

        zaglBtn.SetActive(true);
        zaglState = false;
        zaglObj.SetActive(false);
        ItemClip.Play();
        if (firstTake)
        {
            GameObject.Find("Zagl_13").GetComponent<Zagl_13>().changeValue();
            firstTake = false;
        }
    }

    public void AntiGazClickBtn()
    {
        GameObject.Find("Ph-1").GetComponent<Ph_1>().changeValue();
        antiGazState = false;
        antiGazObj.SetActive(true);
        Destroy(GameObject.Find("GazTrig(Clone)").gameObject);
        Debug.Log("Противогаз");
        AntiGazClip.Play();
    }

    public void ZaglClickBtn()
    {
        zaglState = true;
        zaglBtn.SetActive(false);
        zaglObj.SetActive(true);
        wrenchButton.SetActive(true);
        wrenchState = false;
        ItemClip.Play();
        string storona = boltGameObject.GetComponent<BoltController>().storona;
        camOutline.zaglState = true;


        if (storona == "R")
        {
            zaglPrefR.SetActive(true);
            Debug.Log("Заглушка правая заспавнилась");
        }
        else if (storona == "L")
        {
            zaglPrefL.SetActive(true);
            Debug.Log("Заглушка левая заспавнилась");
        }
    }

        //public void OnExitBtn()
        //{
        //    Application.Quit();
        //}

/*
    public void gazSpawn(string pos)
    {
        if(pos == "L" && !gazSpawned)
        {
            Instantiate(gazPref, gazSpawnPosL.position, gazSpawnPosL.rotation);
            gazSpawned = true;
        }
        else if (pos == "R" && !gazSpawned)
        {
            Instantiate(gazPref, gazSpawnPosR.position, gazSpawnPosR.rotation);
            gazSpawned = true;
        }

        GameObject.Find("GazZone01").GetComponent<GaZZone01>().gazAnalizator = true;
       // GameObject.Find("GazZone01").GetComponent<GaZZone01>().GazEnabled = true;
    }
*/
}
