using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvScript : MonoBehaviour
{
    Camera mainCamera; //это главная камера - нужна тут, чтобы пускать из нее лучи в сторону курсора мыши и понимать, куда нажимает пользователь
    MainScript mainScript; //ссылка на главный скрипт - для связи с его данными
    Transform panel; //панель управления

    //эти переменные заполняются при загрузке файла в главном скрипте
    public int deviceNum; //индекс текущего прибора в главном списке 
    public string Name_prib;
    public string Ed_izm;
    public float VMin;
    public float VMax;
    public float rotateSpeed = 0.1f;

    //компоненты панели управления
    TextMesh componentPVName;
    TextMesh componentPVText;

    // Start is called before the first frame update
    void Start()
    {
        //получаем камеру
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();


        //получаем ссылку на главный скрипт
        mainScript = GameObject.Find("GameObject").GetComponent<MainScript>();


        //получаем панель управления на регуляторе
        panel = transform.Find("Panel");

        //получаем компоненты панели управления - важен их порядок, так как обращаемся к ним по номерам
        componentPVName = panel.GetChild(0).GetComponent<TextMesh>();
        componentPVText = panel.GetChild(1).GetComponent<TextMesh>();

        //выводим данные на панель управления
        UpdateTextPanel();
    }

    // Update is called once per frame
    void Update()
    {
        //выводим данные на панель
        UpdateTextPanel();
    }

    public void UpdateTextPanel() //выводим данные на панель - важен порядок её детей, так как обращаемся к ним просто по номерам
    {
        MainScript.AI st = (MainScript.AI)mainScript.deviceListServer[deviceNum].device;

        componentPVName.text = Name_prib;
        componentPVText.text = Math.Round(st.PV, 1).ToString() + " " + Ed_izm;
    }

}
