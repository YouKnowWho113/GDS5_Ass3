using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Scene Start Dialogue")]
    public bool playOnSceneStart = true;
    public DialogueBase sceneStartDialogue;
    public float sceneStartDelay = 0.3f;

    [Header("Report Submit Dialogue")]
    public bool playAfterReportSubmit = true;
    public DialogueBase correctReportDialogue;
    public DialogueBase incorrectReportDialogue;
    public float submitDialogueDelay = 0.2f;

    [Header("Behaviour")]
    public bool waitUntilCurrentDialogueEnds = true;
    public bool playSceneStartOnlyOnce = true;

    private bool sceneStartPlayed;

    private void Start()
    {
        if (playOnSceneStart && sceneStartDialogue != null)
        {
            StartCoroutine(PlaySceneStartDialogue());
        }
    }

    private void OnEnable()
    {
        if (DeductionManager.Instance != null)
        {
            DeductionManager.Instance.OnReportSubmitted += HandleReportSubmitted;
        }
    }

    private void OnDisable()
    {
        if (DeductionManager.Instance != null)
        {
            DeductionManager.Instance.OnReportSubmitted -= HandleReportSubmitted;
        }
    }

    private IEnumerator PlaySceneStartDialogue()
    {
        if (playSceneStartOnlyOnce && sceneStartPlayed)
            yield break;

        sceneStartPlayed = true;

        yield return new WaitForSeconds(sceneStartDelay);

        PlayDialogue(sceneStartDialogue);
    }

    private void HandleReportSubmitted(bool reportCorrect)
    {
        if (!playAfterReportSubmit)
            return;

        DialogueBase dialogueToPlay = reportCorrect
            ? correctReportDialogue
            : incorrectReportDialogue;

        if (dialogueToPlay == null)
            return;

        StartCoroutine(PlaySubmitDialogue(dialogueToPlay));
    }

    private IEnumerator PlaySubmitDialogue(DialogueBase dialogue)
    {
        yield return new WaitForSeconds(submitDialogueDelay);

        PlayDialogue(dialogue);
    }

    public void PlayDialogue(DialogueBase dialogue)
    {
        if (dialogue == null)
            return;

        if (DialogueManager.instance == null)
        {
            Debug.LogWarning("[DialogueTrigger] DialogueManager missing in scene.");
            return;
        }

        if (waitUntilCurrentDialogueEnds)
        {
            StartCoroutine(PlayWhenDialogueFree(dialogue));
        }
        else
        {
            DialogueManager.instance.EnqueueDialogue(dialogue);
        }
    }

    private IEnumerator PlayWhenDialogueFree(DialogueBase dialogue)
    {
        while (DialogueManager.instance != null &&
               DialogueManager.instance.dialogueBox != null &&
               DialogueManager.instance.dialogueBox.activeSelf)
        {
            yield return null;
        }

        DialogueManager.instance.EnqueueDialogue(dialogue);
    }

    public void PlaySceneStartManually()
    {
        PlayDialogue(sceneStartDialogue);
    }

    public void PlayCorrectReportDialogue()
    {
        PlayDialogue(correctReportDialogue);
    }

    public void PlayIncorrectReportDialogue()
    {
        PlayDialogue(incorrectReportDialogue);
    }
}