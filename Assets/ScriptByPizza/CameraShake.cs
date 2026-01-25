using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void Shake(float duration, float strength)
    {
        
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, strength));
    }

    IEnumerator ShakeRoutine(float duration, float strength)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = (Mathf.PerlinNoise(Time.time * 2f, 0f) - 0.5f) * strength;

            // ดึงกล้องลงด้านล่างเป็นหลัก
            float yNoise = Mathf.PerlinNoise(0f, Time.time * 2f);
            float y = -yNoise * strength; // ติดลบ = ลง

            transform.localPosition = originalPos + new Vector3(x * 0.3f, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }


}
