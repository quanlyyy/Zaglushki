using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Zagl_13 : MonoBehaviour
{
    MainScript mainScript; // ссылка на главный скрипт - для связи с его данными
    

    //эти переменные заполняются при загрузке файла в главном скрипте
    public int deviceNum;     // индекс текущего прибора в главном списке 
    public string NamePrib;  // имя прибора
    

    bool zapisal = false; //признак того, что уже передали на сервер

    // инициализация при старте скрипта
    void Start()
    {
      
        //получаем ссылку на главный скрипт
        mainScript = GameObject.Find("GameObjectOPC").GetComponent<MainScript>();

        zapisal = false; // ещё не передавали на сервер
    }

    // вызывается каждый кадр - по умолчанию 60 раз в секунду
    void Update()
    {
        

    }

    public void changeValue()
    {
        MainScript.DI5 st = (MainScript.DI5)mainScript.deviceListServer[deviceNum].device;
        st.SET = 1;
        mainScript.deviceListClient[deviceNum].device = st;
        mainScript.deviceListClient[deviceNum].time = DateTime.Now;
    }
}
