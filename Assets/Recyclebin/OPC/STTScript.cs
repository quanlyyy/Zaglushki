using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class STTScript : MonoBehaviour
{
    MainScript mainScript; //ссылка на главный скрипт - для связи с его данными
    GameObject plafon;

    Transform propel;        //

    TextMesh NamePribor; // текстовое поле имени прибора

    //эти переменные заполняются при загрузке файла в главном скрипте
    public string Name_prib;   // имя прибора возьмём из OPCS.xlsx
    public string Color_off;   // на будущее - брать из OPCS.xlsx
    public string Color_on;    // на будущее - брать из OPCS.xlsx
    public int deviceNum;      //индекс текущего прибора в главном списке приборов из сервера 

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
 
        plafon = transform.GetChild(0).gameObject; // обновим состояние прибора

        // изменим цвет плафона в зависимости от состояния насоса
        if (st.STT == 0)
        {
            plafon.GetComponent<MeshRenderer>().material.color = Color.grey;
        }
        else
        {
            plafon.GetComponent<MeshRenderer>().material.color = Color.green;
        }

    }

}
