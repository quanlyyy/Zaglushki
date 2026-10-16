using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

//скрипт для управления клапано - регулятором со своим ручным штурвалом (рукояткой) и панелью индикации и управления
public class KRScript : MonoBehaviour
{
    Camera mainCamera;        //это главная камера - нужна тут, чтобы пускать из нее лучи в сторону курсора мыши и понимать,
                              //куда нажимает пользователь
    Transform movingPart;     //дочерний объект, который будем крутить этим скриптом на относительное вращение следующего объекта
    Transform fakeMovingPart; //дочерний объект, который будем крутить этим скриптом для получения относительного вращения
    MainScript mainScript;    //ссылка на главный скрипт - для связи с его данными
    Transform panel;          //панель индикации и управления

    //эти переменные заполняются при загрузке файла в главном скрипте
    public int deviceNum;            //индекс текущего прибора в главном списке 
    public string Name_prib;         // наименование прибора
    public string Ed_izm;            // единица измерения регулируемого параметра
    public float VMin;               // минимальное значение шкалы параметра
    public float VMax;               // максимальное значение шкалы параметра
    public float rotateSpeed = 0.1f; // скорость вращение штурвала

    float curMV; // будем хранить текущее MV прямо тут в объекте - и сравнивать его с тем, что приходит с сервера,
                 // чтобы определять дельту и поворачивать штурвал автоматически
    DateTime lastTouchTime; // будем хранить время последнего касания штурвала и не пускать сервер к управлению
                            // несколько секунд после касания (чтобы не дергалось туда-сюда)
    
    //ссылки на компоненты панели управления
    TextMesh componentButtonText;     // на кнопку
    MeshRenderer componentButtonMesh; // на наименование режима работы регулятора
    TextMesh componentPVText;         // на текущее сзначение регулируемого параметра
    TextMesh componentMVText;         // на текущих процент открытия клапана
    TextMesh componentSVText;         // на уставку регулирования параметра
    TextMesh NameKR;                  // на имя прибора

   
    // Инициализация при старте скрипта
    void Start()
    {        
        //получаем ссылку на камеру
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        //получаем дочерний объект, который будем двигать этим скриптом
        movingPart = transform.Find("Gate Valve"); // штурвал
        fakeMovingPart = transform.Find("Tube");   // скрытый пустой объект, относительно которого происходит вращение штурвала
                     
        //получаем ссылку на главный скрипт
        mainScript = GameObject.Find("GameObject").GetComponent<MainScript>();

                    
        //получаем ссылку на собственную (первичную) панель управления регулятора
        panel = transform.Find("Panel");

        //получаем компоненты панели управления - важен их порядок размещения в структуре, так как обращаемся к ним по номерам
        componentButtonText = panel.GetChild(0).GetComponentInChildren<TextMesh>();     // кнопка
        componentButtonMesh = panel.GetChild(0).GetComponentInChildren<MeshRenderer>(); // имя режима работы
        componentPVText = panel.GetChild(1).GetComponent<TextMesh>();                   // текущее значение
        componentMVText = panel.GetChild(2).GetComponent<TextMesh>();                   // % открытия клапана
        componentSVText = panel.GetChild(3).GetComponent<TextMesh>();                   // уставка регулирования
        NameKR = panel.GetChild(4).GetComponent<TextMesh>();                            // имя прибора

        //выводим текущие данные на панель управления
        UpdateTextPanel();
    }

