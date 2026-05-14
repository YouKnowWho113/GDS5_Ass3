using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Canvas")]
    public GameObject dialogueCanvasRoot;
    public bool disableCanvasWhenDialogueEnds = true;

    [Header("Scene Start Dialogue")]
    public bool playOnSceneStart = false;
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
    private Coroutine canvasCloseRoutine;

    private void Start()
    {
        if (dialogueCanvasRoot != null)
            dialogueCanvasRoot.SetActive(false);

        if (playOnSceneStart && sceneStartDialogue != null)
            StartCoroutine(PlaySceneStartDialogue());
    }

    private void OnEnable()
    {
        if (DeductionManager.Instance != null)
            DeductionManager.Instance.OnReportSubmitted += HandleReportSubmitted;
    }

    private void OnDisable()
    {
        if (DeductionManager.Instance != null)
            DeductionManager.Instance.OnReportSubmitted -= HandleReportSubmitted;
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

        StartCoroutine(PlayDialogueRoutine(dialogue));
    }

    private IEnumerator PlayDialogueRoutine(DialogueBase dialogue)
    {
        if (waitUntilCurrentDialogueEnds)
            yield return StartCoroutine(WaitUntilDialogueFree());

        if (dialogueCanvasRoot != null && !dialogueCanvasRoot.activeSelf)
        {
            dialogueCanvasRoot.SetActive(true);

            // Wait one frame so DialogueManager inside DiaCanvas wakes up.
            yield return null;
        }

        if (DialogueManager.instance == null)
        {
            Debug.LogWarning("[DialogueTrigger] DialogueManager missing. Check DiaCanvas and DialogueManager.");
            yield break;
        }

        DialogueManager.instance.EnqueueDialogue(dialogue);

        if (disableCanvasWhenDialogueEnds)
        {
            if (canvasCloseRoutine != null)
                StopCoroutine(canvasCloseRoutine);

            canvasCloseRoutine = StartCoroutine(DisableCanvasWhenDialogueEnds());
        }
    }

    private IEnumerator WaitUntilDialogueFree()
    {
        while (DialogueManager.instance != null &&
               DialogueManager.instance.IsDialogueActive())
        {
            yield return null;
        }
    }

    private IEnumerator DisableCanvasWhenDialogueEnds()
    {
        while (DialogueManager.instance != null &&
               DialogueManager.instance.IsDialogueActive())
        {
            yield return null;
        }

        // Wait one extra frame so DialogueManager can finish closing its dialogueBox.
        yield return null;

        if (dialogueCanvasRoot != null)
            dialogueCanvasRoot.SetActive(false);

        canvasCloseRoutine = null;
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