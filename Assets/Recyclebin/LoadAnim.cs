using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAnim : MonoBehaviour
{
    public void PlayAnim()
    {
        this.GetComponent<Animator>().Play("LoadBG");
        this.gameObject.SetActive(true);
    }

    public void ChangeScene()
    {
        GameObject.Find("Main Camera").GetComponent<MenuManager>().SavePrefs();
    }
}
