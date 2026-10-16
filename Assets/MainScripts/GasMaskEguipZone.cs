using UnityEngine;

public class GasMaskEquipZone : MonoBehaviour
{
    private GasReleaseController controller;

    public void Init(GasReleaseController c) => controller = c;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        controller?.OnPlayerEnterEquipZone();

        if (TrainingManager.Instance != null)
            TrainingManager.Instance.OnEnteredEquipZone();
    }
}