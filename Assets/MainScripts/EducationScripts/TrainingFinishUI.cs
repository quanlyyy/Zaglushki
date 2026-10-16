using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingFinishUI : MonoBehaviour
{
    public GameObject finishCanvas;
    public MonoBehaviour[] scriptsToDisableOnFinish;

    void Start()
    {
        if (finishCanvas != null)
            finishCanvas.SetActive(false);
    }

    public void ShowFinish()
    {
        if (finishCanvas != null)
            finishCanvas.SetActive(true);

        if (scriptsToDisableOnFinish != null)
        {
            foreach (var s in scriptsToDisableOnFinish)
            {
                if (s != null)
                    s.enabled = false;
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitToMenu(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}