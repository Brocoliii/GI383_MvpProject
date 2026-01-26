using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MonsterDarkandShake : MonoBehaviour
{
    public Transform player;
    public Transform monster;

    [Header("Distance")]
    public float startDarkDistance = 6f;
    public float maxDarkDistance = 1.5f;

    [Header("Vignette")]
    public float maxVignette = 0.45f;
    public float vignetteSmooth = 5f;

    [Header("Exposure")]
    public float minExposure = -1.2f;
    public float exposureSmooth = 5f;

    [Header("Heartbeat Sound")]
    public AudioSource heartbeatSource;
    public float maxHeartbeatVolume = 1f;
    public float heartbeatSmooth = 5f;

    [Header("Heartbeat Control")]
    public float safeThreshold = 0.05f; // ต่ำกว่านี้ = ปลอดภัย

    [Header("Heartbeat BPM")]
    public float minBPM = 40f;   // ไกล
    public float maxBPM = 160f;  // ใกล้มาก

    public float heartbeatStartDistance = 15f; // ระยะที่หัวใจเริ่มเต้น

    private float heartbeatTimer;



    private float currentHeartbeatVolume;


    public Volume volume;
    private Vignette vignette;
    private ColorAdjustments colorAdjust;

    private float currentVignette;
    private float currentExposure;

    void Start()
    {
        if (volume == null)
        {
            Debug.LogError("Volume not assigned!");
            return;
        }
        if (heartbeatSource && !heartbeatSource.isPlaying)
        {
            heartbeatSource.loop = false;
            
        }


        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out colorAdjust);
    }

    void Update()
    {
        if (!player || !monster || vignette == null || colorAdjust == null || heartbeatSource == null)
            return;

        float distance = Vector2.Distance(player.position, monster.position);


        float t = Mathf.InverseLerp(startDarkDistance, maxDarkDistance, distance);
        t = Mathf.Clamp01(t);

        float heartT = 1f - Mathf.InverseLerp(
    maxDarkDistance,
    heartbeatStartDistance,
    distance
);
        heartT = Mathf.Clamp01(heartT);



        float targetVignette = Mathf.Lerp(0f, maxVignette, t);
        float targetExposure = Mathf.Lerp(0f, minExposure, t);
        // ----- Heartbeat BPM System -----
        if (heartT <= 0f)
        {
            heartbeatTimer = 0f;
        }
        else
        {
            float bpm = Mathf.Lerp(minBPM, maxBPM, heartT);
            float interval = 60f / bpm;

            heartbeatTimer += Time.deltaTime;

            if (heartbeatTimer >= interval)
            {
                heartbeatSource.volume = Mathf.Lerp(0.2f, maxHeartbeatVolume, heartT);
                heartbeatSource.pitch = Mathf.Lerp(0.9f, 1.15f, heartT);
                heartbeatSource.PlayOneShot(heartbeatSource.clip);
                heartbeatTimer = 0f;
            }
        }






        currentVignette = Mathf.Lerp(
            currentVignette,
            targetVignette,
            Time.deltaTime * vignetteSmooth
        );

        currentExposure = Mathf.Lerp(
            currentExposure,
            targetExposure,
            Time.deltaTime * exposureSmooth
        );


        float pulse = Mathf.Sin(Time.time * 6f) * 0.08f * t;



        vignette.intensity.value = Mathf.Clamp01(currentVignette + pulse);
        colorAdjust.postExposure.value = currentExposure;
    }
}
