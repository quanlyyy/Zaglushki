using UnityEngine;
//для показа панели инструктора
public class MainMenuRoleUI : MonoBehaviour
{
    public GameObject instructorPanel;

    void Start()
    {
        if (instructorPanel != null)
            instructorPanel.SetActive(UserSession.IsInstructor);
    }
}