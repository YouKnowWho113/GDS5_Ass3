using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ToolCursorVisual : MonoBehaviour
{
    [Header("Cursor")]
    public bool hideSystemCursor = true;

    [Header("Blocked / Menu Cursor")]
    public Sprite menuCursorSprite;
    public Vector2 menuCursorOffset = Vector2.zero;
    public float menuCursorRotation = 0f;
    public Vector2 menuCursorScale = Vector2.one;

    [Header("Input")]
    public int lightScanButton = 1; // right mouse
    public int soundScanButton = 0; // left mouse

    [Header("Flashlight Visual")]
    public Sprite flashlightSprite;
    public Vector2 flashlightOffset = Vector2.zero;
    public float flashlightRotation = -35f;
    public Vector2 flashlightScale = Vector2.one;

    [Header("Flashlight Twitch")]
    public bool twitchWhenLightScanning = true;
    public float twitchAmount = 0.04f;
    public float twitchSpeed = 35f;
    public float rotationTwitchAmount = 3f;

    [Header("Chopstick Animation")]
    public Sprite[] chopstickFrames;
    public Vector2 chopstickOffset = Vector2.zero;
    public float chopstickRotation = -35f;
    public Vector2 chopstickScale = Vector2.one;
    public float chopstickFrameRate = 12f;

    [Header("Idle")]
    public bool showFlashlightWhenIdle = true;

    private SpriteRenderer spriteRenderer;
    private float frameTimer;
    private int frameIndex;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (hideSystemCursor)
            Cursor.visible = false;
    }

    private void OnDestroy()
    {
        if (hideSystemCursor)
            Cursor.visible = true;
    }

    private void Update()
    {
        if (GameplayInputLock.IsLocked)
        {
            spriteRenderer.enabled = false;
            Cursor.visible = true;
            return;
        }
        else
        {
            Cursor.visible = false;
        }

        bool usingLightScanner = Input.GetMouseButton(lightScanButton);
        bool usingSoundScanner = Input.GetMouseButton(soundScanButton);

        if (usingSoundScanner)
        {
            ShowChopsticks();
        }
        else if (usingLightScanner)
        {
            ShowFlashlight(true);
        }
        else
        {
            if (showFlashlightWhenIdle)
                ShowFlashlight(false);
            else
                spriteRenderer.enabled = false;
        }
    }

    private void ShowFlashlight(bool scanning)
    {
        if (flashlightSprite == null)
        {
            spriteRenderer.enabled = false;
            return;
        }

        spriteRenderer.enabled = true;
        spriteRenderer.sprite = flashlightSprite;

        Vector3 finalLocalPosition = new Vector3(
            flashlightOffset.x,
            flashlightOffset.y,
            0f
        );

        float finalRotation = flashlightRotation;

        if (scanning && twitchWhenLightScanning)
        {
            float twitchX = Mathf.Sin(Time.time * twitchSpeed) * twitchAmount;
            float twitchY = Mathf.Cos(Time.time * twitchSpeed * 0.7f) * twitchAmount;
            float twitchRot = Mathf.Sin(Time.time * twitchSpeed * 0.8f) * rotationTwitchAmount;

            finalLocalPosition += new Vector3(twitchX, twitchY, 0f);
            finalRotation += twitchRot;
        }

        transform.localPosition = finalLocalPosition;
        transform.localRotation = Quaternion.Euler(0f, 0f, finalRotation);
        transform.localScale = new Vector3(flashlightScale.x, flashlightScale.y, 1f);
    }

    private void ShowChopsticks()
    {
        if (chopstickFrames == null || chopstickFrames.Length == 0)
        {
            ShowFlashlight(false);
            return;
        }

        spriteRenderer.enabled = true;

        frameTimer += Time.deltaTime;

        if (frameTimer >= 1f / chopstickFrameRate)
        {
            frameTimer = 0f;
            frameIndex++;

            if (frameIndex >= chopstickFrames.Length)
                frameIndex = 0;
        }

        spriteRenderer.sprite = chopstickFrames[frameIndex];

        transform.localPosition = new Vector3(
            chopstickOffset.x,
            chopstickOffset.y,
            0f
        );

        transform.localRotation = Quaternion.Euler(0f, 0f, chopstickRotation);
        transform.localScale = new Vector3(chopstickScale.x, chopstickScale.y, 1f);
    }
}