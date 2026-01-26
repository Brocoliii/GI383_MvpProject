using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Header("-- References --")]
    public PlayerController player;
    public SpriteRenderer[] tiles;

    [Header("-- Visual Settings --")]
    public Gradient backgroundGradient;
    public Sprite finishSprite;

    [Header("-- Scroll Settings --")]
    public float tileSize = 32.4f;
    public float overlapAmount = 0.05f;
    public float finishOffset = 0f;

    private bool hasSpawnedFinish;

    void Update()
    {
        if (!player) return;

        float playerY = player.transform.position.y;
        UpdateTileColors(playerY);
        ScrollTiles(playerY);
    }

    void UpdateTileColors(float playerY)
    {
        float progress = Mathf.Clamp01(playerY / player.finishLineY);
        Color bgColor = backgroundGradient.Evaluate(progress);

        foreach (var tile in tiles)
        {
            tile.color = (hasSpawnedFinish && tile.sprite == finishSprite)
                ? Color.white
                : bgColor;
        }
    }

    void ScrollTiles(float playerY)
    {
        foreach (var tile in tiles)
        {
            if (playerY - tile.transform.position.y <= tileSize)
                continue;

            if (hasSpawnedFinish)
                continue;

            float nextY = tile.transform.position.y + tileSize * tiles.Length;

            if (ShouldSpawnFinish(nextY))
            {
                SpawnFinish(tile, nextY);
            }
            else
            {
                RepositionTile(tile, nextY);
            }
        }
    }

    bool ShouldSpawnFinish(float targetY)
    {
        return targetY + tileSize * 0.5f >= player.finishLineY;
    }

    void RepositionTile(SpriteRenderer tile, float targetY)
    {
        tile.transform.position = new Vector3(
            tile.transform.position.x,
            targetY - overlapAmount,
            tile.transform.position.z
        );

        
        tile.flipY = !tile.flipY;
    }

    void SpawnFinish(SpriteRenderer tile, float targetY)
    {
        tile.transform.position = new Vector3(tile.transform.position.x, targetY, tile.transform.position.z);
        tile.sprite = finishSprite;
        tile.flipY = false;

        float spriteHeight = tile.sprite.bounds.size.y;
        float scale = tileSize / spriteHeight;
        tile.transform.localScale = Vector3.one * scale;

        player.finishLineY = targetY + finishOffset;
        hasSpawnedFinish = true;
    }
}
