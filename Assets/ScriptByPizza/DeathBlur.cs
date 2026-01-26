using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class DeathBlur : MonoBehaviour
{
    public Volume volume;

    Vignette vignette;
    ColorAdjustments color;



    Quaternion startRotation;

    private void Awake()
    {
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out color);

        startRotation = transform.rotation;

        // รีเซ็ตค่าเริ่มต้น (ไม่วูบ)
       
        color.postExposure.Override(0f);
        color.saturation.Override(0f);
    }

    public void PlayDeathBlur()
    {
        StopAllCoroutines();
        StartCoroutine(DeathBlurRoutine());
    }

    IEnumerator DeathBlurRoutine()
    {
        float t = 0f;
        float duration = 5f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duration);

          
            

           
            color.postExposure.Override(Mathf.Lerp(0f, -2f, p));
            color.saturation.Override(Mathf.Lerp(0f, -80f, p));

     

        

            yield return null;
        }

       
    
    }

    // เรียกตอนเริ่มรอบใหม่ / respawn
    public void ResetEffect()
    {
        StopAllCoroutines();
        transform.rotation = startRotation;

        vignette.intensity.Override(0f);
        color.postExposure.Override(0f);
        color.saturation.Override(0f);
    }
}
