using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltInstall_3 : MonoBehaviour
{
    public string NamePrib;
    public int deviceNum;
    MainScript mainScript;

    public void Start()
    {
        mainScript = GameObject.Find("GameObjectOPC").GetComponent<MainScript>();
    }

    public void changeValue()
    {
        MainScript.DI5 st = (MainScript.DI5)mainScript.deviceListServer[deviceNum].device;
        st.SET = 1;
        mainScript.deviceListClient[deviceNum].device = st;
        mainScript.deviceListClient[deviceNum].time = DateTime.Now;
    }
}
