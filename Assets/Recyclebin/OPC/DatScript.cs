using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatScript : MonoBehaviour
{
    
    MainScript mainScript; //ссылка на главный скрипт - для связи с его данными
    GameObject plafon;

    //эти переменные заполняются при загрузке файла в главном скрипте
    public string Name_prib;
    public string Color_off;
    public string Color_on;
    public int deviceNum; //индекс текущего прибора в главном списке 
  //  TextMesh NamePribor; //

    // Start is called before the first frame update
    void Start()
    {
       
        //получаем ссылку на главный скрипт
        mainScript = GameObject.Find("GameObject").GetComponent<MainScript>();

    }

    // Update is called once per frame
    void Update()
    {
        //получим данные прибора
        MainScript.DI st = (MainScript.DI)mainScript.deviceListServer[deviceNum].device;
        
        plafon = transform.GetChild(0).gameObject;
      
        if (st.STTPAZ == 0)  plafon.GetComponent<Image>().color = Color.grey;
        else                 plafon.GetComponent<Image>().color = Color.red;
      
    }
}
