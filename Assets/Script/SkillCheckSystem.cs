using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillCheckSystem : MonoBehaviour
{
    public enum PowerType { Low, Medium, High }

    [Header("-- UI Components --")]
    public Image[] slotImages;
    public RectTransform needle;
    public RectTransform perfectFrame;

    [Header("-- Colors --")]
    public Color lowColor = Color.green;
    public Color medColor = Color.yellow;
    public Color highColor = Color.red;

    [Header("-- Gameplay Settings --")]
    public float needleSpeed = 2f;

    public float lowForce = 1f; public float lowCost = 1f;
    public float medForce = 3f; public float medCost = 3f;
    public float highForce = 5f; public float highCost = 5f;
    public float perfectForce = 4f; public float perfectCost = 0f;

    private List<PowerType> barSlots = new List<PowerType>();
    private float needleValue = 0f;
    private bool movingUp = true;
    private bool isPlaying = true;
    private float perfectMin, perfectMax;

    public PlayerController player;
    public StaminaSystem stamina;

    private void Start()
    {
        SetupNewBar();
    }

    void Update()
    {
        if (!isPlaying) return;

        if (Input.GetKeyDown(KeyCode.Space)) stamina.isPaused = true;

        if (Input.GetKey(KeyCode.Space)) MoveNeedle();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            stamina.isPaused = false;
            CheckResult();
        }
    }

    void MoveNeedle()
    {
        float step = Time.deltaTime * needleSpeed;
        needleValue += movingUp ? step : -step;

        if (needleValue >= 1f) { needleValue = 1f; movingUp = false; }
        if (needleValue <= 0f) { needleValue = 0f; movingUp = true; }

        needle.anchorMin = new Vector2(0.5f, needleValue);
        needle.anchorMax = new Vector2(0.5f, needleValue);
    }

    public void SetupNewBar()
    {
        barSlots.Clear();
        for (int i = 0; i < 5; i++)
        {
            PowerType chosen = (PowerType)Random.Range(0, 3);
            if (i > 0 && chosen == PowerType.High && barSlots[i - 1] == PowerType.High)
                chosen = PowerType.Low;
            barSlots.Add(chosen);
        }

        int perfectSlot;
        do { perfectSlot = Random.Range(0, 5); }
        while (barSlots[perfectSlot] == PowerType.High);

        float slotHeight = 1f / 5f;
        perfectMin = perfectSlot * slotHeight;
        perfectMax = perfectMin + slotHeight;

        perfectFrame.anchorMin = new Vector2(0f, perfectMin);
        perfectFrame.anchorMax = new Vector2(1f, perfectMax);
        perfectFrame.offsetMin = Vector2.zero; 
        perfectFrame.offsetMax = Vector2.zero; 

        needleValue = 0f;
        movingUp = true;
        needle.anchorMin = new Vector2(0.5f, 0f);
        needle.anchorMax = new Vector2(0.5f, 0f);
       

        UpdateVisuals();
        isPlaying = true;
    }

    void CheckResult()
    {
        isPlaying = false;
        bool hitPerfect = (needleValue >= perfectMin && needleValue <= perfectMax);
        int slotIndex = Mathf.Clamp(Mathf.FloorToInt(needleValue * 5), 0, 4);

        ApplyPower(barSlots[slotIndex], hitPerfect);
    }

    void ApplyPower(PowerType type, bool isPerfect)
    {
        float f = 0, c = 0;

        if (isPerfect)
        {
            f = perfectForce;
            c = perfectCost; 
        }
        else
        {
            switch (type)
            {
                case PowerType.Low: f = lowForce; c = lowCost; break; 
                case PowerType.Medium: f = medForce; c = medCost; break; 
                case PowerType.High: f = highForce; c = highCost; break;
            }
        }

        if (stamina.UseStamina(c))
        {
            player.Climb(f);

            if (stamina.currentStamina <= 0)
            {
                player.GameOver();
                return;
            }

            Invoke("SetupNewBar", 0.5f);
        }
        else
        {
            player.GameOver();
        }
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (barSlots[i] == PowerType.Low) slotImages[i].color = lowColor;
            else if (barSlots[i] == PowerType.Medium) slotImages[i].color = medColor;
            else slotImages[i].color = highColor;
        }
    }
}