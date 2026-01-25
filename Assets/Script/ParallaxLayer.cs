using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("-- References --")]
    public Transform player;

    [Header("-- Parallax Setting --")]
    [Range(0, 1)]
    public float parallaxStrength;
    public float tileSize;
    public Transform[] tiles;

    private float startY;

     void Start()
    {
        startY = transform.position.y;
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

     void Update()
    {
        float distance = player.position.y * parallaxStrength;
        transform.position = new Vector3(transform.position.x, startY + distance, transform.position.z);

        foreach (Transform tile in tiles)
        {
            if (player.position.y - tile.position.y > tileSize)
            {
                tile.position += new Vector3(0, tileSize * tiles.Length, 0);
            }
        }
    }
}
