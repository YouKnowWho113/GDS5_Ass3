using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    public static SFXManager instance; // legacy support for old scripts

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Dialogue")]
    [SerializeField] private AudioClip dialogueOpen;
    [SerializeField] private AudioClip dialogueNext;
    [SerializeField] private AudioClip dialogueClose;

    [Header("Scanner / Evidence")]
    [SerializeField] private AudioClip evidenceFound;
    [SerializeField] private AudioClip scanStart;
    [SerializeField] private AudioClip scanStop;

    [Header("Report / Journal")]
    [SerializeField] private AudioClip reportSelect;
    [SerializeField] private AudioClip reportRemove;
    [SerializeField] private AudioClip reportCorrect;
    [SerializeField] private AudioClip reportWrong;
    [SerializeField] private AudioClip journalOpen;
    [SerializeField] private AudioClip journalClose;

    [Header("Dish / Spawning")]
    [SerializeField] private AudioClip spawnNext;

    [Header("UI")]
    [SerializeField] private AudioClip panelOpen;
    [SerializeField] private AudioClip panelClose;
    [SerializeField] private AudioClip uiClick;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        instance = this;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (sfxSource != null)
            sfxSource.playOnAwake = false;
    }

    private void Play(AudioClip clip, float volume = 1f)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayDialogueOpen() => Play(dialogueOpen);
    public void PlayDialogueNext() => Play(dialogueNext);
    public void PlayDialogueClose() => Play(dialogueClose);

    public void PlayEvidenceFound() => Play(evidenceFound);
    public void PlayScanStart() => Play(scanStart);
    public void PlayScanStop() => Play(scanStop);

    public void PlayReportSelect() => Play(reportSelect);
    public void PlayReportRemove() => Play(reportRemove);

    public void PlayReportSubmitted(bool correct)
    {
        Play(correct ? reportCorrect : reportWrong);
    }

    public void PlayJournalOpen() => Play(journalOpen);
    public void PlayJournalClose() => Play(journalClose);

    public void PlaySpawnNext() => Play(spawnNext);

    public void PlayPanelOpen() => Play(panelOpen);
    public void PlayPanelClose() => Play(panelClose);

    public void PlayPanelToggle(bool isOpen)
    {
        Play(isOpen ? panelOpen : panelClose);
    }

    public void PlayUIClick() => Play(uiClick);
}