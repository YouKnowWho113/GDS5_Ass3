using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// ─────────────────────────────────────────────────────────────────────────────
//  Data container for each scannable region in the mask.
// ─────────────────────────────────────────────────────────────────────────────
[System.Serializable]
public struct MaterialAudioData
{
    [Header("Identity")]
    public string materialName;
    public string evidenceID;

    [Header("Mask Color")]
    [Tooltip("The flat color used for this region in the mask texture. " +
             "Exact match is not required — see Color Tolerance on FoodScan.")]
    public Color maskColor;

    [Header("Audio")]
    public AudioClip scanSound;
    [Range(0f, 1f)] public float volume;

    [Header("Evidence")]
    [Tooltip("Hold the cursor over this region for this many seconds to unlock evidence.")]
    public float requiredScanTime;
}

// ─────────────────────────────────────────────────────────────────────────────
//  FoodScan
//  Detects which color region the cursor is over using a Read/Write-enabled
//  mask texture (never placed in the scene) and plays the matching sound.
//
//  HOW THE MASK WORKS
//  ──────────────────
//  1. Create a texture the same pixel size as your sprite.
//  2. Paint each scannable region a distinct flat color.
//  3. Enable Read/Write on the texture's Import Settings.
//  4. Assign it to Color Mask — do NOT place it in the scene.
//  5. Add one MaterialAudioData entry per color to the Material Database.
//
//  COLOR MATCHING
//  ──────────────
//  Uses squared Euclidean distance in RGBA so slight JPEG/atlas artefacts
//  or anti-aliased edges won't break detection.
//  Raise Color Tolerance if misses occur (0.10–0.25 is typical for lossy masks).
//  Keep it low to avoid false matches between adjacent colors.
// ─────────────────────────────────────────────────────────────────────────────
[RequireComponent(typeof(AudioSource))]
public class FoodScan : MonoBehaviour
{
    // ── inspector ─────────────────────────────────────────────────────────────

    [Header("References")]
    [Tooltip("The SpriteRenderer whose bounds define the scan area.")]
    public SpriteRenderer foodRenderer;

    [Tooltip("Read/Write-enabled mask texture. Each region is a flat color matching " +
             "an entry in the Material Database. Must be the same pixel size as the sprite.")]
    public Texture2D colorMask;

    [Header("Input")]
    [Tooltip("0 = left mouse button, 1 = right, 2 = middle.")]
    public int mouseButton = 0;
    [Tooltip("Ignore input when the cursor is over a UI element.")]
    public bool ignoreUI = true;

    [Header("Audio")]
    public AudioSource audioSource;
    [Tooltip("Loop the clip while hovering. When false the clip fires as a one-shot " +
             "using Play Interval as the cooldown — good for dragging.")]
    public bool loopWhileHovering = false;
    [Tooltip("Master volume multiplier on top of each entry's own volume.")]
    public float globalVolume = 1f;
    [Tooltip("Minimum seconds between one-shot plays. " +
             "Prevents audio spam when dragging quickly across a region.")]
    [Min(0f)] public float playInterval = 0.35f;

    [Header("Color Matching")]
    [Range(0f, 1f)]
    [Tooltip("Maximum Euclidean distance in RGBA space for a match. " +
             "0.10–0.20 handles most JPEG / texture-atlas artefacts.")]
    public float colorTolerance = 0.15f;

    [Header("Feel & Transitions")]
    [Tooltip("Seconds the active region stays alive after the cursor leaves it. " +
             "Prevents choppy sound when dragging quickly across a boundary.")]
    [Min(0f)] public float gracePeriod = 0.12f;
    [Tooltip("Seconds the cursor must hover over a new region before switching. " +
             "Stops flickering when the cursor grazes a color boundary.")]
    [Min(0f)] public float regionSwitchDelay = 0.08f;

    [Header("Material Database")]
    public List<MaterialAudioData> materialDatabase = new List<MaterialAudioData>();

    [Header("Debug")]
    [Tooltip("Logs world position, local UV, and sampled color whenever they change.")]
    public bool debugMode = false;