    //вызывается каждый кадр - по умолчанию 60 раз в секунду
    void Update() 
    {
        float delta = 0;       // рассогласование положения штурвала - сколько его докручивать
        bool manual = false;   // признак того, что в этот такт штурвал крутили руками

        //получим с сервера текущие данные прибора
        MainScript.PID01 st = (MainScript.PID01)mainScript.deviceListServer[deviceNum].device;
                
        // обработаем нажатие левой кнопки мышки на штурвале
        if (Input.GetMouseButton(0)) // нажали левую кнопку мышки ?
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // иогда бросаем из главной камеры луч в направлении мышки на экране
            RaycastHit hit = new RaycastHit();  // получаем данные объекта сцены, с которым пересекся луч (для этого у объекта должен быть коллайдер)
            if (Physics.Raycast(ray, out hit))
            {
               if (hit.transform == transform && st.MODE == 0) // выполняется если попали лучом в штурвал и режим ручной, т.е. можно крутить вручную 
               {
                    // тут вращение основного объекта мы определяем через вращение дополнительного (так как надо перейти от абсолютного угла к относительному)
                    float oldY = fakeMovingPart.localEulerAngles.y; // старый угол дополнительного объекта - сохраняем в переменную, чтобы сравнить с новым
                    fakeMovingPart.LookAt(hit.point); // главное всего расчета - этой операцией наш дополнительный объект поворачивается в сторону касания мыши
                                                      // (и не надо считать никакие тангенсы)
                    float newY = fakeMovingPart.localEulerAngles.y; // новый угол дополнительного вращения (после поворота)
                    fakeMovingPart.localEulerAngles = new Vector3(0, newY, 0); // обнуляем поворот по всем осям кроме Y - иначе поворачиваться будет не только в одной плоскости.
                    if (oldY > 300 && newY < 60) delta = newY + 360 - oldY;    // проверяем на переход угла через 360 градусов -
                                                                               // после них следует 0 и без этих поправок штурвал начинает дергаться
                    else if (oldY < 60 && newY > 300) delta = newY - 360 - oldY; // и в другую сторону
                    else delta = newY - oldY;                                    // и если не перешло через 360
                    delta /= 20;                                                 // делим полученную дельту на 20 - тут опционально
                    lastTouchTime = DateTime.Now;                                // запоминаем последнее время касания мышки
                    manual = true;                                               // ставим признак, что управляли руками                                
                }
                else if (hit.transform == panel) // открываем UI панель, если нажали на первичную панель управления
                {
                    GameObject panelUI = GameObject.Find("Canvas").transform.GetChild(0).gameObject;  // находим объект PanelUI - вторичную панель управления регулятора
                    // if (panelUI != null) print("panelUI = " + panelUI);       // использовалось при отладке
                    panelUI.GetComponent<PanelUIScript>().owner = gameObject;    // передаем в панель текущий объект сцены - чтобы она знала, с каким прибором работает
                    panelUI.GetComponent<PanelUIScript>().deviceNum = deviceNum; // передаем индекс текущего прибора из главного списка
                    panelUI.GetComponent<PanelUIScript>().synonym = Name_prib;   // передаем в панель синоним имени регулятора
                    panelUI.GetComponent<PanelUIScript>().VMin_ui = VMin;        // передаём в панель минимальное значение шкалы параметра
                    panelUI.GetComponent<PanelUIScript>().VMax_ui = VMax;        // передаём в панель максимальное значение шкалы параметра

                    panelUI.SetActive(true); // теперь всё подготовлено для включения вторичной панели управления

                    GameObject slider_Obj = GameObject.Find("Slider_V");   // находим объект слайдер
                    Slider slider = slider_Obj.GetComponent<Slider>();     // получаем его компонент слайдер
                    slider.minValue = VMin;                                // заполняем пределы шкалы слайдера
                    slider.maxValue = VMax;                                // заполняем пределы шкалы слайдера
                }
            }
        }

        if (lastTouchTime.AddSeconds(1) < DateTime.Now && mainScript.autoRotate) // если штурвал не крутили руками последнюю секунду и включено автовращение -
                                                                                 // то крутим автоматом с сервера 
        {
            delta = curMV - st.MV;                               // автоматически довернём штурвал в сторону MV на сервере
            if (delta > 0) delta = Math.Min(delta, rotateSpeed); // ограничим скорость вращения штурвала
            else delta = Math.Max(delta, -rotateSpeed);
        }                 
        
        if (delta != 0)  // надо довернуть штурвал
        {
            // проверка на превышение разрешенных значений
            if (curMV - delta > VMax) delta = curMV - VMax;
            if (curMV - delta < VMin) delta = curMV - VMin;

            // поворачиваем штурвал регулятора
            movingPart.Rotate(0, delta * 10, 0); 
            curMV = curMV - delta;

            // отправляем новое значение % открытия в главную структуру для отправки на сервер, если крутили руками
            st.MV = curMV; //меняем вращение на клиенте
            mainScript.deviceListClient[deviceNum].device = st; // сохраняем новые данные в структуру приборов
            if(manual) mainScript.deviceListClient[deviceNum].time = DateTime.Now; // присвоим время обновления - чтобы передалось на сервер                        
        }

        //выводим текущие данные на панель
        UpdateTextPanel();
    }

    public void UpdateTextPanel() // выводим текущие данные на панель - важен порядок её дочерних объектов,
                                  // так как обращаемся к ним просто по номерам
    {        
        MainScript.PID01 st = (MainScript.PID01)mainScript.deviceListServer[deviceNum].device;  // считаем текущие значения всех 4-х тегов
        if (st.MODE == 0)  // если режим РУЧНОЙ
        {
            componentButtonText.text = "Ручной";              // выведем имя режима на кнопку и изменим её цвет
            componentButtonMesh.material.color = Color.green;
        }
        else               // иначе режим АВТОМАТИЧЕСКИЙ
        {
            componentButtonText.text = "Авто";                // выведем имя режима на кнопку и изменим её цвет
            componentButtonMesh.material.color = Color.blue;
        }

        if (Ed_izm == "МПа")  // для давления округлим до 2-х знаков после запятой
        {
            componentPVText.text = "PV:" + Math.Round(st.PV, 2).ToString() + " " + Ed_izm;
            componentSVText.text = "SV:" + Math.Round(st.SV, 2).ToString() + " " + Ed_izm;    
        }
        else                  // а для других до 1-го знака
        {
            componentPVText.text = "PV:" + Math.Round(st.PV, 1).ToString() + " " + Ed_izm;
            componentSVText.text = "SV:" + Math.Round(st.SV, 1).ToString() + " " + Ed_izm;
        }

        componentMVText.text = "MV:" + Math.Round(st.MV, 1).ToString() + " %"; // выведем текущий % открытия клапана
        NameKR.text = Name_prib;    // выведем имя прибора
        }
}
