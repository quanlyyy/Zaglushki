using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GasAnalyzerUI_TMP : MonoBehaviour
{
    public TMP_Text H25;
    public TMP_Text CO;
    public TMP_Text O2;
    public TMP_Text CH4;

    [Header("Safe values (outside zone)")]
    public string safe1 = "0";
    public string safe2 = "0";
    public string safe3 = "0";
    public string safe4 = "0";

    public void SetValues(float v1, float v2, float v3, float v4)
    {
        if (H25) H25.text = v1.ToString();
        if (CO) CO.text = v2.ToString();
        if (O2) O2.text = v3.ToString();
        if (CH4) CH4.text = v4.ToString();
    }

    public void ResetValues()
    {
        if (H25) H25.text = safe1;
        if (CO) CO.text = safe2;
        if (O2) O2.text = safe3;
        if (CH4) CH4.text = safe4;
    }
}