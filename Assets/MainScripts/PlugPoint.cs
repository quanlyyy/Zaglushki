using UnityEngine;

public class PlugPoint : MonoBehaviour
{
    [Header("Links")]
    public Flange flange;

    [Header("Visuals")]
    public GameObject previewPlug;   // полупрозрачная 
    public GameObject finalPlug;     // финальная

    [Header("Outline")]
    public Outline outline;          // Outline на PlugPoint или на previewPlug
    public float outlineWidth = 3f;

    [Header("Runtime")]
    public bool isAvailable;         // можно ли показывать превью
    public bool isInstalled;

    void Awake()
    {
        if (previewPlug != null) previewPlug.SetActive(false);
        if (finalPlug != null) finalPlug.SetActive(false);

        if (outline == null)
            outline = GetComponent<Outline>();

        SetHighlight(false);
        isAvailable = false;
        isInstalled = false;
    }

    void OnEnable()
    {
        // подписываемся на событие фланца
        if (flange != null)
            flange.OnAllBoltsUnscrewed += EnablePreview;
    }

    void OnDisable()
    {
        if (flange != null)
            flange.OnAllBoltsUnscrewed -= EnablePreview;
    }

    private void EnablePreview()
    {
        //активация превью если фланец активный
        if (ExamController.Instance.activeFlange != flange)
            return;

        isAvailable = true;

        if (previewPlug != null)
            previewPlug.SetActive(true);

        Debug.Log($"превью заглушки включено для {flange.name}");
    }

    public bool CanHover()
    {
        if (!isAvailable) return false;
        if (isInstalled) return false;

        //подстветка на активном фланце
        if (!ExamController.Instance.CanUnscrewOnFlange(flange))
            return false;

        return true;
    }

    public bool CanInstall()
    {
        if (!CanHover())
        {
            Debug.Log("CanInstall=false: CanHover=false");
            return false;
        }

        bool hasPlug = PlayerInventory.Instance != null &&
                       PlayerInventory.Instance.HasHandItem(HandItemType.Plug);

        if (!hasPlug)
            Debug.Log("в руках НЕТ заглушки");

        return hasPlug;
    }

    public void Install()
    {
        Debug.Log("Install() вызван на " + name);

        if (!CanInstall())
        {
            Debug.Log("Install прерван: CanInstall=false");
            return;
        }

        isInstalled = true;

        if (previewPlug != null)
        {
            previewPlug.SetActive(false);
            Debug.Log("previewPlug OFF");
        }
        else Debug.LogError("previewPlug не назначен в инспекторе!");

        if (finalPlug != null)
        {
            finalPlug.SetActive(true);
            Debug.Log("finalPlug ON");
        }
        else Debug.LogError("finalPlug не назначен в инспекторе!");

        SetHighlight(false);

        if (flange != null && flange.valveUnit != null)
        {
            flange.valveUnit.RegisterPlugInstallation(flange);
            Debug.Log("RegisterPlugInstallation OK");
        }
        else Debug.LogError("flange или flange.valveUnit не назначены!");

        Debug.Log($"Заглушка УСТАНОВЛЕНА на {flange.name} ({flange.side})");
    }


    public void SetHighlight(bool state)
    {
        if (outline != null)
            outline.OutlineWidth = state ? outlineWidth : 0f;
    }
}
