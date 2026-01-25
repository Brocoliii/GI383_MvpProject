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

        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out colorAdjust);
    }

    void Update()
    {
        if (!player || !monster || vignette == null || colorAdjust == null)
            return;

        float distance = Mathf.Abs(player.position.y - monster.position.y);

        float t = Mathf.InverseLerp(startDarkDistance, maxDarkDistance, distance);
        t = Mathf.Clamp01(t);


        float targetVignette = Mathf.Lerp(0f, maxVignette, t);
        float targetExposure = Mathf.Lerp(0f, minExposure, t);


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
