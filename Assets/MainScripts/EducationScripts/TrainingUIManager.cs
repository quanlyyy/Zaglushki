using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingUIManager : MonoBehaviour
{
    [Header("Theory Canvases")]
    public GameObject theoryCanvas1;
    public GameObject theoryCanvas2;
    public GameObject theoryCanvas3;

    [Header("Gameplay UI")]
    public GameObject gameplayCanvas;
    public TMP_Text hintText;

    [Header("Finish UI")]
    public GameObject finishCanvas;

    [Header("Player Control")]
    public MonoBehaviour[] playerControlScriptsToDisable;

    private bool uiBlocksPlayer = false;
    private bool isTrainingStarted = false;

    void Start()
    {
        if (theoryCanvas1 != null) theoryCanvas1.SetActive(true);
        if (theoryCanvas2 != null) theoryCanvas2.SetActive(false);
        if (theoryCanvas3 != null) theoryCanvas3.SetActive(false);

        if (gameplayCanvas != null) gameplayCanvas.SetActive(false);
        if (finishCanvas != null) finishCanvas.SetActive(false);

        FreezePlayer();
        ShowCursor();
    }

    void Update()
    {
        if (uiBlocksPlayer)
        {
            ShowCursor();
        }
    }

    public void OpenTheoryPage2()
    {
        if (theoryCanvas1 != null) theoryCanvas1.SetActive(false);
        if (theoryCanvas2 != null) theoryCanvas2.SetActive(true);
        if (theoryCanvas3 != null) theoryCanvas3.SetActive(false);

        FreezePlayer();
        ShowCursor();
    }

    public void OpenTheoryPage3()
    {
        if (theoryCanvas1 != null) theoryCanvas1.SetActive(false);
        if (theoryCanvas2 != null) theoryCanvas2.SetActive(false);
        if (theoryCanvas3 != null) theoryCanvas3.SetActive(true);

        FreezePlayer();
        ShowCursor();
    }

    //открытие из игры
    public void OpenInstructionFromGame()
    {
        if (theoryCanvas1 != null) theoryCanvas1.SetActive(true);
        if (theoryCanvas2 != null) theoryCanvas2.SetActive(false);
        if (theoryCanvas3 != null) theoryCanvas3.SetActive(false);

        if (gameplayCanvas != null) gameplayCanvas.SetActive(false);

        FreezePlayer();
        ShowCursor();
    }

    //кнопка для закрытия
    public void CloseInstructionSmart()
    {
        //запуск обчуения
        if (!isTrainingStarted)
        {
            isTrainingStarted = true;

            if (theoryCanvas1 != null) theoryCanvas1.SetActive(false);
            if (theoryCanvas2 != null) theoryCanvas2.SetActive(false);
            if (theoryCanvas3 != null) theoryCanvas3.SetActive(false);

            if (gameplayCanvas != null) gameplayCanvas.SetActive(true);

            UnfreezePlayer();
            HideCursor();

            if (TrainingManager.Instance != null)
                TrainingManager.Instance.StartTraining();
        }
        else
        {
            //зактритие инструкции
            if (theoryCanvas1 != null) theoryCanvas1.SetActive(false);
            if (theoryCanvas2 != null) theoryCanvas2.SetActive(false);
            if (theoryCanvas3 != null) theoryCanvas3.SetActive(false);

            if (gameplayCanvas != null) gameplayCanvas.SetActive(true);

            UnfreezePlayer();
            HideCursor();
        }
    }

    public void ShowHint(string text)
    {
        if (hintText != null)
            hintText.text = text;
    }

    public void ShowFinishCanvas()
    {
        if (finishCanvas != null) finishCanvas.SetActive(true);
        if (gameplayCanvas != null) gameplayCanvas.SetActive(false);

        FreezePlayer();
        ShowCursor();
    }

    public void ExitToMenu(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private void FreezePlayer()
    {
        uiBlocksPlayer = true;

        if (playerControlScriptsToDisable != null)
        {
            foreach (var script in playerControlScriptsToDisable)
            {
                if (script != null)
                    script.enabled = false;
            }
        }
    }

    private void UnfreezePlayer()
    {
        uiBlocksPlayer = false;

        if (playerControlScriptsToDisable != null)
        {
            foreach (var script in playerControlScriptsToDisable)
            {
                if (script != null)
                    script.enabled = true;
            }
        }
    }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}