using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowMouse : MonoBehaviour
{
    [Header("Parallax Settings")]
    public bool MoveOnX = true;
    public bool MoveOnY = true;
    public float parallaxStrength = 50f;

    [Range(0f, 1f)]
    public float smoothSpeedMouse = 0.03f;

    [Range(0f, 1f)]
    public float smoothSpeedTouch = 0.02f;

    private RectTransform rectTransform;  // Reference to RectTransform
    private Vector2 initialPosition;     // The starting position of the UI element
    private Vector2 screenCenter;        // The center of the screen

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        initialPosition = rectTransform.anchoredPosition;

        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    private void Update()
    {
        // Get the mouse position or touch position
        Vector2 mousePosition = Input.mousePosition;

        float offsetX = MoveOnX ? (mousePosition.x - screenCenter.x) / screenCenter.x * parallaxStrength : 0f;
        float offsetY = MoveOnY ? (mousePosition.y - screenCenter.y) / screenCenter.y * parallaxStrength : 0f;

        Vector2 targetPosition = initialPosition + new Vector2(offsetX, offsetY);

        float smoothSpeed = Input.touchCount > 0 ? smoothSpeedTouch : smoothSpeedMouse;
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, smoothSpeed);
    }
}