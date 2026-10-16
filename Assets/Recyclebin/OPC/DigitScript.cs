using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

//скрипт занимается управлением крутилками - его надо добавлять во все такие объекты сцены. Если понадобятся объекты других типов - то для них нужно будет делать скрипты наподобии этого
public class DigitScript : MonoBehaviour
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
    TextMesh componentPVText;
    TextMesh componentPVName;

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
        componentPVText = panel.GetChild(0).GetComponent<TextMesh>();
        componentPVName = panel.GetChild(1).GetComponent<TextMesh>();

        //выводим данные на панель управления
        UpdateTextPanel();
    }

    // Update is called once per frame
    void Update() //вызывается каждый кадр - по умолчанию 60 раз в секунду
    {
        
       //выводим данные на панель
        UpdateTextPanel();
    }

    public void UpdateTextPanel() //выводим данные на панель - важен порядок её детей, так как обращаемся к ним просто по номерам
    {        
        MainScript.AI st = (MainScript.AI)mainScript.deviceListServer[deviceNum].device;

        componentPVText.text = Math.Round(st.PV, 2).ToString() + " " + Ed_izm;
        componentPVName.text = Name_prib + " =";
    }
}
