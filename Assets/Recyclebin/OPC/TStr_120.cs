using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TStr_120 : MonoBehaviour
{
    MainScript mainScript; //ссылка на главный скрипт - для связи с его данными

    //эти переменные заполняются при загрузке файла в главном скрипте
    public int deviceNum; //индекс текущего прибора в главном списке 
    public string Name_prib;
    public float VMin;
    public float VMax;
    public float rotateSpeed = 0.1f;

    private float param_min, param_max, smesh, ugol, param_smesh, koeff;
    private float prom, param_vich;
    public Transform Pribor_strel;
    TextMesh NamePribor; //

    // Start is called before the first frame update
    void Start()
    {
        param_min = 0f;
        param_max = 120.0f;
        smesh = - 44.5f; // 130.5f;
        ugol = 360f;
        koeff = 1.3f;

        //получаем ссылку на главный скрипт
        mainScript = GameObject.Find("GameObject").GetComponent<MainScript>();

        NamePribor = transform.GetComponentsInChildren<TextMesh>()[0];
        NamePribor.text = Name_prib;
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

        if (st.PV < param_min) param_vich = param_min;
        else if (st.PV > param_max) param_vich = param_max;
        else param_vich = st.PV;

        param_smesh = ((ugol * param_vich) / ((param_max - param_min) * koeff)) - smesh;

        Pribor_strel.localRotation = Quaternion.Euler(0f, 0f, param_smesh);
    }
}
