using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class MonsterChaser : MonoBehaviour
{
    [Header("-- References --")]
    public Transform player;
    private Animator anim;
    private AudioSource audioSource;

    [Header("-- Smooth Movement Settings --")]
    public float minSpeed = 2f;      
    public float maxSpeed = 8f;       
    public float safeDistance = 3f;  
    public float maxDistance = 10f;   
    public float acceleration = 5f;

    [Header("-- Wait for Player Settings --")]
    public float startDistanceThreshold = 2.0f;
    private bool hasStartedChasing = false;
    private float playerStartY;

    [Header("-- Audio Settings --")]
    public AudioClip[] randomGrowls;
    public AudioClip killSound;
    public float minGrowlInterval = 5f;
    public float maxGrowlInterval = 10f;

    [Header("-- Camera Shake --")]
    public CameraShake cameraShake;
    public float shakeDuration = 0.1f;
    public float maxShakeStrength = 0.3f;
    public float shakeStartDistance = 3f; // เริ่มสั่นเมื่อใกล้กว่านี้

    [Header("-- Drag Player Down --")]
    public float dragSpeed = 3f;
    public float dragOffsetX = 0.3f; // ระยะเอียงเข้าหาตัว monster
    public float dragDepth = 5f;     // ลากลงไปลึกแค่ไหน

    [Header("-- Kill Camera Shake --")]
    public float killShakeDuration = 0.35f;
    public float killShakeStrength = 0.6f;


    [Header("-- Kill Camera Zoom --")]
    public float killZoomInSize = 3.2f;   // ค่ายิ่งน้อย = zoom เข้า
    public float killZoomSpeed = 1.2f;

    float shakeTimer = 0f;


    private bool isDraggingPlayer = false;

    private float currentSpeed;
    private bool isPlayerDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(RandomGrowlRoutine());

     

        if (cameraShake == null)
            cameraShake = Camera.main.GetComponent<CameraShake>();


        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null) playerStartY = player.position.y;
    }

    void Update()
    {
        if (isPlayerDead || player == null || !GameManager.Instance.isGameActive) return;

        if (isDraggingPlayer)
            return; 

        if (!hasStartedChasing)
        {
            float playerProgress = player.position.y - playerStartY;
            if (playerProgress >= startDistanceThreshold)
            {
                hasStartedChasing = true;
                SoundManager.Instance.PlaySFX("MonChase", 0.5f);
                Debug.Log("!");
            }
            else
            {
                return;
            }
        }

        float distance = Mathf.Abs(player.position.y - transform.position.y);


        if (!isDraggingPlayer && distance <= shakeStartDistance && cameraShake != null)
        {
            float shakeT = Mathf.InverseLerp(shakeStartDistance, 0f, distance);
            float strength = Mathf.Lerp(0.05f, maxShakeStrength, shakeT);

            cameraShake.Shake(0.1f, strength);


        }



        float t = Mathf.InverseLerp(safeDistance, maxDistance, distance);
        float targetSpeed = Mathf.Lerp(minSpeed, maxSpeed, t);

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        transform.Translate(Vector2.up * currentSpeed * Time.deltaTime);

        if (anim != null) anim.SetFloat("Speed", currentSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPlayerDead)
        {
            KillPlayer();
        }
    }

    void KillPlayer()
    {
        isPlayerDead = true;
        isDraggingPlayer = true;

        GameManager.Instance.GameOver("OPERATIVE TERMINATED: CONSUMED BY HOSTILE");

        if (killSound != null) audioSource.PlayOneShot(killSound);
        SoundManager.Instance.PlaySFX("MonsterEat", 6f);
        if (anim != null) anim.SetTrigger("KillPlayer");

        
        if (cameraShake != null)
            cameraShake.Shake(killShakeDuration, killShakeStrength);

        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
            pc.enabled = false;

        player.SetParent(transform);
        player.localPosition = new Vector3(0.3f, -0.7f, 3f);

        CameraFollow follow = Camera.main.GetComponent<CameraFollow>();
        if (follow != null)
            follow.enabled = false;


        StartCoroutine(DragDownRoutine());
    }



    IEnumerator DragDownRoutine()
    {
        Vector3 startPlayerPos = player.localPosition;
        Vector3 targetPlayerPos = startPlayerPos + new Vector3(0f, -dragDepth, 0f);

        Camera cam = Camera.main;
        float startZoom = cam.orthographicSize;
        float targetZoom = killZoomInSize;

        float t = 0f;

        float shakeInterval = 0.25f;

        while (t < 1f)
        {
            t += Time.deltaTime * dragSpeed;

            player.localPosition = Vector3.Lerp(startPlayerPos, targetPlayerPos, t);
            cam.orthographicSize = Mathf.Lerp(startZoom, targetZoom, t);

            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f && cameraShake != null)
            {
                cameraShake.Shake(0.15f, 0.25f);
                shakeTimer = shakeInterval;
            }

            yield return null;
        }

    }




    void OnDrawGizmosSelected() // อันนี้ CHAT GPT ครับ
    {
        // 1. ระยะปลอดภัย (Safe Distance) - สีเขียว
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, safeDistance);

        // 2. ระยะสั่นของกล้อง (Shake Start Distance) - สีเหลือง
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shakeStartDistance);

        // 3. ระยะความเร็วสูงสุด (Max Distance) - สีแดง
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }


    IEnumerator RandomGrowlRoutine()
    {
        while (!isPlayerDead)
        {
            float waitTime = Random.Range(minGrowlInterval, maxGrowlInterval);
            yield return new WaitForSeconds(waitTime);

            if (!isPlayerDead && randomGrowls.Length > 0)
            {
                AudioClip clip = randomGrowls[Random.Range(0, randomGrowls.Length)];
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
