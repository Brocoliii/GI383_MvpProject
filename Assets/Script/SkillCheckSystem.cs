using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

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

    [Header("-- Perfect Settings --")]
    public float perfectSize = 0.1f;


    [Header("-- UI Shake --")]
    public RectTransform uiShakeTarget;   
    public float shakeDuration = 0.15f;
    public float shakeStrength = 15f;

    private Vector2 uiOriginalPos;


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
    public GameObject uiRoot; 


    private void Start()
    {
        if (uiShakeTarget != null)
            uiOriginalPos = uiShakeTarget.anchoredPosition;

        SetupNewBar();
    }

    IEnumerator ShakeUI()
    {
        float t = 0f;

        while (t < shakeDuration)
        {
            t += Time.unscaledDeltaTime;

            Vector2 randomOffset = Random.insideUnitCircle * shakeStrength;
            uiShakeTarget.anchoredPosition = uiOriginalPos + randomOffset;

            yield return null;
        }

        uiShakeTarget.anchoredPosition = uiOriginalPos;
    }



    void Update()
    {
        if (!isPlaying) return;

        if (Input.GetKeyDown(KeyCode.Space)) stamina.isPaused = true;
        player.SetHolding(true); //Anim

        if (Input.GetKey(KeyCode.Space)) MoveNeedle();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            stamina.isPaused = false;
            player.SetHolding(false); //Anim
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
        needle.anchoredPosition = Vector2.zero;
    }

    public void SetupNewBar()
    {
        needleValue = 0f;
        movingUp = true;

        if (needle != null)
        {
            needle.anchorMin = new Vector2(0.5f, 0f);
            needle.anchorMax = new Vector2(0.5f, 0f);
            needle.anchoredPosition = Vector2.zero;


        }
        barSlots.Clear();

        List<PowerType> pool = new List<PowerType>();
        pool.Add(PowerType.High);
        pool.Add(PowerType.Medium);
        pool.Add(PowerType.Low);

        List<PowerType> extra = new List<PowerType>
        { PowerType.High,PowerType.Medium,PowerType.Medium,PowerType.Low,PowerType.Low };
        for (int i = 0; i < 2; i++)
        {
            int r = Random.Range(0, extra.Count);
            pool.Add(extra[r]);
            extra.RemoveAt(r);

        }

        for (int i = 0; i < pool.Count; i++)
        {
            PowerType temp = pool[i];
            int r = Random.Range(i, pool.Count);
            pool[i] = pool[r];
            pool[r] = temp;
        }
        barSlots = pool;
        List<int> validPerfectIndices = new List<int>();
        for (int i = 0; i < barSlots.Count; i++)
        {
            if (barSlots[i] != PowerType.High) validPerfectIndices.Add(i);
        }

        //if (validPerfectIndices.Count > 0)
        //{
        //    int perfectSlot = validPerfectIndices[Random.Range(0, validPerfectIndices.Count)];

        //    float slotHeight = 1f / 5f;
        //    perfectMin = perfectSlot * slotHeight;
        //    perfectMax = perfectMin + slotHeight;

        //    perfectFrame.anchorMin = new Vector2(0f, perfectMin);
        //    perfectFrame.anchorMax = new Vector2(1f, perfectMax);
        //    perfectFrame.offsetMin = Vector2.zero;
        //    perfectFrame.offsetMax = Vector2.zero;
        //}
        if (validPerfectIndices.Count > 0)
        {
            int perfectSlot = validPerfectIndices[Random.Range(0, validPerfectIndices.Count)];
            float slotHeight = 1f / 5f;

            UpdatePerfectVisual(perfectSlot);


            float actualHeight = slotHeight * perfectSize;
            float offsetInsideSlot = (slotHeight - actualHeight) / 2f;

            perfectMin = (perfectSlot * slotHeight) + offsetInsideSlot;
            perfectMax = perfectMin + actualHeight;
         


        }
        else
        {
            Debug.LogError("SkillCheckSystem: perfect frame missing");
        }


        UpdateVisuals();
        isPlaying = true;


    }

    void UpdatePerfectVisual(int slotIndex)
    {
        if (perfectFrame == null) return;

        RectTransform parent = perfectFrame.parent as RectTransform;
        float parentHeight = parent.rect.height;
        float slotHeightPx = parentHeight / 5f;

        float visualHeight = slotHeightPx * perfectSize;
        perfectFrame.sizeDelta = new Vector2(
            perfectFrame.sizeDelta.x,
            visualHeight
        );

       
        float y = -(parentHeight / 2f)
                  + slotHeightPx * slotIndex
                  + slotHeightPx / 2f;

        perfectFrame.anchoredPosition = new Vector2(
            perfectFrame.anchoredPosition.x,
            y
        );
    }




    bool IsNeedleInsidePerfect()
    {
        Vector3[] perfectCorners = new Vector3[4];
        perfectFrame.GetWorldCorners(perfectCorners);

        Vector3 needleWorldPos = needle.position;

        return needleWorldPos.y >= perfectCorners[0].y &&
               needleWorldPos.y <= perfectCorners[1].y;
    }



    void CheckResult()
    {
        isPlaying = false;

        bool hitPerfect = IsNeedleInsidePerfect();

        float slotHeight = 1f / 5f;
        int slotIndex = Mathf.Clamp(
            Mathf.FloorToInt(needleValue / slotHeight),
            0, 4
        );

        ApplyPower(barSlots[slotIndex], hitPerfect);
    }



    public void HideUI()
    {
        isPlaying = false;

        if (uiRoot != null)
            uiRoot.SetActive(false);
    }


    void ApplyPower(PowerType type, bool isPerfect)
    {
        shakeStrength = isPerfect ? 3f : (type == PowerType.High ? 25f : 10f);

        if (uiShakeTarget != null)
            StartCoroutine(ShakeUI());

        float f = 0, c = 0;
        int soundIdx = 0;

        if (isPerfect)
        {
            Debug.Log("hit");
            SoundManager.Instance.PlaySFX("PerfectHit", 3f);
            f = perfectForce;
            c = perfectCost;
            soundIdx = 0;
        }
        else
        {
            Debug.Log("hitcolor");
            SoundManager.Instance.PlaySFX("ColorHit" ,2f);
            switch (type)
            {
                case PowerType.Low: f = lowForce; c = lowCost; break;
                case PowerType.Medium: f = medForce; c = medCost; break;
                case PowerType.High: f = highForce; c = highCost; break;
            }
        }

        if (stamina.UseStamina(c))
        {
            player.LaunchClimb(); //Anim
            player.Climb(f, soundIdx);
            if (stamina.currentStamina <= 0)
            {
                player.GameOver("ขาดอากาศหายใจ");
                return;
            }

            Invoke("SetupNewBar", 0.5f);
        }
        else
        {
            player.GameOver("แรงหมด/อากาศหมด");
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