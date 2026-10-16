using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnim : MonoBehaviour
{
    public string animName; // Имя анимации


    public void PlayAnimation(GameObject hitObj) // Функция запуска анимации 
    {
        GameObject LeftWrench, RightWrench; // Переменные для гаечных ключей 

        LeftWrench = hitObj.transform.GetChild(2).gameObject; // Получаем дочерний объект, объекта в который попали лучом 
        RightWrench = hitObj.transform.GetChild(3).gameObject; // Получаем дочерний объект, объекта в который попали лучом 

        LeftWrench.SetActive(true); // Включаем гаечные ключи
        RightWrench.SetActive(true); // Включаем гаечные ключи

        Debug.Log(animName); 
        if (animName == "") // Если переменная еще пустая
        {
            animName = hitObj.name.Substring(1); // записываем в переменную название анимации без последнего символа
        }
        Debug.Log(animName);
        hitObj.GetComponent<Animator>().Play(animName); // получаем компонент аниматора и запускаем анимацию по названию записаному в переменную
    }

    public void PlayAnimationReverse(GameObject hitObj) // Запуск анимации закручивания болтов
    {
        GameObject LeftWrench, RightWrench;

        LeftWrench = hitObj.transform.GetChild(2).gameObject;
        RightWrench = hitObj.transform.GetChild(3).gameObject;

        LeftWrench.SetActive(true);
        RightWrench.SetActive(true);

        Debug.Log(animName);
        if (animName == "")
        {
            animName = hitObj.name.Substring(1) + " 0";
        }
        Debug.Log(animName);
        hitObj.GetComponent<Animator>().Play(animName);
    }


    public void OnExitAnimation()
    {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(2).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(3).gameObject.SetActive(false);
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }
}
