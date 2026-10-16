using System;
using System.Collections;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    public Flange flange;

    [Header("State")]
    public bool isUnscrewed;
    public bool isRemoved;                // визуально снят (дети выключены)
    public bool isLockedAfterTighten;     // после закрутки больше не интерактивен
    public bool isBusy;                  // идет анимация/процесс

    [Header("Highlight")]
    public Outline outline;
    public float outlineWidth = 3f;

    [Header("Animation")]
    public Animator animator;            // можно назначить на болт или на ребенка
    public string boltActionTrigger = "BoltAction";
    public float unscrewDuration = 2f; // сколько ждать
    public float tightenDuration = 2f; 

    [Header("Sound")]
    public AudioSource audioSource;      // можно на этом же объекте
    public AudioClip boltClip;

    private Collider[] colliders;

    [Header("Order")]
    public int boltNumber;

    void Awake()
    {
        if (outline == null) outline = GetComponent<Outline>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        colliders = GetComponentsInChildren<Collider>(true);
        DisableHighlight();
    }

    public void EnableHighlight()
    {
        if (outline != null) outline.OutlineWidth = outlineWidth;
    }

    public void DisableHighlight()
    {
        if (outline != null) outline.OutlineWidth = 0f;
    }

    public void SetInteractable(bool value)
    {
        if (!value) DisableHighlight();
        foreach (var c in colliders) c.enabled = value;
    }

    public void TryUnscrew()
    {
        if (isBusy) return;
        if (isLockedAfterTighten) return;
        if (isUnscrewed) return;
        if (flange == null) return;

        if (TrainingManager.Instance != null)
        {
            if (!TrainingManager.Instance.CanWorkWithFlange(flange))
                return;
        }

        if (!PlayerInventory.Instance.HasHandItem(HandItemType.Wrench))
            return;

        if (!ExamController.Instance.CanUnscrewOnFlange(flange))
            return;

        StartCoroutine(UnscrewRoutine());
    }

    private IEnumerator UnscrewRoutine()
    {
        isBusy = true;

        SetInteractable(false);

        if (audioSource != null && boltClip != null)
            audioSource.PlayOneShot(boltClip);

        if (animator != null && !string.IsNullOrEmpty(boltActionTrigger))
            animator.SetTrigger(boltActionTrigger);

        yield return new WaitForSeconds(unscrewDuration);

        isUnscrewed = true;
        isRemoved = true;

        // сообщаем фланцу
        flange.OnBoltUnscrewed(this);
        flange.CheckUnscrewOrder(this);

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        isBusy = false;
    }

    public void Tighten(Action onFinished = null)
    {
        if (isBusy) return;
        if (isLockedAfterTighten) return;

        // закручивать есть смысл только если он снят
        if (!isRemoved) return;

        StartCoroutine(TightenRoutine(onFinished));
    }

    private IEnumerator TightenRoutine(Action onFinished)
    {
        isBusy = true;

        // возвращаем визуал болта сразу
        foreach (Transform child in transform)
            child.gameObject.SetActive(true);

        // звук
        if (audioSource != null && boltClip != null)
            audioSource.PlayOneShot(boltClip);

        // анимация
        if (animator != null && !string.IsNullOrEmpty(boltActionTrigger))
            animator.SetTrigger(boltActionTrigger);

        yield return new WaitForSeconds(tightenDuration);

        // состояние
        isRemoved = false;
        isUnscrewed = false;

        // блокируем навсегда
        isLockedAfterTighten = true;

        // больше никак не взаимодействовать
        DisableHighlight();
        SetInteractable(false);

        // сообщаем фланцу
        if (flange != null)
        {
            flange.OnBoltTightened();
            flange.CheckTightenOrder(this);
        }

        isBusy = false;

        onFinished?.Invoke();
    }
}