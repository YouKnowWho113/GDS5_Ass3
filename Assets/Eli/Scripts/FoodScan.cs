using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MaterialAudioData
{
    public string materialName;
    public Color maskColor;
    public AudioClip dragSound;

    [Range(0.5f, 2f)]
    public float basePitch;
}

[RequireComponent(typeof(AudioSource))]
public class FoodScan : MonoBehaviour
{
    [Header("Setup")]
    public SpriteRenderer foodRenderer;
    public Texture2D audioMask;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public float maxSpeedThreshold = 15f;
    public float colorTolerance = 0.1f;

    [Header("Debug")]
    public bool debugScan = true;

    [Header("Material Database")]
    public List<MaterialAudioData> materialDatabase;

    private Vector2 lastMousePos;
    private MaterialAudioData currentMaterial;
    private bool isPlayingMaterialSound = false;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.loop = true;
    }

    void Update()
    {
        if (foodRenderer == null || audioMask == null)
        {
            Debug.LogWarning("[FoodScan] Missing foodRenderer or audioMask.");
            return;
        }

        Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            bool insideFood = foodRenderer.bounds.Contains(currentMousePos);

            if (debugScan)
            {
                Debug.Log("[FoodScan] Mouse held. World Pos: " + currentMousePos + " | Inside food: " + insideFood);
            }

            if (insideFood)
            {
                ProcessAudioForensics(currentMousePos);
            }
            else
            {
                StopAudio("Mouse outside food sprite");
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopAudio("Mouse released");
        }

        lastMousePos = currentMousePos;
    }

    private void ProcessAudioForensics(Vector2 mousePos)
    {
        float mouseSpeed = Vector2.Distance(mousePos, lastMousePos) / Time.deltaTime;

        if (debugScan)
        {
            Debug.Log("[FoodScan] Mouse speed: " + mouseSpeed);
        }

        if (mouseSpeed < 0.1f)
        {
            StopAudio("Mouse too slow");
            return;
        }

        Color hitColor = GetColorFromMask(mousePos);

        if (debugScan)
        {
            Debug.Log("[FoodScan] Scanned mask color: " + hitColor);
        }

        bool foundMaterial = false;

        foreach (var mat in materialDatabase)
        {
            float colorDistance = Vector4.Distance(hitColor, mat.maskColor);

            if (debugScan)
            {
                Debug.Log("[FoodScan] Checking: " + mat.materialName +
                          " | Target Color: " + mat.maskColor +
                          " | Distance: " + colorDistance);
            }

            if (colorDistance < colorTolerance)
            {
                currentMaterial = mat;
                foundMaterial = true;

                if (debugScan)
                {
                    Debug.Log("[FoodScan] MATCH FOUND: " + mat.materialName);
                }

                break;
            }
        }

        if (foundMaterial)
        {
            if (audioSource.clip != currentMaterial.dragSound)
            {
                audioSource.clip = currentMaterial.dragSound;
                audioSource.Play();
                isPlayingMaterialSound = true;

                if (debugScan)
                {
                    Debug.Log("[FoodScan] Playing new sound: " + currentMaterial.materialName);
                }
            }
            else if (!isPlayingMaterialSound)
            {
                audioSource.Play();
                isPlayingMaterialSound = true;

                if (debugScan)
                {
                    Debug.Log("[FoodScan] Resuming sound: " + currentMaterial.materialName);
                }
            }

            float speedRatio = Mathf.Clamp01(mouseSpeed / maxSpeedThreshold);

            audioSource.volume = Mathf.Lerp(0.2f, 1.0f, speedRatio);
            audioSource.pitch = Mathf.Lerp(
                currentMaterial.basePitch - 0.2f,
                currentMaterial.basePitch + 0.3f,
                speedRatio
            );

            if (debugScan)
            {
                Debug.Log("[FoodScan] Volume: " + audioSource.volume +
                          " | Pitch: " + audioSource.pitch);
            }
        }
        else
        {
            StopAudio("No matching material color");
        }
    }

    private Color GetColorFromMask(Vector2 worldPos)
    {
        Vector2 localPos = foodRenderer.transform.InverseTransformPoint(worldPos);

        Bounds bounds = foodRenderer.sprite.bounds;

        float u = Mathf.InverseLerp(bounds.min.x, bounds.max.x, localPos.x);
        float v = Mathf.InverseLerp(bounds.min.y, bounds.max.y, localPos.y);

        int pixelX = Mathf.Clamp(Mathf.RoundToInt(u * audioMask.width), 0, audioMask.width - 1);
        int pixelY = Mathf.Clamp(Mathf.RoundToInt(v * audioMask.height), 0, audioMask.height - 1);

        Color pixelColor = audioMask.GetPixel(pixelX, pixelY);

        if (debugScan)
        {
            Debug.Log("[FoodScan] Local Pos: " + localPos +
                      " | UV: " + new Vector2(u, v) +
                      " | Pixel: " + pixelX + "," + pixelY +
                      " | Color: " + pixelColor);
        }

        return pixelColor;
    }

    private void StopAudio(string reason)
    {
        if (isPlayingMaterialSound)
        {
            audioSource.Pause();
            isPlayingMaterialSound = false;

            if (debugScan)
            {
                Debug.Log("[FoodScan] Audio stopped. Reason: " + reason);
            }
        }
    }
}