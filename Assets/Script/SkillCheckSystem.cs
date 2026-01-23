using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class SkillCheckSystem : MonoBehaviour
{
    [Header("-- UI Elements --")]
    public Image[] slotImages;
    public RectTransform needle;
    public RectTransform perfectFrame;

    [Header("-- Bar Colors --")]
    public Color LowColor = Color.green;
    public Color mediamColor = Color.yellow;
    public Color highColor = Color.red;

    [Header("-- Needle Settings --")]
    [UnityEngine.Range(0.5f, 5f)]
    public float needleSoeed = 2f;

    [Header("-- Power & Stamina --")]
    public float lowForce = 3f; public float lowCost = 10f;
    public float medForce = 6f; public float medCost = 20f;
    public float highForce = 10f; public float highCost = 40f;

    [Header("-- Perfect Bonus --")]
    public float perfectCost = 2f;

    public enum PowerType { Low, Medium, High }
    private List<PowerType> barSlots = new List<PowerType>();
    private float needleValue = 0f;
    private bool movingRight = true;
    private bool isGameActive = true;
    private float perfectMin, perfectMax;
    public PlayerController player;
    public StaminaSystem stamina;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        stamina = FindAnyObjectByType<StaminaSystem>();
        SetupNewBar();
    }

    void Update()
    {
        if (!isGameActive) return;

        
        if (Input.GetKey(KeyCode.Space))
        {
            MoveNeedle();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CheckResult();
        }
    }

    public void SetupNewBar()
    {
        barSlots.Clear();

        barSlots.Add(PowerType.High);    
        barSlots.Add(PowerType.Medium);  
        barSlots.Add(PowerType.Low);     

        int highCount = 1;
        int medCount = 1;
        int lowCount = 1;

        int remainingSlots = 5 - barSlots.Count;

        for (int i = 0; i < remainingSlots; i++)
        {
            List<PowerType> available = new List<PowerType>();

            if (highCount < 2) available.Add(PowerType.High);
            if (medCount < 3) available.Add(PowerType.Medium);
            if (lowCount < 3) available.Add(PowerType.Low);

            PowerType chosen;

            do
            {
                chosen = available[Random.Range(0, available.Count)];
            }
            while (
                chosen == PowerType.High &&
                barSlots.Count > 0 &&
                barSlots[^1] == PowerType.High
            );

            barSlots.Add(chosen);

            if (chosen == PowerType.High) highCount++;
            else if (chosen == PowerType.Medium) medCount++;
            else lowCount++;
        }

        for (int i = 0; i < barSlots.Count; i++)
        {
            for (int j = i + 1; j < barSlots.Count; j++)
            {
                if (barSlots[i] == PowerType.High && barSlots[j] == PowerType.High)
                {
                    if (Mathf.Abs(i - j) == 1) continue;
                }
            }

            int rand = Random.Range(i, barSlots.Count);
            (barSlots[i], barSlots[rand]) = (barSlots[rand], barSlots[i]);
        }

        int perfectSlot;
        do
        {
            perfectSlot = Random.Range(0, 5);
        }
        while (barSlots[perfectSlot] == PowerType.High);

        float slotWidth = 1f / 5f;
        perfectMin = perfectSlot * slotWidth;
        perfectMax = perfectMin + slotWidth;

        perfectFrame.anchorMin = new Vector2(perfectMin, 0.5f);
        perfectFrame.anchorMax = new Vector2(perfectMax, 0.5f);

        needleValue = 0f;
        movingRight = true;
        needle.anchorMin = new Vector2(0, 0.5f);
        needle.anchorMax = new Vector2(0, 0.5f);

        UpdateUISlots();
        isGameActive = true;
    }
    void MoveNeedle()
    {
        if (movingRight) needleValue += Time.deltaTime * needleSoeed;
        else needleValue -= Time.deltaTime * needleSoeed;

        if (needleValue >= 1f) movingRight = false;
        if (needleValue <= 0f) movingRight = true;

        needle.anchorMin = new Vector2(needleValue, 0.5f);
        needle.anchorMax = new Vector2(needleValue, 0.5f);
    }
    void CheckResult()
    {
        isGameActive = false;
        bool isPerfect = (needleValue >= perfectMin && needleValue <= perfectMax);
        int slotIndex = Mathf.Clamp(Mathf.FloorToInt(needleValue * 5), 0, 4);
        ApplyResult(barSlots[slotIndex], isPerfect);
    }

    void ApplyResult(PowerType type, bool isPerfect)
    {
        float force = 0;
        float cost = 0;
        switch (type)
        {
            case PowerType.Low: force = lowForce; cost = lowCost; break;
            case PowerType.Medium: force = medForce; cost = medCost; break;
            case PowerType.High: force = highForce; cost = highCost; break;
        }
        if (!stamina.UseStamina(cost))
        {
            player.SendMessage("GameOver");
            return;
        }

        player.ClimbWithPower(force);
        Invoke("SetupNewBar", 1f);

    }

    void UpdateUISlots()
    { 
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i < barSlots.Count)
            {
                if (barSlots[i] == PowerType.Low) slotImages[i].color = LowColor;
                else if (barSlots[i] == PowerType.Medium) slotImages[i].color = mediamColor;
                else slotImages[i].color = highColor;
            }    
        }
    }


}

