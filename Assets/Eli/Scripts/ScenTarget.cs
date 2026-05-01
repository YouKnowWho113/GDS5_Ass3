using UnityEngine;

public class ScenTarget : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer scannedRenderer;

    [Header("Evidence")]
    public string evidenceID;
    public float requiredRevealTime = 0.6f;
    public float revealThreshold = 0.6f;

    [Header("Fade")]
    public bool fadeOutWhenNotScanned = true;
    public float fadeOutSpeed = 3f;

    private float revealTimer;
    private float currentReveal;
    private bool evidenceUnlocked;
    private bool wasScannedThisFrame;

    private void Awake()
    {
        if (scannedRenderer == null)
            scannedRenderer = GetComponent<SpriteRenderer>();

        SetAlpha(0f);
    }

    private void LateUpdate()
    {
        if (!wasScannedThisFrame && fadeOutWhenNotScanned)
        {
            currentReveal = Mathf.MoveTowards(
                currentReveal,
                0f,
                fadeOutSpeed * Time.deltaTime
            );

            SetAlpha(currentReveal);

            if (!evidenceUnlocked)
                revealTimer = 0f;
        }

        wasScannedThisFrame = false;
    }

    public void SetReveal(float value)
    {
        wasScannedThisFrame = true;

        currentReveal = Mathf.Clamp01(value);
        SetAlpha(currentReveal);

        if (evidenceUnlocked)
            return;

        if (currentReveal >= revealThreshold)
        {
            revealTimer += Time.deltaTime;

            if (revealTimer >= requiredRevealTime)
            {
                evidenceUnlocked = true;

                if (EvidenceNotebook.Instance != null)
                {
                    EvidenceNotebook.Instance.AddVisibleEvidence(evidenceID);
                }
                else
                {
                    Debug.LogWarning("[ScenTarget] EvidenceNotebook missing in scene.");
                }
            }
        }
        else
        {
            revealTimer = 0f;
        }
    }

    private void SetAlpha(float alpha)
    {
        if (scannedRenderer == null)
            return;

        Color c = scannedRenderer.color;
        c.a = alpha;
        scannedRenderer.color = c;
    }
}