    // ── texture cache ─────────────────────────────────────────────────────────

    private Sprite cachedSprite;
    private Texture2D sampleTexture;
    private Color32[] pixelCache;      // CPU pixel array; fast O(1) lookup
    private int texW, texH;
    private bool needAtlasRemap;  // true when sprite lives inside a texture atlas
    private Rect atlasNorm;       // sprite sub-rect in atlas, normalised to [0,1]

    // ── material cache ────────────────────────────────────────────────────────
    // Pre-baked to plain floats so TryMatch is fully allocation-free every frame.

    private struct MatEntry
    {
        public MaterialAudioData data;
        public float r, g, b, a;
        public float tolSq;
    }
    private MatEntry[] matCache = new MatEntry[0];

    // ── scan / region state ───────────────────────────────────────────────────

    private MaterialAudioData activeRegion;
    private bool hasActive;
    private float graceTimer;
    private float switchTimer;

    // ── audio state ───────────────────────────────────────────────────────────

    private bool loopIsPlaying;
    private float nextOneShotTime;

    // ── evidence state ────────────────────────────────────────────────────────

    private string trackedEvidenceID;
    private float evidenceTimer;
    private bool evidenceGranted;

    // ── misc ──────────────────────────────────────────────────────────────────

    private Camera cam;
    private Color lastDebugColor;   // only used when debugMode = true

    // ═════════════════════════════════════════════════════════════════════════
    //  Unity messages
    // ═════════════════════════════════════════════════════════════════════════

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.pitch = 1f;

        cam = Camera.main;
        BuildCache();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (audioSource != null) audioSource.loop = loopWhileHovering;
        BuildCache();
    }
