using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    [Header("Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float regenRate = 5f;


    private void Start()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (currentStamina < maxStamina )
        {
            currentStamina += regenRate * Time.deltaTime;
        }
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

    public void AddStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0, maxStamina);
    }
}


