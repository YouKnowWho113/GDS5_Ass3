using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UVFoodScanner : MonoBehaviour
{
    [Header("Scanner Settings")]
    public float radius = 2.5f;
    public LayerMask foodLayer;

    [Header("Input")]
    public bool requireInput = true;
    public int mouseButton = 1; // 0 = left click, 1 = right click

    [Header("Evidence Unlock")]
    public float revealStrength = 1.5f;

    [Header("Scanner Light Visual")]
    public Light2D scannerLight2D;

    [Header("Light Fade")]
    public float maxLightIntensity = 1f;
    public float fadeInSpeed = 8f;
    public float fadeOutSpeed = 6f;

    [Header("Soft Shader Reveal")]
    public float visualRadius = 2.5f;
    public float softEdgeSize = 0.8f;

    private float currentLightIntensity;
    private readonly Collider2D[] hits = new Collider2D[32];

    private void Awake()
    {
        currentLightIntensity = 0f;

        if (scannerLight2D != null)
        {
            scannerLight2D.intensity = 0f;
            scannerLight2D.enabled = false;
        }

        UpdateShaderReveal(0f);
    }

    private void Update()
    {
        if (GameplayInputLock.IsLocked)
        {
            UpdateScannerLight(false);
            return;
        }

        bool scanning = !requireInput || Input.GetMouseButton(mouseButton);

        UpdateScannerLight(scanning);

        if (!scanning)
            return;

        ScanForVisibleEvidence();
    }

    private void UpdateScannerLight(bool scanning)
    {
        float targetIntensity = scanning ? maxLightIntensity : 0f;
        float fadeSpeed = scanning ? fadeInSpeed : fadeOutSpeed;

        currentLightIntensity = Mathf.MoveTowards(
            currentLightIntensity,
            targetIntensity,
            fadeSpeed * Time.deltaTime
        );

        bool shouldShowLight = currentLightIntensity > 0.01f;

        if (scannerLight2D != null)
        {
            scannerLight2D.enabled = shouldShowLight;
            scannerLight2D.intensity = currentLightIntensity;
        }

        float revealAmount = 0f;

        if (maxLightIntensity > 0f)
            revealAmount = currentLightIntensity / maxLightIntensity;

        UpdateShaderReveal(revealAmount);
    }

    private void UpdateShaderReveal(float revealAmount)
    {
        Shader.SetGlobalVector("_ScannerWorldPos", transform.position);
        Shader.SetGlobalFloat("_ScannerRadius", visualRadius);
        Shader.SetGlobalFloat("_ScannerSoftness", softEdgeSize);
        Shader.SetGlobalFloat("_ScannerEnabled", Mathf.Clamp01(revealAmount));
    }

    private void ScanForVisibleEvidence()
    {
        int hitCount = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            radius,
            hits,
            foodLayer
        );

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = hits[i];

            if (hit == null)
                continue;

            ScenTarget target = hit.GetComponent<ScenTarget>();

            if (target == null)
                continue;

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            float reveal = revealStrength - distance / radius;
            reveal = Mathf.Clamp01(reveal);

            target.SetReveal(reveal);
        }
    }

    private void OnDisable()
    {
        UpdateShaderReveal(0f);

        if (scannerLight2D != null)
        {
            scannerLight2D.intensity = 0f;
            scannerLight2D.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visualRadius);
    }
}