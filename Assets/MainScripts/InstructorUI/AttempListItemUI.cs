using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttemptListItemUI : MonoBehaviour
{
    [Header("Main Texts")]
    public TMP_Text dateText;
    public TMP_Text fullNameText;
    public TMP_Text scoreText;
    public TMP_Text timeText;

    [Header("Details")]
    public TMP_Text detailsText;
    public GameObject detailsRoot;

    [Header("Layout")]
    public float collapsedHeight = 70f;
    public float expandedHeight = 140f;

    private string attemptId;
    private InstructorResultsUI instructorOwner;
    private ProfileSceneUI profileOwner;

    private bool detailsLoaded = false;
    private bool isExpanded = false;

    private LayoutElement layoutElement;

    void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();

        if (layoutElement == null)
            layoutElement = gameObject.AddComponent<LayoutElement>();

        layoutElement.enabled = true;

        layoutElement.preferredHeight = collapsedHeight;

        if (detailsRoot != null)
            detailsRoot.SetActive(false);
    }

    //элемент результата
    public void Setup(
        InstructorResultsUI parent,
        string id,
        string dateUtc,
        string fio,
        int score,
        int maxScore,
        int timeSpentSeconds)
    {
        //ссылка на род юай
        instructorOwner = parent;

        attemptId = id;

        if (dateText != null)
            dateText.text = FormatDateToMoscow(dateUtc);

        if (fullNameText != null)
            fullNameText.text = fio;

        if (scoreText != null)
            scoreText.text = $"{score}/{maxScore}";

        if (timeText != null)
            timeText.text = FormatDuration(timeSpentSeconds);

        if (detailsText != null)
            detailsText.text = "";

        if (detailsRoot != null)
            detailsRoot.SetActive(false);

        detailsLoaded = false;
        isExpanded = false;

        layoutElement.preferredHeight = collapsedHeight;

        ForceLayout();
    }

    //владелец для резульатов профиля
    public void SetProfileOwner(ProfileSceneUI owner)
    {
        profileOwner = owner;
    }

    // открытие/закрытие блока подробнее
    public void ToggleDetails()
    {
        if (!isExpanded)
        {
            if (!detailsLoaded)
            {
                if (instructorOwner != null)
                    instructorOwner.LoadDetails(this, attemptId);

                else if (profileOwner != null)
                    profileOwner.LoadDetails(this, attemptId);
            }
            else
            {
                ShowExpanded();
            }
        }
        else
        {
            ShowCollapsed();
        }
    }

    //записи в подробнее
    public void SetDetails(string text)
    {
        if (detailsText != null)
            detailsText.text = text;

        detailsLoaded = true;

        ShowExpanded();
    }

    //расскрытие подробнее
    private void ShowExpanded()
    {
        isExpanded = true;

        if (detailsRoot != null)
            detailsRoot.SetActive(true);

        layoutElement.preferredHeight = expandedHeight;

        ForceLayout();
    }

    //скрытие подробнее
    private void ShowCollapsed()
    {
        isExpanded = false;

        if (detailsRoot != null)
            detailsRoot.SetActive(false);

        layoutElement.preferredHeight = collapsedHeight;

        ForceLayout();
    }

    private string FormatDuration(int totalSeconds)
    {
        int mins = totalSeconds / 60;
        int secs = totalSeconds % 60;

        return $"{mins:00}:{secs:00}";
    }

    //UTC время в мск
    private string FormatDateToMoscow(string utcString)
    {
        if (string.IsNullOrWhiteSpace(utcString))
            return "-";

        if (!DateTimeOffset.TryParse(utcString, out var dto))
            return utcString;

        var msk = dto.ToOffset(TimeSpan.FromHours(3));

        return msk.ToString("dd-MM-yy HH:mm");
    }

    // обновление layout
    private void ForceLayout()
    {
        Canvas.ForceUpdateCanvases();

        var rect = transform as RectTransform;

        if (rect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

        if (transform.parent is RectTransform parent)
            LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
    }
}