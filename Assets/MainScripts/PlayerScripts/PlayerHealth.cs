using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool wearingGasMask;
    public float damagePerSecond = 5f;
    public float health = 100f;

    public void ApplyGas()
    {
        if (wearingGasMask) return;
        health -= damagePerSecond * Time.deltaTime;
    }
}
