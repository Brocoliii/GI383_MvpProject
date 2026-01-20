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
    public Color greenColor = Color.green;
    public Color yellowColor = Color.yellow;
    public Color redColor = Color.red;

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
    private PlayerController player;
    private StaminaSystem stamina;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        stamina = FindAnyObjectByType<StaminaSystem>();
        SetupNewBar();
    }

    void Update()
    {
        if (!isGameActive) return;

        MoveNeedle();
        if (Input.GetKeyDown(KeyCode.Space) )
        {
            CheckResult();
        }    
    }

    public void SetupNewBar()
    {
        barSlots.Clear();

        int redCount = Random.Range(0, 3);
        for (int i = 0; i < redCount; i ++)
        {
            barSlots.Add(PowerType.High);
        }

        int remainingSlots = 5 - redCount;

        int yellowCount = Random.Range(0, remainingSlots + 1);
        for (int i = 0; i < yellowCount;  i++)
        {
            barSlots.Add(PowerType.Medium);
        }

        int greenCount = remainingSlots - yellowCount;
        for (int i = 0;i < greenCount; i ++)
        {
            barSlots[i] = PowerType.Low;
        }

        for (int i = 0; i < barSlots.Count; i++)
        {
            PowerType temp = barSlots[i];
            int randowIndex = Random.Range(i, barSlots.Count);
            barSlots[i] = barSlots[randowIndex];
            barSlots[randowIndex] = temp;
        }

        for (int i = 0; i < slotImages.Length; i++ )
        {
            if (barSlots[i] == PowerType.Low) slotImages[i].color = greenColor;
            else if (barSlots[i] == PowerType.Medium) slotImages[i].color = yellowColor;
            else slotImages[i].color = redColor;
        }

        float randomPos = Random.Range(0.1f, 0.9f);
        perfectFrame.anchorMin = new Vector2(randomPos - 0.05f, 0.5f);
        perfectFrame.anchorMax = new Vector2(randomPos + 0.05f, 0.5f);
        perfectMin = randomPos - 0.05f;
        perfectMax = randomPos + 0.05f;

        isGameActive = true;
    }
    void MoveNeedle()
    {
        if (movingRight) needleValue += Time.deltaTime * needleSoeed;
        else needleValue -= Time.deltaTime * needleSoeed;

        if (needleValue >= 1f ) movingRight = false;
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

    void ApplyResult(PowerType type , bool isPerfect)
    {
        float force = 0;
        float cost = 0; 
        switch (type)
        {
            case PowerType.Low : force = lowForce; cost = lowCost; break;
            case PowerType.Medium : force = medForce; cost = medCost; break;
            case PowerType.High: force = highForce; cost = highCost; break;
        }
        if (isPerfect) cost = perfectCost;

        if (stamina.UseStamina(cost))
        {
            player.ClimbWithPower(force);
        }

        Invoke("SetupNewBar", 1f);
    }
}

