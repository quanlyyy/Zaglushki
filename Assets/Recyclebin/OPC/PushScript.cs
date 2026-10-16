using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushScript : MonoBehaviour
{

    [SerializeField]
    // MainScript mainScript; // ссылка на главный скрипт - для связи с его данными
    Camera mainCamera;     // это главная камера - нужна тут, чтобы пускать из нее лучи в сторону курсора мыши и понимать, куда нажимает пользователь
                           //TextMesh NamePribor;   // ссылка на текстовое поле

    //эти переменные заполняются при загрузке файла в главном скрипте
    /* public int deviceNum;     // индекс текущего прибора в главном списке 
     public string Name_prib;  // имя прибора
     public string Color_off;  // цвет при значении 0
     public string Color_on;   // цвет при значении 1
    */
    bool zapisal; //признак того, что уже передали на сервер
    public string LeftOrRight;
    public bool wrenchState;

    BoltController left;
    BoltController right;

    //public GameObject bolt_gameObj;

    // инициализация при старте скрипта
    void Start()
    {
        string test = "1L";

        Debug.Log(test.Substring(1));

        //получаем ссылку на камеру
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        /*
        //получаем ссылку на главный скрипт
        mainScript = GameObject.Find("GameObject").GetComponent<MainScript>();

        // получаем ссылку на текстовое поле
        NamePribor = transform.GetComponentsInChildren<TextMesh>()[0];

        // запишем в текстовое поле значение глобальной переменной Name_prib
        NamePribor.text = Name_prib;*/
        left = GameObject.Find("Left").GetComponent<BoltController>();
        right = GameObject.Find("Right").GetComponent<BoltController>();

        zapisal = false; // ещё не передавали на сервер

        //bolt_gameObj = GameObject.Find("Bolts");
    }

    // вызывается каждый кадр - по умолчанию 60 раз в секунду
    void Update()
    {
        wrenchState = GameObject.Find("Player").GetComponent<Player>().wrenchState;
        /*
        //получим данные прибора типа DI5
        MainScript.DI5 st = (MainScript.DI5)mainScript.deviceListServer[deviceNum].device;
        // если значение тега равно нулю
        if (st.SET == 0)
        {
            GetComponent<MeshRenderer>().material.color = Color.grey; // установим серый цвет вентилю
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.cyan; // иначе цвет будет циановый
        }
        */
        // проверим было ли нажатие кнопки мышки?
        if (Input.GetMouseButton(0) && wrenchState && !zapisal) // нажали левую кнопку мышки на выбранном объекте
        {

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // бросаем из главной камеры луч в направлении мышки на экране
            RaycastHit hit = new RaycastHit(); // получаем данные объекта сцены, с которым пересекся луч (для этого у объекта должен быть коллайдер)

            if (Physics.Raycast(ray, out hit)) // выполняется если попали лучом в текущий объект сцены 
            {
                if (hit.transform.gameObject.tag == "Bolts") {
                    zapisal = true;

                    if (hit.transform.name.Substring(1) == "L")
                    {
                        left.boltL += 1;
   
                    }
                    else if (hit.transform.name.Substring(1) == "R")
                    {
                        right.boltR += 1;

                    }
                    else
                    {
                        Debug.LogError("Не корректная сторона");
                    }
                }
            }
        }
        else zapisal = false; // сбросим признак обработки нажатия кнопки мышки

    }


}
