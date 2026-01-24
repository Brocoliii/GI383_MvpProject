using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audioSource;
    private bool isDead = false;
    private bool isFinished = false;

    [Header("-- Movement --")]
    public float unitSize = 1.0f;
    public float climbDelay = 0.3f;
    public float descendSpeed = 0.5f;
    public float smoothTime = 0.1f;
    public float finishLineY = 50f;

    [Header("-- Audio & Animation --")]
    public AudioClip pullSound;   
    public AudioClip[] climbLevelSounds; 
    private AudioClip currentClimbSound;

    private float targetY;
    private Vector2 currentVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        targetY = transform.position.y;
        rb.gravityScale = 0;
    }

    public void Climb(float force, int soundIndex)
    {
        if (isDead || isFinished) return;

        if (soundIndex >= 0 && soundIndex < climbLevelSounds.Length)
            currentClimbSound = climbLevelSounds[soundIndex];

        StartCoroutine(ClimbRoutine(force));
    }

    private IEnumerator ClimbRoutine(float force)
    {
        if (anim != null) anim.SetTrigger("Pull");
        if (pullSound != null) audioSource.PlayOneShot(pullSound);

        yield return new WaitForSeconds(climbDelay);

        if (!isDead && !isFinished)
        {
            if (anim != null) anim.SetTrigger("ClimbLaunch");
            if (currentClimbSound != null) audioSource.PlayOneShot(currentClimbSound);

            targetY = transform.position.y + (force * unitSize);
        }
    }

    private void FixedUpdate()
    {
        if (isDead || isFinished) return;

        if (transform.position.y >= finishLineY)
        {
            isFinished = true;
            GameManager.Instance.WinStage();
            return;
        }

        targetY -= descendSpeed * Time.fixedDeltaTime;
        float nextY = Mathf.SmoothDamp(transform.position.y, targetY, ref currentVelocity.y, smoothTime);
        rb.MovePosition(new Vector2(transform.position.x, nextY));
    }

    public void GameOver()
    {
        if (isDead || isFinished) return;
        isDead = true;

        if (anim != null) anim.SetTrigger("Eaten");

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        GameManager.Instance.GameOver();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (  other.CompareTag("Monster"))
        {
            GameOver();
        }
    }
}