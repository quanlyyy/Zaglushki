using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//скрипт Ё ћ дл€ вывода значени€ прибора типа AI в стрелочном виде со стрелками уставок max и min
public class TEST : MonoBehaviour
{
    MainScript mainScript; //ссылка на главный скрипт - дл€ св€зи с его данными

    //эти переменные заполн€ютс€ при загрузке файла в главном скрипте
    public int deviceNum;     // индекс текущего прибора в главном списке 
    public string Name_prib;  // им€ прибора
    public float VMin;        // минимальное аварийное значение
    public float VMax;        // максимальное аварийное значение
    public float SMin;        // минимальное значение шкалы
    public float SMax;        // максимальное значение шкалы
    public string Ed_izm;     // единица измерени€

    // вспомогательные переменные дл€ расчЄта
    private float smesh, ugol, param_smesh, koeff, min_smesh, max_smesh, param_vich;

    public Transform Pribor_strel;      // ссылка на чЄрную стрелку
    public Transform Pribor_strel_min;  // ссылка на синюю стрелку
    public Transform Pribor_strel_max;  // ссылка на красную стрелку
    TextMesh NamePribor;                // им€ прибора
    TextMesh PVValue;                   // дублирующее цифровое значение

    // инициализаци€ при старте скрипта
    void Start()
    {
        smesh = -49.5f;  // начальное смещение стрелок
        ugol = 360f;     // круговой циферблат
        koeff = 1.37f;   // подстроечный коэффициент дл€ подгонки стрелочных показаний

        //получаем ссылку на главный скрипт
        mainScript = GameObject.Find("GameObject").GetComponent<MainScript>();
        //получаем ссылку на им€ прибора
        NamePribor = transform.GetComponentsInChildren<TextMesh>()[0];
        NamePribor.text = Name_prib;
        //получаем ссылку на дублирующее цифровое табло
        PVValue = transform.GetComponentsInChildren<TextMesh>()[1];

        // повернЄм синюю стрелку до заданного значени€ в VMin
        min_smesh = ((ugol * VMin) / ((SMax - SMin) * koeff)) - smesh;
        Pribor_strel_min.localRotation = Quaternion.Euler(0f, 0f, min_smesh);

        // повернЄм красную стрелку до заданного значени€ в VMax
        max_smesh = ((ugol * VMax) / ((SMax - SMin) * koeff)) - smesh;
        Pribor_strel_max.localRotation = Quaternion.Euler(0f, 0f, max_smesh);

        //выводим данные
        UpdateTextPanel();
    }

    //вызываетс€ каждый кадр - по умолчанию 60 раз в секунду
    void Update()
    {
        //выводим данные
        UpdateTextPanel();
    }

    public void UpdateTextPanel() // функци€ вывода данных
    {
        // считаем текущее значение тегов прибора типа AI - один тег с суффиксом PV
        MainScript.AI st = (MainScript.AI)mainScript.deviceListServer[deviceNum].device;

        // ограничим движение чЄрной стрелки в пределах шкалы прибора
        if (st.PV < SMin) param_vich = SMin;
        else if (st.PV > SMax) param_vich = SMax;
        else param_vich = st.PV;

        // повернЄм чЄрную стрелку до текущего показани€
        param_smesh = ((ugol * param_vich) / ((SMax - SMin) * koeff)) - smesh;
        Pribor_strel.localRotation = Quaternion.Euler(0f, 0f, param_smesh);

        // дополнительное цифровое значение (в реальном стрелочном Ё ћ отсутствует)
        PVValue.text = Math.Round(st.PV, 2).ToString() + " " + Ed_izm;

        this.GetComponent<Text>().text = st.PV.ToString() + " " + Ed_izm;
    }
}
