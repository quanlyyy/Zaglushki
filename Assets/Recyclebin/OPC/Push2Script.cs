using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push2Script : MonoBehaviour
{
    MainScript mainScript; //ссылка на главный скрипт - для связи с его данными
    Camera mainCamera;     //это главная камера - нужна тут, чтобы пускать из нее лучи в сторону курсора мыши и понимать, куда нажимает пользователь
    Transform stop;        //кнопка стоп
    Transform pusk;        //кнопка пуск

    TextMesh NamePribor; // текстовое поле имени прибора

    //эти переменные заполняются при загрузке файла в главном скрипте
    public int deviceNum; //индекс текущего прибора в главном списке 
    public string Name_prib;
    public string Color_off;
    public string Color_on;

    bool zapisal; //признак того, что уже передали на сервер

    // Start is called before the first frame update
    void Start()
    {
        //получаем камеру
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        //получаем ссылку на главный скрипт
        mainScript = GameObject.Find("GameObject").GetComponent<MainScript>();
        
        //получаем панель управления на регуляторе
        stop = transform.Find("Otkachka_stop");
        pusk = transform.Find("Otkachka_pusk");
 
        NamePribor = transform.GetComponentsInChildren<TextMesh>()[0];
        NamePribor.text = Name_prib;


        zapisal = false; //признак того, что ещё не передавали на сервер 
    }

    // Update is called once per frame
    void Update()
    {
        //получим данные прибора
        MainScript.N12 st = (MainScript.N12)mainScript.deviceListServer[deviceNum].device;
        if (st.STT == 0) 
        { 
            GetComponent<MeshRenderer>().material.color = Color.grey;
        }
        else 
        { 
            GetComponent<MeshRenderer>().material.color = Color.cyan;
        }

        //опросим пользователя
        if (Input.GetMouseButton(0)) //зажали левую кнопку мыши на выбранном объекте
        {
            if (!zapisal)  // выполняем один раз после нового нажатия на объекте левой кнопки мыши
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); //бросаем из главной камеры луч в направлении мыши на экране
                RaycastHit hit = new RaycastHit(); //получаем данные объекта сцены, с которым пересекся луч (для этого у объекта должен быть коллайдер)
                if (Physics.Raycast(ray, out hit))
                 {
                    if (hit.transform == stop) //выполняется если попали лучом в кнопку стоп 
                    {
                        st.CMDSTP = 1;
                        mainScript.deviceListClient[deviceNum].device = st; //сохраняем
                        mainScript.deviceListClient[deviceNum].time = DateTime.Now; //время обновления - чтобы передалось на сервер 
                        zapisal = true; //ставим признак, что уже записали - не проверяем пока нажата левая кнопка мышки
                    }
                    else if (hit.transform == pusk) // выполняется если попали лучом в кнопку пуск
                    {
                        st.CMDON = 1;
                        mainScript.deviceListClient[deviceNum].device = st; //сохраняем
                        mainScript.deviceListClient[deviceNum].time = DateTime.Now; //время обновления - чтобы передалось на сервер 
                        zapisal = true; //ставим признак, что уже записали - не проверяем пока нажата левая кнопка мышки
                    }
                }
            }
        }
        else zapisal = false; //признак того, что ещё не нажимали на кнопку и передавали на сервер

    }

}
