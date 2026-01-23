using System.Collections;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    [Header("Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float regenInterval = 1f;
    public bool isUsingStamina;
    private Coroutine regenRoutine;



    private void Start()
    {
        currentStamina = maxStamina;
        regenRoutine = StartCoroutine(RegenerateStamina());
    }

    private void Update()
    {
        
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

    IEnumerator RegenerateStamina()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenInterval);
            if (!isUsingStamina && currentStamina < maxStamina)
            {
                currentStamina += 1;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            }
        }
    }
}


