using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    public Image fillImage ;
    private StaminaSystem staminaSystem ;

    private void Start()
    {
        staminaSystem = FindAnyObjectByType<StaminaSystem>();
        fillImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (staminaSystem != null)
        {
            fillImage.fillAmount = staminaSystem.currentStamina / staminaSystem.maxStamina;
        }
    }
}   
