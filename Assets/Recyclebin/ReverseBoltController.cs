using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReverseBoltController : MonoBehaviour
{
    public bool isMoved = false;
    private int boltCount = 0;
    char[] delChar = { 'L', 'R' }; //Сохраняем символы для удаления из названия 
    private bool opcSend = false;
    public GameObject boltsGameObject;

    public void playAnimRevers(string rl = "", GameObject hitObj = null) //Функция принимает сторону с которой пользователь начал откручивать а так же объект на который нажал пользователь
    {
        Debug.Log("playAnimRevers");
        if (!isMoved) //Проверяем если это первое нажатие на отверстие для болта, то сдвигаем трубы
        {
            GameObject.Find("MovementTrubs").GetComponent<Movetrub>().trubL.SetActive(false); 
            GameObject.Find("MovementTrubs").GetComponent<Movetrub>().trubR.SetActive(false); 
            GameObject.Find("MovementTrubs").GetComponent<Movetrub>().trub.SetActive(true); 

            isMoved = true; // Меняем значение переменной, следующее нажатие уже будет не первым 
        }
        else
        { 
            // Получаем дочерние объекты и включаем их
            hitObj.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            hitObj.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            hitObj.gameObject.transform.GetChild(2).gameObject.SetActive(true);
            hitObj.gameObject.transform.GetChild(3).gameObject.SetActive(true);

            // Запускаем анимацию по имени объекта, удалив последний символ из имени объекта 
            hitObj.GetComponent<Animator>().Play(hitObj.name.TrimEnd(delChar));
            boltsGameObject.GetComponent<AudioSource>().Play();

            Debug.Log(hitObj.name.TrimEnd(delChar)); // Выводим название объекта у которого запускали анимацию
            boltCount += 1;
            opcBolts(hitObj.gameObject.name);
            //return true;
        }
    }

    public void Update()
    {
        if (boltCount >= 6 && !opcSend)
        {
            GameObject.Find("InstallFlanec").GetComponent<InstallFlanec>().changeValue();
            opcSend = true;
        }
    }
    public void opcBolts(string objName)
    {
        string nameOPC = objName.TrimEnd(delChar);

        if (nameOPC == "1")
        {
            GameObject.Find("BoltInstall_1").GetComponent<BoltInstall_1>().changeValue();
        }
        else if (nameOPC == "2")
        {
            GameObject.Find("BoltInstall_2").GetComponent<BoltInstall_2>().changeValue();
        }
        else if (nameOPC == "3")
        {
            GameObject.Find("BoltInstall_3").GetComponent<BoltInstall_3>().changeValue();
        }
        else if (nameOPC == "4")
        {
            GameObject.Find("BoltInstall_4").GetComponent<BoltInstall_4>().changeValue();
        }
        else if (nameOPC == "5")
        {
            GameObject.Find("BoltInstall_5").GetComponent<BoltInstall_5>().changeValue();
        }
        else if (nameOPC == "6")
        {
            GameObject.Find("BoltInstall_6").GetComponent<BoltInstall_6>().changeValue();
        }
        else
        {
            Debug.Log("Ошибка вызова функции OPC");
        }
    }

}
