using UnityEngine;

public class TrainingIntroUI : MonoBehaviour
{
    [Header("Intro Canvases")]
    public GameObject canvas1;
    public GameObject canvas2;
    public GameObject canvas3;

    [Header("After Intro")]
    public GameObject gameplayHintCanvas;
    public MonoBehaviour[] scriptsToEnableAfterIntro;

    private int currentPage = 1;

    void Start()
    {
        ShowPage(1);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameplayHintCanvas != null)
            gameplayHintCanvas.SetActive(false);

        if (scriptsToEnableAfterIntro != null)
        {
            foreach (var s in scriptsToEnableAfterIntro)
            {
                if (s != null)
                    s.enabled = false;
            }
        }
    }

    public void NextPage()
    {
        currentPage++;
        ShowPage(currentPage);
    }

    public void CloseIntro()
    {
        if (canvas1 != null) canvas1.SetActive(false);
        if (canvas2 != null) canvas2.SetActive(false);
        if (canvas3 != null) canvas3.SetActive(false);

        if (gameplayHintCanvas != null)
            gameplayHintCanvas.SetActive(true);

        if (scriptsToEnableAfterIntro != null)
        {
            foreach (var s in scriptsToEnableAfterIntro)
            {
                if (s != null)
                    s.enabled = true;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ShowPage(int page)
    {
        if (canvas1 != null) canvas1.SetActive(page == 1);
        if (canvas2 != null) canvas2.SetActive(page == 2);
        if (canvas3 != null) canvas3.SetActive(page == 3);
    }
}