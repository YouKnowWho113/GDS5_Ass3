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

    [Header("Material Database")]
    public List<MaterialAudioData> materialDatabase; 

    private Vector2 lastMousePos;
    private MaterialAudioData currentMaterial;
    private bool isPlayingMaterialSound = false;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }

    void Update()
    {
        Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            if (foodRenderer.sprite.bounds.Contains(currentMousePos))
            {
                ProcessAudioForensics(currentMousePos);
            }
            else
            {
                StopAudio();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopAudio();
        }

        lastMousePos = currentMousePos;
    }

    private void ProcessAudioForensics(Vector2 mousePos)
    {
        float mouseSpeed = Vector2.Distance(mousePos, lastMousePos) / Time.deltaTime;

        if (mouseSpeed < 0.1f)
        {
            StopAudio();
            return;
        }

        Color hitColor = GetColorFromMask(mousePos);

        bool foundMaterial = false;
        foreach (var mat in materialDatabase)
        {
            if (Vector4.Distance(hitColor, mat.maskColor) < colorTolerance)
            {
                currentMaterial = mat;
                foundMaterial = true;
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
            }
            else if (!isPlayingMaterialSound)
            {
                audioSource.Play();
                isPlayingMaterialSound = true;
            }

            float speedRatio = Mathf.Clamp01(mouseSpeed / maxSpeedThreshold);

            audioSource.volume = Mathf.Lerp(0.2f, 1.0f, speedRatio);
            audioSource.pitch = Mathf.Lerp(currentMaterial.basePitch - 0.2f, currentMaterial.basePitch + 0.3f, speedRatio);
        }
        else
        {
            StopAudio();
        }
    }

    private Color GetColorFromMask(Vector2 worldPos)
    {
        Vector2 localPos = foodRenderer.transform.InverseTransformPoint(worldPos);

        Vector2 boundsMin = foodRenderer.sprite.bounds.min - foodRenderer.transform.position;
        Vector2 boundsMax = foodRenderer.sprite.bounds.max - foodRenderer.transform.position;
        Vector2 boundsSize = boundsMax - boundsMin;

        float u = (localPos.x - boundsMin.x) / boundsSize.x;
        float v = (localPos.y - boundsMin.y) / boundsSize.y;

        int pixelX = Mathf.RoundToInt(u * audioMask.width);
        int pixelY = Mathf.RoundToInt(v * audioMask.height);

        return audioMask.GetPixel(pixelX, pixelY);
    }

    private void StopAudio()
    {
        if (isPlayingMaterialSound)
        {
            audioSource.Pause();
            isPlayingMaterialSound = false;
        }
    }
}