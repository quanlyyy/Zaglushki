using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialogUI : MonoBehaviour
{
    [Header("Root")]
    public GameObject root;

    [Header("Text")]
    public TMP_Text messageText;

    [Header("Buttons")]
    public GameObject confirmCancelGroup;
    public GameObject okButton;

    private Action pendingAction;

    void Awake()
    {
        if (root != null)
            root.SetActive(false);
    }

    public void ShowConfirm(string message, Action onConfirm)
    {
        pendingAction = onConfirm;

        if (messageText != null)
            messageText.text = message;

        if (confirmCancelGroup != null)
            confirmCancelGroup.SetActive(true);

        if (okButton != null)
            okButton.SetActive(false);

        if (root != null)
            root.SetActive(true);

        Debug.Log("Dialog: Confirm");
    }

    public void ShowInfo(string message)
    {
        pendingAction = null;

        if (messageText != null)
            messageText.text = message;

        if (confirmCancelGroup != null)
            confirmCancelGroup.SetActive(false);

        if (okButton != null)
            okButton.SetActive(true);

        if (root != null)
            root.SetActive(true);

        Debug.Log("Dialog: Info");
    }


    public void OnConfirmClicked()
    {
        if (root != null)
            root.SetActive(false);

        var action = pendingAction;
        pendingAction = null;

        action?.Invoke();
    }

    public void OnCancelClicked()
    {
        if (root != null)
            root.SetActive(false);

        pendingAction = null;
    }

    public void OnOkClicked()
    {
        if (root != null)
            root.SetActive(false);

        pendingAction = null;
    }
}