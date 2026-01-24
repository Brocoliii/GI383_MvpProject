using System.Collections;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    [Header("Settings")]
    public float maxStamina = 10f; 
    public float currentStamina;
    public float regenAmount = 1f;
    public float regenInterval = 0.5f;

    [HideInInspector] public bool isPaused = false;

    private void Start()
    {
        currentStamina = maxStamina;
        StartCoroutine(RegenerateStamina());
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            return true;
        }
        return false;
    }

    IEnumerator RegenerateStamina()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenInterval);
            if (!isPaused && currentStamina < maxStamina)
            {
                currentStamina = Mathf.Clamp(currentStamina + regenAmount, 0, maxStamina);
            }
        }
    }
}
