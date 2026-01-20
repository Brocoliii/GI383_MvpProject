using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDead = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ClimbWithPower(float force)
    {
        if (isDead) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);

        Debug.Log("P Clime with force : " + force);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameOver();
        }
        if (other.CompareTag("Monster"))
        {
            GameOver();
        }
    }
    
    void GameOver()
    {
        if (isDead) return;

        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Debug.Log("µØÂ");
    }
}

