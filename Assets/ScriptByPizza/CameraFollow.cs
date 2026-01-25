using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Follow")]
    public float smoothTime = 0.15f;
    public float baseOffsetY = 1.5f;

    [Header("Dynamic Offset")]
    public float climbLookUp = 2f;
    public float offsetLerpSpeed = 3f;

    [Header("Dynamic Tilt (Comfort)")]
    public float maxTilt = 1.5f;     //  ลดจาก 4
    public float tiltSmooth = 8f;    //  กลับตรงเร็ว
    public float swaySpeed = 1.2f;   //  ส่ายช้าลง

    private Vector3 velocity;
    private float currentOffsetY;
    private float targetOffsetY;

    private float currentTilt;

    private PlayerController player;

    void Start()
    {
        if (target)
            player = target.GetComponent<PlayerController>();

        currentOffsetY = baseOffsetY;
    }

    void LateUpdate()
    {
        if (!target) return;

        bool isClimbing = player && player.IsClimbing();

        // ===== Offset Y =====
        targetOffsetY = baseOffsetY + (isClimbing ? climbLookUp : 0f);

        currentOffsetY = Mathf.Lerp(
            currentOffsetY,
            targetOffsetY,
            Time.deltaTime * offsetLerpSpeed
        );

        float targetY = target.position.y + currentOffsetY;

        Vector3 targetPos = new Vector3(
            transform.position.x,
            targetY,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );

        // ===== Gentle Camera Tilt =====
        float targetTilt = 0f;

        if (isClimbing)
        {
            targetTilt = Mathf.Sin(Time.time * swaySpeed) * maxTilt;
        }

        currentTilt = Mathf.Lerp(
            currentTilt,
            targetTilt,
            Time.deltaTime * tiltSmooth
        );

        transform.rotation = Quaternion.Euler(0, 0, currentTilt);
    }
}
