using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Dialogue")]
    [SerializeField] private AudioClip dialogueOpen;
    [SerializeField] private AudioClip dialogueNext;
    [SerializeField] private AudioClip dialogueClose;

    [Header("Placement")]
    [SerializeField] private AudioClip objectSelect;
    [SerializeField] private AudioClip invalidPlacement;
    [SerializeField] private AudioClip placeConfirm;
    [SerializeField] private AudioClip spawnNext;

    [Header("Rotation")]
    [SerializeField] private AudioClip rotateStart;
    [SerializeField] private AudioClip rotateStop;

    [Header("UI")]
    [SerializeField] private AudioClip panelOpen;
    [SerializeField] private AudioClip panelClose;
    [SerializeField] private AudioClip uiClick;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
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

    public void PlayObjectSelect() => Play(objectSelect);
    public void PlayInvalidPlacement() => Play(invalidPlacement, 0.9f);
    public void PlayPlaceConfirm() => Play(placeConfirm);
    public void PlaySpawnNext() => Play(spawnNext);

    public void PlayRotateStart() => Play(rotateStart);
    public void PlayRotateStop() => Play(rotateStop);

    public void PlayPanelToggle(bool isOpen)
    {
        Play(isOpen ? panelOpen : panelClose);
    }

    public void PlayUIClick() => Play(uiClick);
}
