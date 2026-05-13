using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public struct MaterialAudioData
{
    [Header("Identity")]
    public string materialName;
    public string evidenceID;

    [Header("Mask")]
    public Color maskColor;

    [Header("Audio")]
    public AudioClip scanSound;

    [Range(0f, 1f)]
    public float volume;

    [Header("Evidence")]
    public float requiredScanTime;
}

[RequireComponent(typeof(AudioSource))]
public class FoodScan : MonoBehaviour
{
    [Header("Setup")]
    public SpriteRenderer foodRenderer;
    public Texture2D audioMask;

    [Header("Input")]
    public int mouseButton = 0; // 0 = left click
    public bool ignoreWhenPointerOverUI = true;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public bool loopSound = true;
    public float globalVolumeMultiplier = 1f;
    public float colorTolerance = 0.1f;

    [Header("Debug")]
    public bool debugScan = false;

    [Header("Material Database")]
    public List<MaterialAudioData> materialDatabase = new List<MaterialAudioData>();

    private string currentEvidenceID;
    private float scanTimer;
    private bool evidenceUnlockedForCurrentRegion;
    private bool isPlayingMaterialSound;

    private Color lastDebugColor;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = loopSound;
        audioSource.pitch = 1f;
    }

    private void Update()
    {
        if (foodRenderer == null || audioMask == null || Camera.main == null)
        {
            ResetScan();
            StopAudio();
            return;
        }

        if (ignoreWhenPointerOverUI &&
            EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            ResetScan();
            StopAudio();
            return;
        }

        if (!Input.GetMouseButton(mouseButton))
        {
            ResetScan();
            StopAudio();
            return;
        }

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (!foodRenderer.bounds.Contains(mouseWorldPos))
        {
            ResetScan();
            StopAudio();
            return;
        }

        ScanColorAndPlaySound(mouseWorldPos);
    }

    private void ScanColorAndPlaySound(Vector2 worldPos)
    {
        Color hitColor = GetColorFromMask(worldPos);

        if (!TryFindMaterial(hitColor, out MaterialAudioData matchedMaterial))
        {
            ResetScan();
            StopAudio();
            return;
        }

        PlayMaterialSound(matchedMaterial);
        TrackAudibleEvidence(matchedMaterial);
    }

    private bool TryFindMaterial(Color hitColor, out MaterialAudioData matchedMaterial)
    {
        foreach (MaterialAudioData material in materialDatabase)
        {
            float colorDistance = Vector4.Distance(hitColor, material.maskColor);

            if (colorDistance <= colorTolerance)
            {
                matchedMaterial = material;
                return true;
            }
        }

        matchedMaterial = default;
        return false;
    }

    private void PlayMaterialSound(MaterialAudioData material)
    {
        if (material.scanSound == null)
        {
            StopAudio();
            return;
        }

        audioSource.loop = loopSound;
        audioSource.pitch = 1f;
        audioSource.volume = Mathf.Clamp01(material.volume * globalVolumeMultiplier);

        if (audioSource.clip != material.scanSound)
        {
            audioSource.clip = material.scanSound;
            audioSource.Play();
            isPlayingMaterialSound = true;
            return;
        }

        if (!isPlayingMaterialSound || !audioSource.isPlaying)
        {
            audioSource.Play();
            isPlayingMaterialSound = true;
        }
    }

    private void TrackAudibleEvidence(MaterialAudioData material)
    {
        if (currentEvidenceID != material.evidenceID)
        {
            currentEvidenceID = material.evidenceID;
            scanTimer = 0f;
            evidenceUnlockedForCurrentRegion = false;
        }

        if (evidenceUnlockedForCurrentRegion)
            return;

        scanTimer += Time.deltaTime;

        if (scanTimer >= material.requiredScanTime)
        {
            evidenceUnlockedForCurrentRegion = true;

            if (EvidenceNotebook.Instance != null)
            {
                EvidenceNotebook.Instance.AddAudibleEvidence(material.evidenceID);
            }
            else
            {
                Debug.LogWarning("[FoodScan] EvidenceNotebook missing in scene.");
            }
        }
    }

    private Color GetColorFromMask(Vector2 worldPos)
    {
        Vector2 localPos = foodRenderer.transform.InverseTransformPoint(worldPos);
        Bounds bounds = foodRenderer.sprite.bounds;

        float u = Mathf.InverseLerp(bounds.min.x, bounds.max.x, localPos.x);
        float v = Mathf.InverseLerp(bounds.min.y, bounds.max.y, localPos.y);

        int pixelX = Mathf.Clamp(
            Mathf.RoundToInt(u * (audioMask.width - 1)),
            0,
            audioMask.width - 1
        );

        int pixelY = Mathf.Clamp(
            Mathf.RoundToInt(v * (audioMask.height - 1)),
            0,
            audioMask.height - 1
        );

        Color pixelColor = audioMask.GetPixel(pixelX, pixelY);

        if (debugScan && Vector4.Distance(pixelColor, lastDebugColor) > 0.05f)
        {
            Debug.Log(
                "[FoodScan] Pixel: " + pixelX + "," + pixelY +
                " | Color: " + pixelColor
            );

            lastDebugColor = pixelColor;
        }

        return pixelColor;
    }

    private void ResetScan()
    {
        scanTimer = 0f;
        currentEvidenceID = "";
        evidenceUnlockedForCurrentRegion = false;
    }

    private void StopAudio()
    {
        if (!isPlayingMaterialSound)
            return;

        audioSource.Stop();
        isPlayingMaterialSound = false;
    }
}