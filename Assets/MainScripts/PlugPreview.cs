using UnityEngine;

public class PlugPreview : MonoBehaviour
{
    [Header("Links")]
    public Flange flange;                // фланец
    public GameObject finalPlug;         // финальная заглушка

    [Header("Install FX")]
    public Animator finalAnimator;       // Animator на финальной заглушке
    public string installTrigger = "Install";
    public AudioSource audioSource;      // AudioSource на финальной заглушке
    public AudioClip installClip;        // звук установки

    [Header("Outline (optional)")]
    public Outline outline;
    public float outlineWidth = 3f;

    [Header("State")]
    public bool isAvailable;             // станет true после откручивания всех болтов
    public bool isInstalled;

    void Awake()
    {
        if (finalPlug != null) finalPlug.SetActive(false);

        if (outline == null)
            outline = GetComponent<Outline>();

        SetHighlight(false);
    }

    public void SetAvailable(bool value)
    {
        isAvailable = value;
        gameObject.SetActive(value);
        if (!value) SetHighlight(false);
    }

    void OnMouseEnter()
    {
        if (CanHover())
            SetHighlight(true);
    }

    void OnMouseExit()
    {
        SetHighlight(false);
    }

    void OnMouseDown()
    {
        if (!CanInstall())
            return;

        isInstalled = true;

        gameObject.SetActive(false);


        if (finalPlug != null)
            finalPlug.SetActive(true);

        if (finalAnimator == null && finalPlug != null)
            finalAnimator = finalPlug.GetComponentInChildren<Animator>(true);

        if (finalAnimator != null && !string.IsNullOrEmpty(installTrigger))
            finalAnimator.ResetTrigger(installTrigger);
        if (finalAnimator != null && !string.IsNullOrEmpty(installTrigger))
            finalAnimator.SetTrigger(installTrigger);

        if (audioSource == null && finalPlug != null)
            audioSource = finalPlug.GetComponentInChildren<AudioSource>(true);

        if (audioSource != null && installClip != null)
            audioSource.PlayOneShot(installClip);

        if (flange != null && flange.valveUnit != null)
            flange.valveUnit.RegisterPlugInstallation(flange);

        if (flange.valveUnit.IsPlugCorrect())
        {
            ExamController.Instance.AddScore(10,
                $"Заглушка установлена правильно на задвижке {flange.valveUnit.name}, сторона {flange.side}");
        }
        else
        {
            ExamController.Instance.AddZeroScore(
                $"Заглушка установлена неправильно на задвижке {flange.valveUnit.name}, сторона {flange.side}, правильная {flange.valveUnit.correctSide}"
            );
        }

        Debug.Log($"Заглушка установлена на {flange.name} ({flange.side})");

        if (TrainingManager.Instance != null)
            TrainingManager.Instance.OnPlugInstalled(flange);
    }

    bool CanHover()
    {
        if (!isAvailable || isInstalled) return false;
        if (flange == null) return false;

        if (TrainingManager.Instance != null)
        {
            if (!TrainingManager.Instance.CanWorkWithFlange(flange))
                return false;
        }

        if (!ExamController.Instance.CanUnscrewOnFlange(flange))
            return false;

        if (!flange.isCompleted)
            return false;

        return true;
    }

    bool CanInstall()
    {
        if (!CanHover()) return false;

        // в руках заглушка
        return PlayerInventory.Instance.HasHandItem(HandItemType.Plug);
    }

    void SetHighlight(bool value)
    {
        if (outline != null)
            outline.OutlineWidth = value ? outlineWidth : 0f;
    }
}
