using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    public static SFXManager instance; // legacy support for existing scripts

    [Header("Lifetime")]
    [SerializeField] private bool dontDestroyOnLoad = false;
    [SerializeField] private bool autoSubscribeToGameEvents = true;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Volume")]
    [Range(0f, 1f)][SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float dialogueVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float evidenceVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float scanVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float reportVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float uiVolume = 1f;

    [Header("Spam Protection")]
    [Tooltip("Prevents the same clip from firing multiple times in the same moment.")]
    [SerializeField] private float repeatedClipCooldown = 0.03f;

    [Header("Dialogue")]
    [SerializeField] private AudioClip dialogueOpen;
    [SerializeField] private AudioClip dialogueNext;
    [SerializeField] private AudioClip dialogueClose;

    [Header("Evidence")]
    [SerializeField] private AudioClip evidenceFound;
    [SerializeField] private AudioClip visibleEvidenceFound;
    [SerializeField] private AudioClip audibleEvidenceFound;
    [SerializeField] private AudioClip bothEvidenceFound;

    [Header("Scanner / Tools")]
    [SerializeField] private AudioClip scanStart;
    [SerializeField] private AudioClip scanStop;
    [SerializeField] private AudioClip invalidAction;

    [Header("Report")]
    [SerializeField] private AudioClip reportSelect;
    [SerializeField] private AudioClip reportRemove;
    [SerializeField] private AudioClip reportCorrect;
    [SerializeField] private AudioClip reportWrong;

    [Header("Journal / Panels")]
    [SerializeField] private AudioClip journalOpen;
    [SerializeField] private AudioClip journalClose;
    [SerializeField] private AudioClip panelOpen;
    [SerializeField] private AudioClip panelClose;

    [Header("UI")]
    [SerializeField] private AudioClip uiClick;
    [SerializeField] private AudioClip uiConfirm;
    [SerializeField] private AudioClip uiBack;

    private readonly Dictionary<AudioClip, float> lastPlayedTime =
        new Dictionary<AudioClip, float>();

    private bool subscribedToEvidence;
    private bool subscribedToReport;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        instance = this;

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (sfxSource != null)
        {
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }
    }

    private void Start()
    {
        TrySubscribeToGameEvents();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        TrySubscribeToGameEvents();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        UnsubscribeFromGameEvents();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            instance = null;
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!autoSubscribeToGameEvents)
            return;

        // If this manager persists across scenes, old scene objects were destroyed.
        subscribedToEvidence = false;
        subscribedToReport = false;

        StartCoroutine(SubscribeNextFrame());
    }

    private IEnumerator SubscribeNextFrame()
    {
        yield return null;
        TrySubscribeToGameEvents();
    }

    private void TrySubscribeToGameEvents()
    {
        if (!autoSubscribeToGameEvents)
            return;

        if (!subscribedToEvidence && EvidenceNotebook.Instance != null)
        {
            EvidenceNotebook.Instance.OnEvidenceAdded += HandleEvidenceAdded;
            subscribedToEvidence = true;
        }

        if (!subscribedToReport && DeductionManager.Instance != null)
        {
            DeductionManager.Instance.OnReportSubmitted += HandleReportSubmitted;
            subscribedToReport = true;
        }
    }

    private void UnsubscribeFromGameEvents()
    {
        if (subscribedToEvidence && EvidenceNotebook.Instance != null)
            EvidenceNotebook.Instance.OnEvidenceAdded -= HandleEvidenceAdded;

        if (subscribedToReport && DeductionManager.Instance != null)
            DeductionManager.Instance.OnReportSubmitted -= HandleReportSubmitted;

        subscribedToEvidence = false;
        subscribedToReport = false;
    }

    private void HandleEvidenceAdded(string evidenceID, EvidenceChannel discoveredChannel)
    {
        PlayEvidenceFound(discoveredChannel);
    }

    private void HandleReportSubmitted(bool correct)
    {
        PlayReportSubmitted(correct);
    }

    private void Play(AudioClip clip, float categoryVolume = 1f, float extraVolume = 1f)
    {
        if (clip == null || sfxSource == null)
            return;

        if (repeatedClipCooldown > 0f &&
            lastPlayedTime.TryGetValue(clip, out float lastTime) &&
            Time.unscaledTime - lastTime < repeatedClipCooldown)
        {
            return;
        }

        lastPlayedTime[clip] = Time.unscaledTime;

        float finalVolume = Mathf.Clamp01(masterVolume * categoryVolume * extraVolume);
        sfxSource.PlayOneShot(clip, finalVolume);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Dialogue
    // ─────────────────────────────────────────────────────────────────────────

    public void PlayDialogueOpen()
    {
        Play(dialogueOpen, dialogueVolume);
    }

    public void PlayDialogueNext()
    {
        Play(dialogueNext, dialogueVolume);
    }

    public void PlayDialogueClose()
    {
        Play(dialogueClose, dialogueVolume);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Evidence
    // ─────────────────────────────────────────────────────────────────────────

    public void PlayEvidenceFound()
    {
        Play(evidenceFound, evidenceVolume);
    }

    public void PlayEvidenceFound(EvidenceChannel discoveredChannel)
    {
        AudioClip selectedClip = evidenceFound;

        if (discoveredChannel == EvidenceChannel.Visible && visibleEvidenceFound != null)
            selectedClip = visibleEvidenceFound;
        else if (discoveredChannel == EvidenceChannel.Audible && audibleEvidenceFound != null)
            selectedClip = audibleEvidenceFound;
        else if (discoveredChannel == EvidenceChannel.Both && bothEvidenceFound != null)
            selectedClip = bothEvidenceFound;

        Play(selectedClip, evidenceVolume);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Scanner / Tools
    // ─────────────────────────────────────────────────────────────────────────

    public void PlayScanStart()
    {
        Play(scanStart, scanVolume);
    }

    public void PlayScanStop()
    {
        Play(scanStop, scanVolume);
    }

    public void PlayInvalidAction()
    {
        Play(invalidAction, uiVolume, 0.9f);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Report
    // ─────────────────────────────────────────────────────────────────────────

    public void PlayReportSelect()
    {
        Play(reportSelect, reportVolume);
    }

    public void PlayReportRemove()
    {
        Play(reportRemove, reportVolume);
    }

    public void PlayReportToggle(bool selected)
    {
        if (selected)
            PlayReportSelect();
        else
            PlayReportRemove();
    }

    public void PlayReportSubmitted(bool correct)
    {
        Play(correct ? reportCorrect : reportWrong, reportVolume);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Journal / Panels / UI
    // ─────────────────────────────────────────────────────────────────────────

    public void PlayJournalOpen()
    {
        Play(journalOpen != null ? journalOpen : panelOpen, uiVolume);
    }

    public void PlayJournalClose()
    {
        Play(journalClose != null ? journalClose : panelClose, uiVolume);
    }

    public void PlayJournalToggle(bool isOpen)
    {
        if (isOpen)
            PlayJournalOpen();
        else
            PlayJournalClose();
    }

    public void PlayPanelOpen()
    {
        Play(panelOpen, uiVolume);
    }

    public void PlayPanelClose()
    {
        Play(panelClose, uiVolume);
    }

    public void PlayPanelToggle(bool isOpen)
    {
        if (isOpen)
            PlayPanelOpen();
        else
            PlayPanelClose();
    }

    public void PlayUIClick()
    {
        Play(uiClick, uiVolume);
    }

    public void PlayUIConfirm()
    {
        Play(uiConfirm != null ? uiConfirm : uiClick, uiVolume);
    }

    public void PlayUIBack()
    {
        Play(uiBack != null ? uiBack : uiClick, uiVolume);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Legacy wrappers so older prototype scripts do not break.
    // ─────────────────────────────────────────────────────────────────────────

    public void PlayObjectSelect()
    {
        PlayUIClick();
    }

    public void PlayInvalidPlacement()
    {
        PlayInvalidAction();
    }

    public void PlayPlaceConfirm()
    {
        PlayUIConfirm();
    }

    public void PlaySpawnNext()
    {
        PlayUIConfirm();
    }

    public void PlayRotateStart()
    {
        PlayScanStart();
    }

    public void PlayRotateStop()
    {
        PlayScanStop();
    }
}
