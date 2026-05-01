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
    public AudioClip dragSound;

    [Range(0.5f, 2f)]
    public float basePitch;

    [Header("Evidence")]
    public float requiredScrapeTime;
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
    public float maxSpeedThreshold = 15f;
    public float minMouseSpeed = 0.1f;
    public float colorTolerance = 0.1f;

    [Header("Debug")]
    public bool debugScan = false;

    [Header("Material Database")]
    public List<MaterialAudioData> materialDatabase = new List<MaterialAudioData>();

    private Vector2 lastMousePos;
    private string currentEvidenceID;
    private float scrapeTimer;
    private bool evidenceUnlockedForCurrentRegion;
    private bool isPlayingMaterialSound;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (foodRenderer == null || audioMask == null)
        {
            StopAudio();
            return;
        }

        if (Camera.main == null)
        {
            StopAudio();
            return;
        }

        Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (ignoreWhenPointerOverUI &&
            EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            ResetScrape();
            StopAudio();
            lastMousePos = currentMousePos;
            return;
        }

        if (Input.GetMouseButton(mouseButton))
        {
            bool insideFood = foodRenderer.bounds.Contains(currentMousePos);

            if (insideFood)
            {
                ProcessAudioForensics(currentMousePos);
            }
            else
            {
                ResetScrape();
                StopAudio();
            }
        }
        else
        {
            ResetScrape();
            StopAudio();
        }

        lastMousePos = currentMousePos;
    }

    private void ProcessAudioForensics(Vector2 mousePos)
    {
        float deltaTime = Mathf.Max(Time.deltaTime, 0.0001f);
        float mouseSpeed = Vector2.Distance(mousePos, lastMousePos) / deltaTime;

        if (mouseSpeed < minMouseSpeed)
        {
            StopAudio();
            return;
        }

        Color hitColor = GetColorFromMask(mousePos);

        if (!TryFindMaterial(hitColor, out MaterialAudioData matchedMaterial))
        {
            ResetScrape();
            StopAudio();
            return;
        }

        PlayMaterialSound(matchedMaterial, mouseSpeed);
        TrackSoundEvidence(matchedMaterial);
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

    private void PlayMaterialSound(MaterialAudioData material, float mouseSpeed)
    {
        if (material.dragSound == null)
        {
            StopAudio();
            return;
        }

        if (audioSource.clip != material.dragSound)
        {
            audioSource.clip = material.dragSound;
            audioSource.Play();
            isPlayingMaterialSound = true;
        }
        else if (!isPlayingMaterialSound)
        {
            audioSource.Play();
            isPlayingMaterialSound = true;
        }

        float speedRatio = Mathf.Clamp01(mouseSpeed / maxSpeedThreshold);

        audioSource.volume = Mathf.Lerp(0.2f, 1.0f, speedRatio);
        audioSource.pitch = Mathf.Lerp(
            material.basePitch - 0.2f,
            material.basePitch + 0.3f,
            speedRatio
        );
    }

    private void TrackSoundEvidence(MaterialAudioData material)
    {
        if (currentEvidenceID != material.evidenceID)
        {
            currentEvidenceID = material.evidenceID;
            scrapeTimer = 0f;
            evidenceUnlockedForCurrentRegion = false;
        }

        if (evidenceUnlockedForCurrentRegion)
            return;

        scrapeTimer += Time.deltaTime;

        if (scrapeTimer >= material.requiredScrapeTime)
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

        int pixelX = Mathf.Clamp(Mathf.RoundToInt(u * (audioMask.width - 1)), 0, audioMask.width - 1);
        int pixelY = Mathf.Clamp(Mathf.RoundToInt(v * (audioMask.height - 1)), 0, audioMask.height - 1);

        Color pixelColor = audioMask.GetPixel(pixelX, pixelY);

        if (debugScan)
        {
            Debug.Log(
                "[FoodScan] Pos: " + worldPos +
                " | UV: " + new Vector2(u, v) +
                " | Pixel: " + pixelX + "," + pixelY +
                " | Color: " + pixelColor
            );
        }

        return pixelColor;
    }

    private void ResetScrape()
    {
        scrapeTimer = 0f;
        currentEvidenceID = "";
        evidenceUnlockedForCurrentRegion = false;
    }

    private void StopAudio()
    {
        if (!isPlayingMaterialSound)
            return;

        audioSource.Pause();
        isPlayingMaterialSound = false;
    }
}