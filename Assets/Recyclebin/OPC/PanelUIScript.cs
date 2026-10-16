using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// скрипт панели UI
public class PanelUIScript : MonoBehaviour
{
    public GameObject owner; // объект сцены, который открыл панель - хозяин панели
    public string synonym;   // синоним прибора, который открыл панель
    MainScript mainScript;   // ссылка на главный скрипт - для связи с его данными
    DateTime lastUpdate;     // метка времени последнего измения на сервере
    
    public float health;     // глобальная переменная высоты
    public Slider slider_m;  // ссылка на дочерний слайдер % открытия клапана
    public Slider slider_v;  // ссылка на дочерний слайдер текущего значения регулируемого парамтера

    public int deviceNum;    // индекс текущего прибора в главном списке
    
    public Sprite A_off;     // ссылка на картинку выключенного АВТОМАТИЧЕСКОГО режима - накладка на кнопку А
    public Sprite A_on;      // ссылка на картинку включенного АВТОМАТИЧЕСКОГО режима - накладка на кнопку А
    public Sprite M_off;     // ссылка на картинку выключенного РУЧНОГО режима - накладка на кнопку М
    public Sprite M_on;      // ссылка на картинку включенного РУЧНОГО режима - накладка на кнопку М

    public float VMax_ui;    // максимальное значение шкалы регулируемого параметра
    public float VMin_ui;    // миксимальное значение шкалы регулируемого параметра

    Button Mode_A, Mode_M;   // кнопки UI А и М

    Text componentName;      // ссылка на имя прибора
    Text componentMode_A;    // ссылка на кнопку А
    Text componentMode_M;    // ссылка на кнопку М
    Text componentPV;        // ссылка на текст с текущим значением
    Text componentMV;        // ссылка на текст % открытия
    Text componentSV;        // ссылка на текст уставки регулированя
    Text componentCLS;       // ссылка на кнопку закрытия панели

    // Инициализация при старте скрипта
    void Start()
    {   
        // проверка открытия панели галочкой редакторе без хозяина -
        // после редактирования панели перед её запуском галочка должна быть снята!
        if (owner == null) throw (new Exception("UI панель можно открывать только из объекта-владельца"));
                
        //получаем ссылку на главный скрипт
        mainScript = GameObject.Find("GameObject").GetComponent<MainScript>();

        //получаем необходимые компоненты дочерних объектов - важен их порядок, так как обращаемся к ним просто по номерам
        componentName = transform.GetChild(0).GetComponent<Text>();
        componentMode_A = transform.GetChild(1).GetComponentInChildren<Text>();
        componentMode_M = transform.GetChild(2).GetComponentInChildren<Text>();
        componentPV = transform.GetChild(3).GetComponentInChildren<Text>();
        componentMV = transform.GetChild(4).GetComponentInChildren<Text>();
        componentSV = transform.GetChild(5).GetComponentInChildren<Text>();

        // получим ссылку на кнопки А и М и наложим на них начальные картинки режима работы
        Mode_A = transform.GetChild(1).GetComponent<Button>();
        Mode_A.image.sprite = A_off;
        Mode_M = transform.GetChild(2).GetComponent<Button>();
        Mode_A.image.sprite = M_off;

        // зададим размеры шкал для слайдеров
        slider_v.minValue = VMin_ui;
        slider_v.maxValue = VMax_ui;
        

    }

    // вызывается каждый кадр - по умолчанию 60 раз в секунду
    void Update()
    {
        UpdatePanelUI();     // циклически обновляем панель текущими данными
    }

