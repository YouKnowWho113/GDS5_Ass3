using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    public static SFXManager instance;

    [Header("Lifetime")]
    [SerializeField] private bool dontDestroyOnLoad = false;
    [SerializeField] private bool autoSubscribeToGameEvents = true;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource lightHoldSource;

    [Header("Volume")]
    [Range(0f, 1f)][SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float dialogueVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float evidenceVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float lightVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float reportVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float uiVolume = 1f;

    [Header("Spam Protection")]
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

    [Header("Light Tool")]
    [SerializeField] private AudioClip lightOpen;
    [SerializeField] private AudioClip lightClose;
    [SerializeField] private AudioClip lightHoldLoop;

    [Header("Report")]
    [SerializeField] private AudioClip reportSelect;
    [SerializeField] private AudioClip reportRemove;
    [SerializeField] private AudioClip reportCorrect;
    [SerializeField] private AudioClip reportWrong;

    [Header("Journal")]
    [SerializeField] private AudioClip journalOpen;
    [SerializeField] private AudioClip journalClose;

    [Header("UI")]
    [SerializeField] private AudioClip uiClick;
    [SerializeField] private AudioClip uiConfirm;
    [SerializeField] private AudioClip uiBack;
    [SerializeField] private AudioClip invalidAction;

    private readonly Dictionary<AudioClip, float> lastPlayedTime = new Dictionary<AudioClip, float>();

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

        SetupLightHoldSource();
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
        StopLightHold();
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        UnsubscribeFromGameEvents();
    }

    private void OnDestroy()
    {
        StopLightHold();

        if (Instance == this)
        {
            Instance = null;
            instance = null;
        }
    }

    private void SetupLightHoldSource()
    {
        if (lightHoldSource == null)
        {
            GameObject loopObj = new GameObject("LightHoldLoopSource");
            loopObj.transform.SetParent(transform);
            lightHoldSource = loopObj.AddComponent<AudioSource>();
        }

        lightHoldSource.playOnAwake = false;
        lightHoldSource.loop = true;
        lightHoldSource.clip = lightHoldLoop;
        lightHoldSource.volume = Mathf.Clamp01(masterVolume * lightVolume);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopLightHold();

        if (!autoSubscribeToGameEvents)
            return;

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

    public void PlayLightOpen()
    {
        Play(lightOpen, lightVolume);
    }

    public void PlayLightClose()
    {
        Play(lightClose, lightVolume);
    }

    public void StartLightHold()
    {
        if (lightHoldLoop == null)
            return;

        if (lightHoldSource == null)
            SetupLightHoldSource();

        if (lightHoldSource.isPlaying && lightHoldSource.clip == lightHoldLoop)
            return;

        lightHoldSource.clip = lightHoldLoop;
        lightHoldSource.volume = Mathf.Clamp01(masterVolume * lightVolume);
        lightHoldSource.loop = true;
        lightHoldSource.Play();
    }

    public void StopLightHold()
    {
        if (lightHoldSource != null && lightHoldSource.isPlaying)
            lightHoldSource.Stop();
    }

    public void SetLightHolding(bool holding)
    {
        if (holding)
            StartLightHold();
        else
            StopLightHold();
    }

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

    public void PlayJournalOpen()
    {
        Play(journalOpen, uiVolume);
    }

    public void PlayJournalClose()
    {
        Play(journalClose, uiVolume);
    }

    public void PlayJournalToggle(bool isOpen)
    {
        if (isOpen)
            PlayJournalOpen();
        else
            PlayJournalClose();
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

    public void PlayInvalidAction()
    {
        Play(invalidAction, uiVolume);
    }
}