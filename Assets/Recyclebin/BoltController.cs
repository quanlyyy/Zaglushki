using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BoltController : MonoBehaviour
{
    public int bolt = 6; // Количество болтов

    private bool sendState = false;
    public Camera m_Camera; 
    bool zapisal = false;

    public bool spawned = false;

    public GameObject ZaglushkaPlayer;

    public string storona; // Хранение стороны с которой начал пользователь 
    public int boltL = 0; // Кол-во открученых болтов с левой стороны
    public int boltR = 0; // Кол-во открученых болтов с правой стороны

    public GameObject trigPrefGaz; // Ссылка на триггер находжения в зоне газа
    public bool trigStateGaz; // 

    public GameObject zaglPref;

    public GameObject gazPref;
    [SerializeField]
    List<GameObject> boltListL;
    [SerializeField]
    List<GameObject> boltListR;

    private bool enabledBolt = false;

    Movetrub movementTrubs; 

    public bool firstClick = true; //Перменная для определения первого нажатия на болт
    public bool firstClick_ = true; //Перменная для определения первого нажатия на болт

    private bool spawnedZagl_ = false;

    public AudioSource zaglAudioZagl;
    public string flanec;
    public CameraOutline cameraOutline;

    char[] delChar = { 'L', 'R' };

    public void Awake()
    {
        m_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        movementTrubs = GameObject.Find("MovementTrubs").GetComponent<Movetrub>();
    }
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            boltListL.Add(GameObject.Find(i + "L").gameObject);
            boltListR.Add(GameObject.Find(i + "R").gameObject);
        }
        Debug.Log(boltListL);  
    }

    public string BoltCheck(GameObject hitObject)
    {
        if (hitObject.gameObject.name.Substring(1) == "L")
        {
            storona = "L";
            return "L";
        }
        else if (hitObject.gameObject.name.Substring(1) == "R")
        {
            storona = "R";
            return "R";
        }
        else
        {
            return "Ошибка определения стороны";
        }
    }

    public void boltCount(GameObject hitObj)
    {
        if (firstClick_) {
            firstClick_ = false;
            Debug.Log("Первое Нажатие на болт");
            if (BoltCheck(hitObj) == "L")
            {
                foreach (GameObject objs in boltListL)
                {
                    if (hitObj == objs)
                    {
                        boltListL.Remove(objs);
                        boltL += 1;
                        this.GetComponent<AudioSource>().Play();
                        hitObj.transform.gameObject.GetComponent<PlayAnim>().PlayAnimation(hitObj.transform.gameObject); // Получаем компонент и передаем в функцию, объект в который попали
                        flanec = "L";
                        GameObject.Find("Flanec_1").GetComponent<Flanec_1>().changeValue();
                        opcBolts(hitObj.gameObject.name);
                    }
                    else
                    {
                        Debug.Log("Повторное нажатие");
                    }
                }
            }
            else if (BoltCheck(hitObj) == "R")
            {
                foreach (GameObject objs in boltListR)
                {
                    if (hitObj == objs)
                    {
                        boltListR.Remove(objs);
                        boltR += 1;
                        this.GetComponent<AudioSource>().Play();
                        hitObj.transform.gameObject.GetComponent<PlayAnim>().PlayAnimation(hitObj.transform.gameObject); // Получаем компонент и передаем в функцию, объект в который попали
                        flanec = "R";
                        GameObject.Find("Flanec_2").GetComponent<Flanec_2>().changeValue();
                        opcBolts(hitObj.gameObject.name);
                    }
                    else
                    {
                        Debug.Log("Повторное нажатие");
                    }
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            Debug.Log("Нажатие на болт");
            if (BoltCheck(hitObj) == "L" && flanec == "L")
            {
                foreach (GameObject objs in boltListL)
                {
                    if (hitObj == objs)
                    {
                        boltListL.Remove(objs);
                        boltL += 1;
                        this.GetComponent<AudioSource>().Play();
                        hitObj.transform.gameObject.GetComponent<PlayAnim>().PlayAnimation(hitObj.transform.gameObject); // Получаем компонент и передаем в функцию, объект в который попали
                        opcBolts(hitObj.gameObject.name);
                    }
                    else
                    {
                        Debug.Log("Повторное нажатие");
                    }
                }
            }
            else if (BoltCheck(hitObj) == "R" && flanec == "R")
            {
                foreach (GameObject objs in boltListR)
                {
                    if (hitObj == objs)
                    {
                        boltListR.Remove(objs);
                        boltR += 1;
                        this.GetComponent<AudioSource>().Play();
                        hitObj.transform.gameObject.GetComponent<PlayAnim>().PlayAnimation(hitObj.transform.gameObject); // Получаем компонент и передаем в функцию, объект в который попали
                        opcBolts(hitObj.gameObject.name);
                    }
                    else
                    {
                        Debug.Log("Повторное нажатие");
                    }
                }
            }
            else
            {
                return;
            }
            GameObject obj = hitObj.gameObject;
        }
    }

    public void gazSpawn(string rl) // Функция спавна газа 
    {
        GameObject.Find("RemooveFlanec").GetComponent<RemooveFlanec>().changeValue();
        GameObject.Find("Sizod").GetComponent<Sizod>().changeValue();
        GameObject gazSpawned; // Переменная для хранения газа который мы заспавним

        

        Transform spawnPos = GameObject.Find("spawnPosTrigGaz").transform; // Получаем объект с позицией для спавна газа

        if (!trigStateGaz)  // Проверяем если еще не спавнили триггер
        {
            Instantiate(trigPrefGaz, spawnPos.position, spawnPos.rotation); // Спавним триггер который проверяет ушел ли пользователь с каре 
            trigStateGaz = true; // Переменную статуса меняем на true
        }
        

        if (!spawned)  // Проверяем если не спавнили газ
        {
            // Проверяем с какой стороны спавнить газ
            if (rl == "L" && !spawned) // слевой стороны ?
            {
                Transform pos = GameObject.Find("GazLeft").transform; // Находим объект для спавна газа и записывем его компонент Transform
                gazSpawned = Instantiate(gazPref, pos.position, pos.rotation); // Спавним газ и записываем объект который мы заспавнили
                 
            }
            else if (rl == "R" && !spawned) // справой стороны ?
            {
                Transform pos = GameObject.Find("GazRight").transform;
                gazSpawned = Instantiate(gazPref, pos.transform.position, pos.rotation);
            }
            else
            {
                Debug.LogError("Ошибка обработки стороны и спавна газа");
            }
            spawned = true;
        }
    }
/*
    public void moveTrub(string rl)
    { 
        trub.SetActive(false);

        if (rl == "L")
        {
            trubL.SetActive(true);    
        }
        else if (rl == "R")
        {
            trubR.SetActive(true);
        }
        else
        {
            Debug.LogError("Ошибка уменьшения стороны");
            trub.SetActive(true);
        }
    }
*/
    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0) && !zapisal) // Если левая кнопка мыши была нажата 
        {
            // Запускаем луч из позиции мыши 
            Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new();
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "Bolts") // если попали в объект с тэгом болт
                {
                    zapisal = true;
                    
                    boltCount(hit.transform.gameObject); // Вызываем функцию и передаем объект
                }
                if (hit.transform.gameObject.tag == "Zagl") // Если попали в объект с тэгом заглушки
                {
                    zapisal = true;
                    zaglSpawn(hit.transform.gameObject); // Вызываем функцю спавна заглушки и передаем в нее объект
                }
                if (hit.transform.gameObject.tag == "Hole") // Если попали в объект с тэгом отверстия 
                {
                    /*
                    zapisal = true;
                    Debug.Log("Закручивание болта");
                    GameObject.Find("MovementTrubs").GetComponent<Movetrub>().trubL.SetActive(false);
                    GameObject.Find("MovementTrubs").GetComponent<Movetrub>().trubR.SetActive(false);
                    GameObject.Find("MovementTrubs").GetComponent<Movetrub>().trub.SetActive(true);*/
                    GameObject.Find("MovementTrubs").GetComponent<ReverseBoltController>().playAnimRevers(storona, hit.transform.gameObject); // Находим объект на сцене и вызываем функцию передав в нее стороне и объект в который мы попали 
                }

                Debug.DrawLine(ray.origin, hit.point, Color.red);   
            }
        }
        else
        {
            zapisal = false;
        }

        if (boltL >= 6 && !enabledBolt) // Если количество открученых болтов с левой стороны равно или больше 6 
        {
            gazSpawn("L"); // Спавним газ из трубы слева
            GameObject.Find("GazZone01").gameObject.GetComponent<GaZZone01>().gazAnalizator = true; // Запускаем работу газ-анализатора 
            movementTrubs.moveTrub("L"); // Раздвигаем трубы делая зазор с левой стороны
            enabledBolt = true; // Меняем статус болтов на true
        }
        else if (boltR >= 6 && !enabledBolt) // Если количество болтов с правой стороны меньше или равно шести
        {
            gazSpawn("R");  // Спавним газ с правой стороны
            GameObject.Find("GazZone01").gameObject.GetComponent<GaZZone01>().gazAnalizator = true; // Запускаем работу газ-анализатора 
            movementTrubs.moveTrub("R"); // раздвигаем труьы делая зазор с правой стороны
            enabledBolt = true;
        }
    }

    public void zaglSpawn(GameObject hit) // Функция спавна заглушки 
    {
        if (!spawnedZagl_) // Если заглушку еще не спавнили
        {
            GameObject spawnedZagl = Instantiate(zaglPref, hit.transform.position, hit.transform.rotation, GameObject.Find("MovementTrubs").gameObject.transform); // спавним заглушку и сохраняем в переменную spawnedZagl
            cameraOutline.holeState = true; // Находим объект на сцене, получаем компонент контроля подсветки объектов, изменяем статус для отверстий на true
            Destroy(hit.gameObject); // Удаляем объект в который попали рейкастом(Это объект подсказка места для установки заглушки)
            ZaglushkaPlayer.SetActive(false); // Выключаем заглушку в руках у игрока

            //spawnedZagl.GetComponent<AudioSource>().volume = 1.0f;

            if (storona == "R") // Если место установки с права
            {
                spawnedZagl.GetComponent<Animator>().Play("ZaglRDown"); // Запускаем анимацию появления заглушки с правой стороны
            }
            else if (storona == "L") // Если место установки с лево
            {
                spawnedZagl.GetComponent<Animator>().Play("ZaglLDown"); // Запускаем анимацию появления заглушки с левой стороны
            }
            spawnedZagl_ = true;
        }
        
    }

    public void opcBolts(string objName)
    {
        string nameOPC = objName.TrimEnd(delChar);

        Debug.Log("Болт ОРС под номером - " + nameOPC);

        if (nameOPC == "1")
        {
            GameObject.Find("BoltRemoove_1").GetComponent<BoltRemoove_1>().changeValue();
        }
        else if (nameOPC == "2")
        {
            GameObject.Find("BoltRemoove_2").GetComponent<BoltRemoove_2>().changeValue();
        }
        else if (nameOPC == "3")
        {
            GameObject.Find("BoltRemoove_3").GetComponent<BoltRemoove_3>().changeValue();
        }
        else if (nameOPC == "4")
        {
            GameObject.Find("BoltRemoove_4").GetComponent<BoltRemoove_4>().changeValue();
        }
        else if (nameOPC == "5")
        {
            GameObject.Find("BoltRemoove_5").GetComponent<BoltRemoove_5>().changeValue();
        }
        else if(nameOPC == "6")
        {
            GameObject.Find("BoltRemoove_6").GetComponent<BoltRemoove_6>().changeValue();
        }
        else
        {
            Debug.Log("Ошибка вызова функции OPC");
        }
    }
}
