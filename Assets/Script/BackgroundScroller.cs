using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Header("-- References --")]
    public PlayerController player;
    public SpriteRenderer[] tiles;

    [Header("-- Smooth Color Settings --")]
    public Gradient backgroundGradient;
    public Sprite finishSprite;

    public float tileSize = 32.4f;
    public float overlapAmount = 0.05f;
    public float finishOffset = 0f;
    private bool hasSpawnedFinish = false;

    void Start()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (i % 2 != 0) tiles[i].flipY = true;
        }
    }

    void Update()
    {
        if (player == null) return;

        float progress = Mathf.Clamp01(player.transform.position.y / player.finishLineY);
        Color currentColor = backgroundGradient.Evaluate(progress);

        foreach (SpriteRenderer tile in tiles)
        {
            if (hasSpawnedFinish && tile.sprite == finishSprite)
                tile.color = Color.white;
            else
                tile.color = currentColor;

            
            if (player.transform.position.y - tile.transform.position.y > tileSize)
            {
                if (!hasSpawnedFinish)
                {
                    float newY = tile.transform.position.y + (tileSize * tiles.Length) - (overlapAmount * (tiles.Length - 1));

                    if (newY + (tileSize / 2f) >= player.finishLineY)
                    {
                        SpawnFinishHouseAt(tile, newY);
                    }
                    else
                    {
                        tile.transform.position = new Vector3(tile.transform.position.x, newY, tile.transform.position.z);
                        tile.flipY = !tile.flipY;
                    }
                }
            }
        }
    }

    void SpawnFinishHouseAt(SpriteRenderer tile, float targetY)
    {
        tile.transform.position = new Vector3(tile.transform.position.x, targetY, tile.transform.position.z);
        tile.sprite = finishSprite;
        tile.flipY = false;

        float spriteHeight = tile.sprite.bounds.size.y;
        float scaleMultiplier = tileSize / spriteHeight;
        tile.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1);

        player.finishLineY = targetY + finishOffset;

        hasSpawnedFinish = true;
    }
}