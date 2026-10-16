using System.Collections;
using UnityEngine;

public class GasReleaseController : MonoBehaviour
{
    [Header("Only this valve number will spawn gas")]
    public int targetValveNumber = 13;

    [Header("Links")]
    public ValveUnit valveUnit;

    [Header("Gas burst (visual)")]
    public GameObject gasEffectLeft;     // эффект слева
    public GameObject gasEffectRight;    // эффект справа
    [Tooltip("Сколько секунд показывать выпуск газа")]
    public float gasBurstDuration = 0.5f;

    [Header("Zone audio (while inside gas zone)")]
    public AudioSource gasAudio;         // loop звука

    [Header("Zones (both start OFF)")]
    public GasZone gasZone;              // триггер газа
    public GasMaskEquipZone equipZone;   // зона "противогаз"

    [Header("UI")]
    public GameObject gasMaskButtonRoot; // корневой объект кнопки
    public GasAnalyzerUI_TMP gasAnalyzerUI;

    [Header("Gas analyzer values inside zone")]
    public float value1 = 15f;
    public float value2 = 5f;
    public float value3 = 2f;
    public float value4 = 0f;

    private bool gasStarted;

    void Awake()
    {
        if (valveUnit == null) valveUnit = GetComponent<ValveUnit>();

        if (gasEffectLeft != null) gasEffectLeft.SetActive(false);
        if (gasEffectRight != null) gasEffectRight.SetActive(false);

        if (gasZone != null) gasZone.gameObject.SetActive(false);
        if (equipZone != null) equipZone.gameObject.SetActive(false);

        if (gasMaskButtonRoot != null) gasMaskButtonRoot.SetActive(false);

        if (gasAudio != null) gasAudio.Stop();

        if (gasZone != null) gasZone.Init(this);
        if (equipZone != null) equipZone.Init(this);
    }

    //когда все болты откручены
    public void OnFlangeFullyUnscrewed(Flange flange)
    {
        if (gasStarted) return;
        if (valveUnit == null) return;
        if (valveUnit.valveNumber != targetValveNumber) return;

        StartGas(flange);
    }

    private void StartGas(Flange flange)
    {
        gasStarted = true;

        //выпуск газа слева или справа
        if (flange != null)
        {
            if (flange.side == FlangeSide.Left)
                StartCoroutine(PlayBurst(gasEffectLeft));
            else
                StartCoroutine(PlayBurst(gasEffectRight));
        }

        //включение зоны
        if (gasZone != null) gasZone.gameObject.SetActive(true);
        if (equipZone != null) equipZone.gameObject.SetActive(true);

        Debug.Log("Газ: зоны активированы (Valve 13)");
    }

    private IEnumerator PlayBurst(GameObject effectGO)
    {
        if (effectGO == null) yield break;

        effectGO.SetActive(true);
        yield return new WaitForSeconds(gasBurstDuration);
        effectGO.SetActive(false);
    }

    //плеер в зоне газа
    public void OnPlayerEnterGasZone()
    {
        if (gasAudio != null && !gasAudio.isPlaying) gasAudio.Play();

        if (gasAnalyzerUI != null)
            gasAnalyzerUI.SetValues(value1, value2, value3, value4);
    }

    //плеер вышел из зоны газа
    public void OnPlayerExitGasZone()
    {
        if (gasAudio != null && gasAudio.isPlaying) gasAudio.Stop();

        if (gasAnalyzerUI != null)
            gasAnalyzerUI.ResetValues();
    }

    //плеер в зоне противогаща
    public void OnPlayerEnterEquipZone()
    {
        if (PlayerInventory.Instance.isGasMaskOn)
            return;

        if (gasMaskButtonRoot != null)
            gasMaskButtonRoot.SetActive(true);

        if (equipZone != null)
            equipZone.gameObject.SetActive(false);
    }

    public void OnGasMaskButtonClicked()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.EquipGasMaskPermanent();

        if (gasMaskButtonRoot != null)
            gasMaskButtonRoot.SetActive(false); // больше не показываем

        //выключаем газовую зону
        if (gasZone != null) gasZone.gameObject.SetActive(false);

        if (gasAudio != null && gasAudio.isPlaying) gasAudio.Stop();
        if (gasAnalyzerUI != null) gasAnalyzerUI.ResetValues();

        Debug.Log("Противогаз надет, газовая зона отключена");
    }

    void OnEnable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnGasMaskEquipped += DisableGasZone;
    }

    void OnDisable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnGasMaskEquipped -= DisableGasZone;
    }

    public void DisableGasZone()
    {
        // выключаем газовую зону
        if (gasZone != null) gasZone.gameObject.SetActive(false);

        // стоп звук
        if (gasAudio != null && gasAudio.isPlaying) gasAudio.Stop();

        // сброс значений
        if (gasAnalyzerUI != null) gasAnalyzerUI.ResetValues();

        Debug.Log("Газовая зона выключена после надевания противогаза");
    }
}