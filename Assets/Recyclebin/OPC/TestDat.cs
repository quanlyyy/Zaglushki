using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDat : MonoBehaviour
{
    Camera mainCamera; //это главная камера - нужна тут, чтобы пускать из нее лучи в сторону курсора мыши и понимать, куда нажимает пользователь
    MainScript mainScript; //ссылка на главный скрипт - для связи с его данными
    Transform knopka; //панель управления
    GameObject plafon;

    // Start is called before the first frame update
    void Start()
    {
        //получаем камеру
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        //получаем ссылку на главный скрипт
        mainScript = GameObject.Find("GameObject").GetComponent<MainScript>();

        //получаем кнопку управления
        knopka = transform.Find("Knopka_ust");
        if (knopka == null) print("Нет кнопки");
        else print("Есть кнопка");
    }

    // Update is called once per frame
    void Update()
    {

        //опросим пользователя
        if (Input.GetMouseButton(0)) //зажали левую кнопку мыши
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); //бросаем из главной камеры луч в направлении мыши на экране
            RaycastHit hit = new RaycastHit(); //получаем данные объекта сцены, с которым пересекся луч (для этого у объекта должен быть коллайдер)
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == knopka) //открываем UI панель, если нажали на панель управления
                {
                    print("knopka");
                    GameObject[] plafons = GameObject.FindGameObjectsWithTag("post_2");
 
                    for (int i = 0; i < plafons.Length; i++)
                    {
                        plafons[i].GetComponent<MeshRenderer>().material.color = Color.green;
                    }

                }
            }

        }
             
    }
}
