using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    //public Image fillImage ;
    public StaminaSystem staminaSystem;
    public Image[] staminaSlots;
  


    private void Update()
    {
        if (staminaSystem == null) return;

        int current = Mathf.RoundToInt(staminaSystem.currentStamina);
        for (int i = 0; i < staminaSlots.Length; i++)
        {
            staminaSlots[i].enabled = i < current;
        }
    }

    //private void Update()
    //{
    //    if (staminaSystem == null)
    //    {
    //        return;
    //    }

    //    Debug.Log("UI stamina = " + staminaSystem.currentStamina);

    //    fillImage.fillAmount =
    //        staminaSystem.currentStamina / staminaSystem.maxStamina;
    //}

}
