using UnityEngine;
using TMPro;

// Manager for UI in the game scene

// By OS

public class GameUIManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float speedScale;

    [Header("References")]
    [SerializeField] TextMeshProUGUI speedKnotsText;
    [SerializeField] TextMeshProUGUI speedMachText;
    [SerializeField] TextMeshProUGUI throttleText;

    PlaneController _plane;


    // Update UI
    private void Update()
    {
        if (_plane == null)
        {
            GameObject player = GameManager.gm.Player;
            if (player == null) return;
            _plane = player.GetComponent<PlaneController>();
        }

        float speed = _plane.Speed * speedScale;

        speedKnotsText.text = $"KT: {Mathf.RoundToInt(SpeedUnits.ToKnots(speed))}";
        speedMachText.text = $"MA: {SpeedUnits.ToMach(speed):F2}";
        throttleText.text = $"{_plane.Throttle * 100:F0} %";
    }
}

// Convert units
public static class SpeedUnits
{
    const float MetresPerSecondToKnots = 1.943844f;
    const float SpeedOfSound = 340.3f;

    public static float ToKnots(float metresPerSecond) => metresPerSecond * MetresPerSecondToKnots;
    public static float ToMach(float metresPerSecond) => metresPerSecond / SpeedOfSound;
}