#endif

    private void Update()
    {
        if (foodRenderer == null) return;
        if (cam == null) cam = Camera.main;

        // Re-cache if the displayed sprite was swapped at runtime.
        if (cachedSprite != foodRenderer.sprite)
            BuildCache();

        if (sampleTexture == null || cam == null) return;

        // ── input gates ───────────────────────────────────────────────────────

        if (!Input.GetMouseButton(mouseButton))
        {
            Deactivate();
            return;
        }

        if (ignoreUI &&
            EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            Deactivate();
            return;
        }

        // ── cursor world position ─────────────────────────────────────────────

        Vector3 screen = Input.mousePosition;
        screen.z = Mathf.Abs(cam.transform.position.z - foodRenderer.transform.position.z);
        Vector2 worldPos = cam.ScreenToWorldPoint(screen);

        // ── fast AABB reject ──────────────────────────────────────────────────

        if (!foodRenderer.bounds.Contains(
                new Vector3(worldPos.x, worldPos.y, foodRenderer.transform.position.z)))
        {
            HandleGrace();
            return;
        }

        // ── sample mask and match ─────────────────────────────────────────────

        Color sampled = SampleMaskAt(worldPos);

        if (TryMatch(sampled, out MaterialAudioData hit))
            HandleHit(hit);
        else
            HandleGrace();
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  Region hit / grace logic
    // ═════════════════════════════════════════════════════════════════════════

    private void HandleHit(MaterialAudioData hit)
    {
        graceTimer = 0f;

        if (!hasActive)
        {
            SetActive(hit);
            return;
        }

        if (hit.evidenceID == activeRegion.evidenceID)
        {
            // Same region — keep running.
            switchTimer = 0f;
            TickEvidence();
            TickAudio();
        }
        else
        {
            // Different region — wait before committing.
            // Prevents flickering when the cursor grazes a color boundary.
            switchTimer += Time.deltaTime;
            if (switchTimer >= regionSwitchDelay)
                SetActive(hit);
            else
            {
                TickEvidence();
                TickAudio();
            }
        }
    }

    private void HandleGrace()
    {
        // Keep the current region alive for gracePeriod after the cursor leaves,
        // so audio doesn't cut out when dragging quickly across a boundary.
        graceTimer += Time.deltaTime;

        if (graceTimer >= gracePeriod)
            Deactivate();
        else if (hasActive)
        {
            TickEvidence();
            TickAudio();
        }
    }

    private void SetActive(MaterialAudioData region)
    {
        bool isNew = (region.evidenceID != trackedEvidenceID);

        activeRegion = region;
        hasActive = true;
        switchTimer = 0f;
        graceTimer = 0f;

        // Only reset the evidence timer when entering a genuinely new region;
        // re-entering the same region continues from where it left off.
        if (isNew)
        {
            trackedEvidenceID = region.evidenceID;
            evidenceTimer = 0f;
            evidenceGranted = false;
        }

        // Audio
        if (region.scanSound != null)
        {
            audioSource.clip = region.scanSound;
            audioSource.volume = Mathf.Clamp01(region.volume * globalVolume);
            audioSource.loop = loopWhileHovering;

            if (loopWhileHovering)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                    loopIsPlaying = true;
                }
            }
            else
            {
                TryPlayOneShot();
            }
        }
        else
        {
            StopAudio();
        }

        TickEvidence();
    }

    private void Deactivate()
    {
        hasActive = false;
        switchTimer = 0f;
        graceTimer = 0f;
        StopAudio();
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  Audio helpers
    // ═════════════════════════════════════════════════════════════════════════

    private void TickAudio()
    {
        if (activeRegion.scanSound == null) return;

        if (loopWhileHovering)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                loopIsPlaying = true;
            }
        }
        else
        {
            TryPlayOneShot();
        }
    }

    // Fires a one-shot only when the cooldown has elapsed.
    // This is the primary mechanism that prevents spam while dragging.
    private void TryPlayOneShot()
    {
        if (Time.time >= nextOneShotTime)
        {
            audioSource.Play();
            nextOneShotTime = Time.time + playInterval;
        }
    }

    private void StopAudio()
    {
        if (loopWhileHovering && loopIsPlaying)
            audioSource.Stop();
        loopIsPlaying = false;
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  Evidence
    // ═════════════════════════════════════════════════════════════════════════

    private void TickEvidence()
    {
        if (evidenceGranted) return;

        evidenceTimer += Time.deltaTime;

        if (evidenceTimer >= activeRegion.requiredScanTime)
        {
            evidenceGranted = true;

            if (EvidenceNotebook.Instance != null)
                EvidenceNotebook.Instance.AddAudibleEvidence(activeRegion.evidenceID);
            else
                Debug.LogWarning("[FoodScan] EvidenceNotebook instance not found in scene.");
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  Color matching  (hot path — zero allocations)
    // ═════════════════════════════════════════════════════════════════════════

    private bool TryMatch(Color sample, out MaterialAudioData result)
    {
        float sr = sample.r, sg = sample.g, sb = sample.b, sa = sample.a;

        for (int i = 0; i < matCache.Length; i++)
        {
            ref MatEntry e = ref matCache[i];
            float dr = sr - e.r;
            float dg = sg - e.g;
            float db = sb - e.b;
            float da = sa - e.a;

            if (dr * dr + dg * dg + db * db + da * da <= e.tolSq)
            {
                result = e.data;
                return true;
            }
        }

        result = default;
        return false;
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  Mask sampling
    // ═════════════════════════════════════════════════════════════════════════

    private Color SampleMaskAt(Vector2 worldPos)
    {
        // Step 1: world → local sprite space.
        //   InverseTransformPoint handles position, rotation, and scale.
        Vector2 local = foodRenderer.transform.InverseTransformPoint(worldPos);

        // Step 2: normalise within sprite.bounds (local space) → UV [0, 1].
        //   This works for any pivot, any PPU, and any scale.
        Bounds lb = cachedSprite.bounds;
        float u = Mathf.InverseLerp(lb.min.x, lb.max.x, local.x);
        float v = Mathf.InverseLerp(lb.min.y, lb.max.y, local.y);

        // Step 3: respect SpriteRenderer flips.
        if (foodRenderer.flipX) u = 1f - u;
        if (foodRenderer.flipY) v = 1f - v;

        // Step 4: if sampling from an atlas, remap into the sprite's sub-rect.
        //   External colorMask is always standalone, so this is skipped for it.
        if (needAtlasRemap)
        {
            u = Mathf.Lerp(atlasNorm.xMin, atlasNorm.xMax, u);
            v = Mathf.Lerp(atlasNorm.yMin, atlasNorm.yMax, v);
        }

        if (debugMode)
        {
            Color dbg = SampleAtUV(u, v);
            if (Vector4.Distance(dbg, lastDebugColor) > 0.05f)
            {
                Debug.Log($"[FoodScan] World:{worldPos}  Local:{local}  " +
                          $"UV:({u:F3}, {v:F3})  Sampled:{dbg}");
                lastDebugColor = dbg;
            }
        }

        return SampleAtUV(u, v);
    }

    // Direct UV sampler. Uses the cached pixel array (fast O(1) array read)
    // when available and falls back to GetPixelBilinear otherwise.
    private Color SampleAtUV(float u, float v)
    {
        if (pixelCache != null)
        {
            int x = Mathf.Clamp(Mathf.RoundToInt(u * (texW - 1)), 0, texW - 1);
            int y = Mathf.Clamp(Mathf.RoundToInt(v * (texH - 1)), 0, texH - 1);
            int idx = y * texW + x;

            if ((uint)idx < (uint)pixelCache.Length)
            {
                Color32 p = pixelCache[idx];
                return new Color(p.r / 255f, p.g / 255f, p.b / 255f, p.a / 255f);
            }
        }

        // Fallback: works without Read/Write but is ~5× slower.
        if (sampleTexture != null)
            return sampleTexture.GetPixelBilinear(u, v);

        return Color.clear;
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  Cache building
    //  Called on Awake and whenever the sprite is swapped at runtime.
    // ═════════════════════════════════════════════════════════════════════════

    private void BuildCache()
    {
        cachedSprite = foodRenderer != null ? foodRenderer.sprite : null;

        // ── choose sampling texture ───────────────────────────────────────────

        if (colorMask != null)
        {
            // External mask — sample the whole texture, no atlas remapping.
            sampleTexture = colorMask;
            needAtlasRemap = false;
            atlasNorm = new Rect(0f, 0f, 1f, 1f);
        }
        else if (cachedSprite != null)
        {
            sampleTexture = cachedSprite.texture;

            Rect tr = cachedSprite.textureRect;
            float tw = sampleTexture.width;
            float th = sampleTexture.height;
            atlasNorm = new Rect(tr.x / tw, tr.y / th, tr.width / tw, tr.height / th);

            // Remap only when the sprite doesn't cover the entire texture.
            needAtlasRemap = atlasNorm.width < 0.999f || atlasNorm.height < 0.999f;
        }
        else
        {
            sampleTexture = null;
            needAtlasRemap = false;
        }

        // ── cache pixel array ─────────────────────────────────────────────────

        pixelCache = null;
        texW = texH = 0;

        if (sampleTexture != null)
        {
            texW = sampleTexture.width;
            texH = sampleTexture.height;

            try
            {
                Color32[] p = sampleTexture.GetPixels32();
                if (p != null && p.Length == texW * texH)
                    pixelCache = p;
            }
            catch
            {
                Debug.LogWarning("[FoodScan] Mask texture is not Read/Write-enabled. " +
                                 "Enable it in the texture's Import Settings for best performance.");
            }
        }

        // ── pre-bake material entries ─────────────────────────────────────────

        float tSq = colorTolerance * colorTolerance;

        matCache = new MatEntry[materialDatabase.Count];
        for (int i = 0; i < materialDatabase.Count; i++)
        {
            MaterialAudioData d = materialDatabase[i];
            matCache[i] = new MatEntry
            {
                data = d,
                r = d.maskColor.r,
                g = d.maskColor.g,
                b = d.maskColor.b,
                a = d.maskColor.a,
                tolSq = tSq
            };
        }
    }

    // ── public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Call this if you add or remove entries in materialDatabase at runtime.
    /// </summary>
    public void RefreshDatabase() => BuildCache();
}