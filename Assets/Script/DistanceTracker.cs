using UnityEngine;
using TMPro; 

public class DistanceTextDisplay : MonoBehaviour
{
    [Header("-- References --")]
    public PlayerController player;
    public TextMeshProUGUI distanceText;

    [Header("-- Settings --")]
    public string prefix = "Remaining: ";
    public string suffix = " m";

    void Update()
    {
        if (player == null || distanceText == null) return;

        float remainingDistance = player.finishLineY - player.transform.position.y;

        if (remainingDistance < 0) remainingDistance = 0;

        distanceText.text = prefix + remainingDistance.ToString("F1") + suffix;

        if (player.isFinished)
        {
            distanceText.text = "GOAL!";
        }
    }
}
