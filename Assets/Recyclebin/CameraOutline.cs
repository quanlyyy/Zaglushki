using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOutline : MonoBehaviour
{
    Camera mainCamera; // Переменная для хранения главной камеры

    public Outline[] outlines; // Массив который хранит все объекты на которых есть скрипт подсветки
    public bool wrenchState; // Переменная для статуса гаечного ключа 
    public bool zaglState; // Переменная для статуса установки заглушки

    public bool holeState = false; // Переменная для статуса нужна ли подсветка места для установки болтов 

    /* Переменные со статусами нужны для последовательного выполнения работ, то есть подсвечивать место установки заглушки можно только после откручивания всех болтов на определеной стороне */

    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>(); // Получаем главную камеру, ищем объект на сцене по его имени
    }


    void FixedUpdate()
    {
        wrenchState = GameObject.Find("Player").GetComponent<Player>().wrenchState; // Устанавливаем связь с той же переменной в скрипте PLayer

        Outline hitOutline; // Создаем временную переменную для хранения именно КОМПОНЕНТА объекта на который попали курсором мыши

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // Запускаем луч из позиции мыши
        RaycastHit hit = new RaycastHit(); // Записываем объект на который попали лучом 
        if (Physics.Raycast(ray, out hit)) // Если есть попадание в объект с коллайдером
        {
            Debug.DrawLine(ray.origin, hit.point); // Отрисововаем луч которым попали
            // Проверяем в какой именно объект мы попали и проверяем можем ли мы в данный момент времени его подсвечивать (проверяем по статусам)
            if ((hit.transform.gameObject.tag == "Bolts" && wrenchState) || (hit.transform.gameObject.tag == "Zagl" && zaglState) || (hit.transform.gameObject.tag == "Hole" && holeState && wrenchState))
            {
                hitOutline = hit.transform.gameObject.GetComponent<Outline>(); // Получаем компонент Outline из объекта в который мы попали 
                hitOutline.OutlineWidth = 3.0f; // Устанавливаем ширину линии подсветки равной 3.0f
            }
            else
            {
                offAllOutline(); // Если мы не можем подсветить или мы никуда не попали, то вырубаем подсветку всех что может подсвечиваться 
            }
            cheakOutline(hit.transform.gameObject); // Устаревшая функция в данный момент на работу не влияет
        }   
    }

    private void offAllOutline() // Функция которая выключает подсветку всех объектов
    {
        /*
        foreach (Outline outline in outlines)
        {
            outline.OutlineWidth = 0;
        }*/
     
        for (int a = 0; a < outlines.Length; a++)
        {
            outlines[a].OutlineWidth = 0;
            //Debug.Log(a);
        }
        
        
    }

     void cheakOutline(GameObject hitObj = null)
    {
        if (hitObj.tag == "Bolts" || hitObj.tag == "Zagl" || hitObj.tag == "Hole")
        {
            return;
        }
        /*
        else
        {
            offAllOutline();
        }
        */
    }
}
