using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    [SerializeField]
    Player player;

    [SerializeField]
    AudioSource currentFloor;

    public AudioSource groundClip;
    public AudioSource stairsClip;
    public bool isStairs = false;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();

        groundClip = GameObject.Find("TerrainAudio").GetComponent<AudioSource>();
        stairsClip = GameObject.Find("StairsAudio").GetComponent<AudioSource>();
        currentFloor = groundClip;
        stairsClip.mute = true;
    }

    void Update()
    {
        changeVolume();
        currentFloor.volume = Input.GetAxis("Vertical");
        currentFloor.mute = false;

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5))
        {
            if (hit.transform.gameObject.tag == "Terrain")
            {
                isStairs = false;
            }
            else if (hit.transform.gameObject.tag == "Stairs")
            {
                isStairs = true;
            }
        }
    }


    private void changeVolume()
    {
        if (floorType(isStairs) == "StairsAudio")
        {
            currentFloor = stairsClip;
            groundClip.mute = true;
        }
        else if (floorType(isStairs) == "TerrainAudio")
        {
            currentFloor = groundClip;
            stairsClip.mute = true;
        }
    }

    public string floorType(bool isStairs) => isStairs ? "StairsAudio" : "TerrainAudio";
}
