using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SMS : MonoBehaviour
{
    public string sms_text = "0";
    public string sms_old;

    public TMP_Text textBox;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (sms_text != sms_old)
        {
            textBox.text = sms_text;
        }

        sms_old = textBox.text;
    }

    public void sendSMS(string sms)
    {
        sms_text = sms;
    }
}
