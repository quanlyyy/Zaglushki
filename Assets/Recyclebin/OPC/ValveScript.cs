using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

//скрипт занимается управлением крутилками - его надо добавлять во все такие объекты сцены. Если понадобятся объекты других типов - то для них нужно будет делать скрипты наподобии этого
public class ValveScript : MonoBehaviour
{
    Camera mainCamera; //это главная камера - нужна тут, чтобы пускать из нее лучи в сторону курсора мыши и понимать, куда нажимает пользователь
    Transform movingPart; //дочерний объект, который будем крутить этим скриптом на относительное вращение следующего объекта
    Transform fakeMovingPart; //дочерний объект, который будем крутить этим скриптом для получения относительного вращения
    MainScript mainScript; //ссылка на главный скрипт - для связи с его данными
    TextMesh valueText; //компонент, отображающий значение процента открытия вентиля на сцене 
    TextMesh NumText; //компонент, отображающий

    //эти переменные заполняются при загрузке файла в главном скрипте
    public int deviceNum; //индекс текущего прибора в главном списке 
    public string Name_prib;
    public float VMin;
    public float VMax;
    public float rotateSpeed = 0.1f;

    float curZ; //будем хранить текущее вращение прямо тут в объекте - и сравнивать его с тем, что приходит с сервера, чтобы определять дельту и поворачивать автоматически
    DateTime lastTouchTime; //будем хранить время последнего касания крутилки и не пускать сервер к управлению несколько секунд после касания (чтобы не дергалось туда-сюда, это некрасиво)

  
    // Start is called before the first frame update
    void Start()
    {

        //получаем камеру
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        //получаем дочерний объект, который будем двигать этим скриптом
        movingPart = transform.Find("Gate Valve");
        fakeMovingPart = transform.Find("Tube");

        //получаем ссылку на главный скрипт
        mainScript = GameObject.Find("GameObject").GetComponent<MainScript>();

       
        //получаем компонент текст - идем в родителя текущего объекта сцены и запрашиваем все компоненты такого типа у его детей
        valueText = transform.GetComponentsInChildren <TextMesh> ()[0];
        NumText = transform.GetComponentsInChildren<TextMesh>()[1];
        NumText.text = Name_prib;
    }

    // Update is called once per frame
    void Update() //вызывается каждый кадр - по умолчанию 60 раз в секунду
    {        
        float delta = 0; //наше вращения
        bool manual = false; //признак того, что в этот такт крутили руками
        
        //получим данные прибора c сервера
        MainScript.Valve st = (MainScript.Valve)mainScript.deviceListServer[deviceNum].device; 

        //пишем поворот в 3D-текст на сцене
        valueText.text = Math.Round(st.Z, 1).ToString() + " %";

        //опросим пользователя
        if (Input.GetMouseButton(0)) //зажали левую кнопку мыши
        {            
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); //бросаем из главной камеры луч в направлении мыши на экране
            RaycastHit hit = new RaycastHit(); //получаем данные объекта сцены, с которым пересекся луч (для этого у объекта должен быть коллайдер)
            if (Physics.Raycast(ray, out hit) && hit.transform == transform) //выполняется если попали лучом в текущий объект сцены 
            {
                //тут небольшой костыль - вращение основного объекта мы определяем через вращение дополнительного (так как надо перейти от абсолютного угла к относительному)
                float oldY = fakeMovingPart.localEulerAngles.y; //старый угол дополнительного объекта - сохраняем в переменную, чтобы сравнить с новым
                fakeMovingPart.LookAt(hit.point); //главная фишка всего расчета - этой операцией наш дополнительный объект поворачивается в сторону касания мыши (и не надо считать никакие тангенсы)
                float newY = fakeMovingPart.localEulerAngles.y; //новый угол дополнительного вращения (после поворота)
                fakeMovingPart.localEulerAngles = new Vector3(0, newY, 0); //обнуляем поворот по всем осям кроме Y - иначе поворачиваться будет не только в одной плоскости.
                if (oldY > 300 && newY < 60) delta = newY + 360 - oldY; //проверяем на переход угла через 360 градусов - после них следует 0 и без этих поправок начинает дергаться
                else if(oldY < 60 && newY > 300) delta = newY - 360 - oldY; //и в другую сторону
                else delta = newY - oldY; //и если не перешло через 360
                delta /= 20; //делим полученную дельту на 20 - тут опционально
                lastTouchTime = DateTime.Now; //запоминаем последнее время касания мыши
                manual = true; //ставим признак, что управляли руками
            }
        }

        if (lastTouchTime.AddSeconds(1) < DateTime.Now && mainScript.autoRotate) //если не крутили руками последнюю секунду и включено автовращение - то крутим автоматом с сервера 
        {
            delta = curZ - st.Z;
        }

        if (delta > 0) delta = Math.Min(delta, rotateSpeed); //ограничение скорости вращения
        else delta = Math.Max(delta, -rotateSpeed); //и в другую сторону

        if (delta != 0)
        {
            //проверка на превышение разрешенных углов
            if (curZ - delta > 100) delta = curZ - 100;
            if (curZ - delta < 0) delta = curZ - 0;
                          
            //поворачиваем
            movingPart.Rotate(0, delta * 10, 0); 
            curZ = curZ - delta;

            //отправляем в главную структуру, если крутили руками
            st.Z = curZ; //меняем данные на клиенте
            mainScript.deviceListClient[deviceNum].device = st; //сохраняем
            if (manual) mainScript.deviceListClient[deviceNum].time = DateTime.Now; //время обновления - чтобы передалось на сервер            
        }
    }
}
