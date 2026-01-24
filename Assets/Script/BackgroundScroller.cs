using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Header("-- References --")]
    public PlayerController player;
    public SpriteRenderer[] tiles;

    [Header("-- Sprites per Phase --")]
    public Sprite deepSprite;         
    public Sprite deepToMidSprite;   
    public Sprite midSprite;         
    public Sprite midToShallowSprite; 
    public Sprite shallowSprite;      

    [Header("-- Settings --")]
    public float tileSize; 

    void Update()
    {
        if (player == null) return;

        float progress = Mathf.Clamp01(player.transform.position.y / player.finishLineY);

        foreach (SpriteRenderer tile in tiles)
        {
            if (player.transform.position.y - tile.transform.position.y > tileSize)
            {
                tile.transform.position += new Vector3(0, tileSize * tiles.Length, 0);

                tile.sprite = GetSpriteByProgress(progress);
            }
        }
    }

    Sprite GetSpriteByProgress(float p)
    {
        if (p < 0.4f) return deepSprite;
        if (p < 0.5f) return deepToMidSprite;
        if (p < 0.8f) return midSprite;
        if (p < 0.9f) return midToShallowSprite;
        return shallowSprite;
    }
}