using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("-- References --")]
    public Transform player;

    [Header("-- Parallax Setting --")]
    [Range(0, 1)]
    public float parallaxStrength = 0.5f;
    public float tileSize;
    public Transform[] tiles;

    [Header("-- Stop At Finish --")]
    public float stopOffset = 1f;   // ระยะเผื่อก่อนถึง Finish

    private float startY;
    private float stopY;
    private bool stopped;

    void Start()
    {
        startY = transform.position.y;

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // ดึง finishLine จาก Player
        PlayerController pc = player.GetComponent<PlayerController>();
        stopY = pc.finishLineY - stopOffset;
    }

    void Update()
    {
        if (stopped) return;

        float playerY = player.position.y;

        if (playerY >= stopY)
        {
            stopped = true;
            return;
        }

        // ===== Parallax Movement =====
        float distance = playerY * parallaxStrength;
        transform.position = new Vector3(
            transform.position.x,
            startY + distance,
            transform.position.z
        );

        // ===== Tile Loop =====
        foreach (Transform tile in tiles)
        {
            if (playerY - tile.position.y > tileSize)
            {
                tile.position += Vector3.up * tileSize * tiles.Length;
            }
        }
    }
}

