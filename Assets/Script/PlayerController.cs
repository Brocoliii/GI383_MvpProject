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

    [Header("-- Oxygen Bubble --")]
    public float idleBubbleRate = 3f;     // หายใจปกติ
    public float climbBubbleRate = 12f;   // ตอนปีน
    public float climbBubbleDelay = 0.15f;

    private float targetY;
    private Vector2 currentVelocity;

 
    public ParticleSystem oxygenBubbleFX;
    private Coroutine bubbleRoutine;

    public SkillCheckSystem skillCheck;

    private void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        targetY = transform.position.y;
        rb.gravityScale = 0;
        SetBubbleRate(idleBubbleRate);
        SetBubbleSize(0.1f);

    }

    public void Climb(float force, int soundIndex)
    {
        if (isDead || isFinished) return;

        if (soundIndex >= 0 && soundIndex < climbLevelSounds.Length)
            currentClimbSound = climbLevelSounds[soundIndex];

        StartCoroutine(ClimbRoutine(force));
    }

    void SetBubbleRate(float rate)
    {
        if (!oxygenBubbleFX) return;

        var emission = oxygenBubbleFX.emission;
        emission.rateOverTime = rate;

        if (!oxygenBubbleFX.isPlaying)
            oxygenBubbleFX.Play();
    }

    void SetBubbleSize(float startSize)
    {
        if (!oxygenBubbleFX) return;

        var main = oxygenBubbleFX.main;
        main.startSize = startSize;
    }




    private IEnumerator ClimbRoutine(float force)
    {
        if (anim != null) anim.SetTrigger("Pull");

        SetBubbleRate(climbBubbleRate);
        SetBubbleSize(0.2f);


        if (pullSound != null) audioSource.PlayOneShot(pullSound);



        yield return new WaitForSeconds(climbDelay);

        if (!isDead && !isFinished)
        {
            if (anim != null) anim.SetTrigger("ClimbLaunch");
            if (currentClimbSound != null) audioSource.PlayOneShot(currentClimbSound);
            




            targetY = transform.position.y + (force * unitSize);
        }
        SetBubbleRate(idleBubbleRate);
        SetBubbleSize(0.1f);
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

    public void GameOver(string reason)
    {
        if (isDead || isFinished) return;
        isDead = true;

        if (skillCheck != null)
            skillCheck.HideUI();

        if (Camera.main != null)
        {
            DeathBlur blur = Camera.main.GetComponent<DeathBlur>();
            if (blur != null)
                blur.PlayDeathBlur();
            SoundManager.Instance.PlaySFX("OutOfO2", 1.3f);
        }

       

        Debug.Log("<color = red> over </color> สาเหตุ :" + reason);

        if (anim != null) anim.SetTrigger("Eaten");

        if (oxygenBubbleFX)
            oxygenBubbleFX.Stop();

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        GameManager.Instance.GameOver();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            GameOver("โดนมอนกิน!");
        }
    }

    public void SetHolding(bool value)
    {
        anim.SetBool("isHolding", value);
    }

    public void LaunchClimb()
    {
        anim.SetTrigger("Launch");
    }

    public bool IsClimbing()
    {
        return anim.GetBool("isHolding");
    }

}