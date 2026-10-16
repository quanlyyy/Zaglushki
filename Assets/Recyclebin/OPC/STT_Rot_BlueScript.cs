using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class STT_Rot_BlueScript : MonoBehaviour
{
    MainScript mainScript; //ссылка на главный скрипт - для связи с его данными

    //эти переменные заполняются при загрузке файла в главном скрипте
    public string Color_off;   // на будущее - брать из OPCS.xlsx
    public string Color_on;    // на будущее - брать из OPCS.xlsx
    public int deviceNum;      //индекс текущего прибора в главном списке приборов из сервера 

    bool proc = false;  // признак действия паузы
    int pausa = 0;      // счётчик паузы

    // Start is called before the first frame update
    void Start()
    {
        //получаем ссылку на главный скрипт
        mainScript = GameObject.Find("GameObject").GetComponent<MainScript>();
    }

    void Update()
    {
        //получим данные прибора типа N12
        MainScript.N12 st = (MainScript.N12)mainScript.deviceListServer[deviceNum].device;
 
        // изменим цвет пропеллера в зависимости от состояния насоса
        if (st.STT == 0)
        {
            GetComponent<MeshRenderer>().material.color = Color.gray;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;
            if (proc == false)  // это происходит в первый раз после паузы
            {
             transform.Rotate(0, 0, 90);
            }
            else
            {
             transform.Rotate(0, 0, 0);
            }
        }

        if (proc == true) // отсчёт длительности паузы в 100 кадров
        {
            pausa++;
            if (pausa > 100) { pausa = 0; if (proc == false) proc = true; else proc = false;} // можно снова открывать/закрывать дверцу
        }

    }

}
