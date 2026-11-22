using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameManager;

// UI for displaying plane heading

// By OS

public class CompassUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float visableDegrees;

    [Header("References")]
    [SerializeField] RawImage image;
    [SerializeField] TextMeshProUGUI headingText;

    float _textWidth;
    float _pixelsPerDegree;
    float _lastShownHeading = -1;


    // Set variables based on inputs
    private void Awake()
    {
        _textWidth = image.texture.width;
        _pixelsPerDegree = _textWidth / 360;

        Rect uv = image.uvRect;
        uv.width = visableDegrees / 360;
        image.uvRect = uv;
    }

    // Get plane heading angle
    private void LateUpdate()
    {
        GameObject player = gm.Player;
        if (player == null) return;

        float heading = player.transform.eulerAngles.y;

        UpdateImage(heading);
        UpdateText(heading);
    }

    // Scroll the repeating image to match the heading
    void UpdateImage(float heading)
    {
        float windowPixels = visableDegrees * _pixelsPerDegree;
        float leftPixel = Mathf.Round(180 + heading * _pixelsPerDegree - windowPixels * 0.5f);

        Rect uv = image.uvRect;
        uv.x = Mathf.Repeat(leftPixel, _textWidth) / _textWidth;
        image.uvRect = uv;
    }

    // Set the number text to display the heading
    void UpdateText(float heading)
    {
        int degrees = Mathf.RoundToInt(heading) % 360;
        if (degrees == _lastShownHeading) return;

        _lastShownHeading = degrees;
        headingText.text = degrees.ToString("000") + "°";
    }
}
