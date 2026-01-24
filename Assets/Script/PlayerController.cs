using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDead = false;

    [Header("-- Movement Settings --")]
    [Tooltip("อันนี้ 1 : 1 ปรับได้ ถ้าเป็น 1 : 1 ตามสไลด์ก็ 4 แรง พุ่ง ไป 4 ช่อง")]
    public float unitSize = 1.0f;

    [Tooltip("หน่วงก่อนจะพุ่งประบได้")]
    public float climbDelay = 0.3f;

    [Tooltip("ตัวไหลลงปรับได้")]
    public float descendSpeed = 0.5f;

    [Tooltip("ปรับน้อยจะพุ่งเร็ว")]
    public float smoothTime = 0.1f;

    private float targetY;
    private Vector2 currentVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetY = transform.position.y;

        
        rb.gravityScale = 0;
    }

    public void Climb(float force)
    {
        if (isDead) return;
        StartCoroutine(ClimbRoutine(force));
    }

    private IEnumerator ClimbRoutine(float force)
    {
        yield return new WaitForSeconds(climbDelay);

        if (!isDead)
        {
            targetY = transform.position.y + (force * unitSize);
            Debug.Log($"Climbing {force} units after {climbDelay}s delay");
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;

    
        targetY -= descendSpeed * Time.fixedDeltaTime;

        float nextY = Mathf.SmoothDamp(transform.position.y, targetY, ref currentVelocity.y, smoothTime);
        rb.MovePosition(new Vector2(transform.position.x, nextY));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Monster"))
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        Debug.Log("Game Over!");
    }
}