    public void UpdatePanelUI()  // функция вывода данных на панель UI 
    {
        // получаем данные по прибору PID01
        MainScript.PID01 st = (MainScript.PID01)mainScript.deviceListServer[deviceNum].device; 
                
        componentName.text = synonym; // выводим на панель синоним прибора
        
        // изменим индикацию текущего режима работы
        if (st.MODE == 0)
        {
            componentMode_A.text = "Ручной";
            componentMode_M.text = "Ручной";
            Mode_A.image.sprite = A_off;
            Mode_M.image.sprite = M_on;
        }
        else
        {
            componentMode_A.text = "Авто";
            componentMode_M.text = "Авто";
            Mode_A.image.sprite = A_on;
            Mode_M.image.sprite = M_off;
        }
        // выведем текущее значение регулируемого параметра
        componentPV.text = Math.Round(st.PV, 1).ToString();  // в строку
        slider_v.value = (float)Math.Round(st.PV, 1);        // на слайдер
        // выведем текущее значение % открытия клапана
        componentMV.text = Math.Round(st.MV, 1).ToString();  // в строку
        slider_m.value = (float)Math.Round(st.MV, 1);        // на слайдер
        // выведем значение уставки регулируемого параметра                                                    
        componentSV.text = Math.Round(st.SV, 1).ToString();  // в строку
        
        // установим доступность полей MV и SV в зависимости от текущего режима работы
        transform.GetChild(4).GetComponent<InputField>().interactable = (st.MODE == 0); // ввод MV доступен только в ручном режиме
        transform.GetChild(5).GetComponent<InputField>().interactable = (st.MODE == 1); // ввод SV доступен только в автоматическом режиме

        lastUpdate = DateTime.Now;  // обновим метку времени
    }

    public void Close() //закрываем текущую панель
    {
        gameObject.SetActive(false);  // сброс галочки доступности в объекте PanelUI
    }

    public void ModeButton() // функция переключения режима работы
    {
        MainScript.PID01 st = (MainScript.PID01)mainScript.deviceListServer[deviceNum].device; // получим старые данные по текущему режиму работы
        st.MODE = (st.MODE + 1) % 2;                                                           // переключим режим - меняем 1 на 0 и обратно
        mainScript.deviceListClient[deviceNum].device = st;                                    // записываем на сервер новые данные по регулятору
        mainScript.deviceListClient[deviceNum].time = DateTime.Now;              // обновляем метку времени, чтобы изменения передались на сервер
        
        // обращаемся к скрипту хозяина и обновляем значения на основной панели регулятора
        owner.GetComponent<KRScript>().UpdateTextPanel(); 

        // изменим индикацию кнопок панели UI
        if (st.MODE == 0)   // ручной режим
        { 
            Mode_A.image.sprite = A_off;
            Mode_A.interactable = true;
            Mode_M.image.sprite = M_on;
            Mode_M.interactable = false;
        }
        else               // автоматический режим
        { 
            Mode_A.image.sprite = A_on;
            Mode_A.interactable = false;
            Mode_M.image.sprite = M_off;                                                                         
            Mode_M.interactable = true;
        }

        }

    public void EndEditSV()   // функция по завершении корректировки поля уставки регулятора
    {
        MainScript.PID01 st = (MainScript.PID01)mainScript.deviceListServer[deviceNum].device; // получим с сервера старые данные по прибоу PID01
        st.SV = float.Parse(transform.GetChild(5).GetComponentInChildren<InputField>().text);  // обновим значение из поля ввода панели
        mainScript.deviceListClient[deviceNum].device = st;                            // записываем в список приборов новые данные по регулятору
        mainScript.deviceListClient[deviceNum].time = DateTime.Now;                    // обновляем метку времени, чтобы передалось на сервер
        transform.GetChild(5).GetComponent<InputField>().text = "";  // сбросим поле ввода, чтобы в текстовое поле выводилось значение из сервера
    }

    public void EndEditMV()  // функция по завершении корректировки % открытия клапана
    {
        MainScript.PID01 st = (MainScript.PID01)mainScript.deviceListServer[deviceNum].device; // получим с сервера старые данные по прибоу PID01
        st.MV = float.Parse(transform.GetChild(4).GetComponentInChildren<InputField>().text);  // обновим значение из поля ввода панели
        mainScript.deviceListClient[deviceNum].device = st;                            // записываем в список приборов новые данные по регулятору
        mainScript.deviceListClient[deviceNum].time = DateTime.Now;                    // обновляем метку времени, чтобы передалось на сервер
        transform.GetChild(4).GetComponent<InputField>().text = "";  // сбросим поле ввода, чтобы в текстовое поле выводилось значение из сервера
    }
}
