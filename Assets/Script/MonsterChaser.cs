using UnityEngine;
using System.Collections;

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

    private float currentSpeed;
    private bool isPlayerDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(RandomGrowlRoutine());

        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null) playerStartY = player.position.y;
    }

    void Update()
    {
        if (isPlayerDead || player == null || !GameManager.Instance.isGameActive) return;

        if (!hasStartedChasing)
        {
            float playerProgress = player.position.y - playerStartY;
            if (playerProgress >= startDistanceThreshold)
            {
                hasStartedChasing = true;
                Debug.Log("!");
            }
            else
            {
                return;
            }
        }

        float distance = player.position.y - transform.position.y;
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

        if (killSound != null) audioSource.PlayOneShot(killSound);

        if (anim != null) anim.SetTrigger("KillPlayer");

        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null) pc.GameOver();
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
