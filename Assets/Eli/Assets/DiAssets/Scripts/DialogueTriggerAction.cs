using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTriggerAction : MonoBehaviour
{
    [Header("Food Dialogue Asset")]
    public FoodDialogue foodDialogue;
    public bool useFoodDialogueAsset = true;

    public enum PostDialogueAction
    {
        None,
        LoadNextScene,
        ReloadCurrentScene,
        LoadSceneByName
    }

    [System.Serializable]
    public class DialogueActionEntry
    {
        [Header("Action Key")]
        public string key;

        [Header("Dialogue")]
        public DialogueBase dialogue;

        [Header("Rules")]
        public bool playOnlyOnce = true;

        [Header("Gameplay Control")]
        public bool lockGameplayDuringDialogue = true;
        public string gameplayLockReason = "Dialogue";

        [Header("After Dialogue")]
        public PostDialogueAction postDialogueAction = PostDialogueAction.None;
        public string sceneNameToLoad;
        public float postDialogueDelay = 0f;

        [HideInInspector] public bool hasPlayed;
    }

    [System.Serializable]
    public class EvidenceDialogueRule
    {
        [Header("Trigger")]
        public string evidenceID;
        public EvidenceChannel requiredChannel = EvidenceChannel.Visible;

        [Header("Dialogue Key To Play")]
        public string dialogueKey;

        [Header("Rules")]
        public bool playOnlyOnce = true;

        [HideInInspector] public bool hasPlayed;
    }

    [System.Serializable]
    public class RequiredEvidence
    {
        public string evidenceID;
        public EvidenceChannel requiredChannel = EvidenceChannel.Visible;
    }

    [System.Serializable]
    public class AllEvidenceDialogueRule
    {
        [Header("Required Evidence")]
        public RequiredEvidence[] requiredEvidence;

        [Header("Dialogue Key To Play")]
        public string dialogueKey = "before_report";

        [Header("Rules")]
        public bool playOnlyOnce = true;

        [HideInInspector] public bool hasPlayed;
    }

    [Header("Dialogue Canvas")]
    public GameObject dialogueCanvasRoot;
    public bool disableCanvasWhenQueueEnds = true;

    [Header("Action Dialogues")]
    public DialogueActionEntry[] actionDialogues;

    [Header("Single Evidence Tutorial Rules")]
    public EvidenceDialogueRule[] evidenceDialogueRules;

    [Header("All Evidence Tutorial Rules")]
    public AllEvidenceDialogueRule[] allEvidenceDialogueRules;

    [Header("Report Submit Keys")]
    public bool playAfterReportSubmit = true;
    public string correctReportKey = "after_correct_submit";
    public string incorrectReportKey = "after_wrong_submit";
    public float submitDialogueDelay = 0.2f;

    [Header("Wait Conditions")]
    public bool waitUntilCurrentDialogueEnds = true;

    [Tooltip("When true, before_report waits until any FoodScan AudioSource stops playing. No manual AudioSource assignment needed.")]
    public bool waitForFoodScanAudioBeforeAllEvidenceDialogue = true;

    [Tooltip("Safety timeout so before_report cannot get stuck forever if a looping scan sound never stops. Set 0 or below to wait forever.")]
    public float foodScanAudioWaitTimeout = 6f;

    [Header("Debug")]
    public bool logEvidenceChecks = true;

    private Dictionary<string, DialogueActionEntry> dialogueMap;
    private readonly Queue<DialogueActionEntry> dialogueQueue = new Queue<DialogueActionEntry>();

    private Coroutine queueRoutine;
    private Coroutine subscribeRoutine;

    private bool subscribedToEvidence;
    private bool subscribedToReport;
    private bool pendingAllEvidenceCheck;

    private void Awake()
    {
        ApplyFoodDialogue();
        ResetPlayedFlags();
        BuildMap();
    }

    private void Start()
    {
        if (dialogueCanvasRoot != null)
            dialogueCanvasRoot.SetActive(false);
    }

    private void OnEnable()
    {
        StartSubscriptionRoutine();
    }

    private void OnDisable()
    {
        if (subscribeRoutine != null)
        {
            StopCoroutine(subscribeRoutine);
            subscribeRoutine = null;
        }

        Unsubscribe();
    }

    private void ApplyFoodDialogue()
    {
        if (!useFoodDialogueAsset || foodDialogue == null)
            return;

        actionDialogues = foodDialogue.actionDialogues;
        evidenceDialogueRules = foodDialogue.evidenceDialogueRules;
        allEvidenceDialogueRules = foodDialogue.allEvidenceDialogueRules;

        playAfterReportSubmit = foodDialogue.playAfterReportSubmit;
        correctReportKey = foodDialogue.correctReportKey;
        incorrectReportKey = foodDialogue.incorrectReportKey;
        submitDialogueDelay = foodDialogue.submitDialogueDelay;

        waitUntilCurrentDialogueEnds = foodDialogue.waitUntilCurrentDialogueEnds;
        waitForFoodScanAudioBeforeAllEvidenceDialogue =
            foodDialogue.waitForFoodScanAudioBeforeAllEvidenceDialogue;
        foodScanAudioWaitTimeout = foodDialogue.foodScanAudioWaitTimeout;
    }

    private void BuildMap()
    {
        dialogueMap = new Dictionary<string, DialogueActionEntry>();

        if (actionDialogues == null)
            return;

        foreach (DialogueActionEntry entry in actionDialogues)
        {
            if (entry == null)
                continue;

            if (string.IsNullOrWhiteSpace(entry.key))
                continue;

            if (dialogueMap.ContainsKey(entry.key))
            {
                Debug.LogWarning("[DialogueTriggerAction] Duplicate dialogue key: " + entry.key);
                continue;
            }

            dialogueMap.Add(entry.key, entry);
        }
    }

    private void StartSubscriptionRoutine()
    {
        if (subscribeRoutine == null)
            subscribeRoutine = StartCoroutine(SubscribeWhenManagersAreReady());
    }

    private IEnumerator SubscribeWhenManagersAreReady()
    {
        while (!subscribedToEvidence || !subscribedToReport)
        {
            TrySubscribe();

            if (subscribedToEvidence && subscribedToReport)
                break;

            yield return null;
        }

        subscribeRoutine = null;
    }

    private void TrySubscribe()
    {
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

    private void Unsubscribe()
    {
        if (subscribedToEvidence && EvidenceNotebook.Instance != null)
        {
            EvidenceNotebook.Instance.OnEvidenceAdded -= HandleEvidenceAdded;
            subscribedToEvidence = false;
        }

        if (subscribedToReport && DeductionManager.Instance != null)
        {
            DeductionManager.Instance.OnReportSubmitted -= HandleReportSubmitted;
            subscribedToReport = false;
        }
    }

    public void PlayByKey(string key)
    {
        TryQueueDialogueByKey(key);
    }

    private bool TryQueueDialogueByKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        if (dialogueMap == null)
            BuildMap();

        if (!dialogueMap.TryGetValue(key, out DialogueActionEntry entry))
        {
            Debug.LogWarning("[DialogueTriggerAction] No dialogue found for key: " + key);
            return false;
        }

        return QueueDialogueEntry(entry);
    }

    private bool QueueDialogueEntry(DialogueActionEntry entry)
    {
        if (entry == null)
            return false;

        if (entry.playOnlyOnce && entry.hasPlayed)
            return false;

        if (entry.dialogue == null)
        {
            Debug.LogWarning("[DialogueTriggerAction] Dialogue is empty for key: " + entry.key);
            return false;
        }

        entry.hasPlayed = true;
        dialogueQueue.Enqueue(entry);
        StartQueueIfNeeded();
        return true;
    }

    private void StartQueueIfNeeded()
    {
        if (queueRoutine == null)
            queueRoutine = StartCoroutine(ProcessDialogueQueue());
    }

    private IEnumerator ProcessDialogueQueue()
    {
        while (true)
        {
            if (dialogueQueue.Count > 0)
            {
                DialogueActionEntry entry = dialogueQueue.Dequeue();
                yield return StartCoroutine(PlaySingleDialogue(entry));
                continue;
            }

            if (pendingAllEvidenceCheck)
            {
                pendingAllEvidenceCheck = false;

                if (waitForFoodScanAudioBeforeAllEvidenceDialogue)
                    yield return StartCoroutine(WaitForFoodScanAudioToFinish());

                TryQueueAllEvidenceDialogues();

                if (dialogueQueue.Count > 0)
                    continue;
            }

            break;
        }

        if (disableCanvasWhenQueueEnds && dialogueCanvasRoot != null)
            dialogueCanvasRoot.SetActive(false);

        queueRoutine = null;
    }

    private IEnumerator PlaySingleDialogue(DialogueActionEntry entry)
    {
        if (entry == null || entry.dialogue == null)
            yield break;

        if (waitUntilCurrentDialogueEnds)
            yield return StartCoroutine(WaitUntilDialogueFree());

        if (dialogueCanvasRoot != null && !dialogueCanvasRoot.activeSelf)
        {
            dialogueCanvasRoot.SetActive(true);
            yield return null;
        }

        if (DialogueManager.instance == null)
        {
            Debug.LogWarning("[DialogueTriggerAction] DialogueManager missing. Check DiaCanvas.");
            yield break;
        }

        if (entry.lockGameplayDuringDialogue)
            GameplayInputLock.Lock(entry.gameplayLockReason);

        DialogueManager.instance.EnqueueDialogue(entry.dialogue);

        while (IsDialogueActive())
            yield return null;

        yield return null;

        if (entry.lockGameplayDuringDialogue)
            GameplayInputLock.Unlock(entry.gameplayLockReason);

        yield return StartCoroutine(RunPostDialogueAction(entry));
    }

    private IEnumerator WaitUntilDialogueFree()
    {
        while (IsDialogueActive())
            yield return null;
    }

    private bool IsDialogueActive()
    {
        return DialogueManager.instance != null &&
               DialogueManager.instance.dialogueBox != null &&
               DialogueManager.instance.dialogueBox.activeSelf;
    }

    private IEnumerator WaitForFoodScanAudioToFinish()
    {
        float startTime = Time.time;

        while (AnyFoodScanAudioPlaying())
        {
            if (foodScanAudioWaitTimeout > 0f && Time.time - startTime >= foodScanAudioWaitTimeout)
            {
                Debug.LogWarning("[DialogueTriggerAction] FoodScan audio wait timed out. Check loopWhileHovering or whether the player is still holding scan.");
                break;
            }

            yield return null;
        }
    }

    private bool AnyFoodScanAudioPlaying()
    {
        FoodScan[] scans = FindObjectsOfType<FoodScan>();

        foreach (FoodScan scan in scans)
        {
            if (scan == null || scan.audioSource == null)
                continue;

            if (scan.audioSource.isPlaying)
                return true;
        }

        return false;
    }

    private void HandleEvidenceAdded(string foundEvidenceID, EvidenceChannel foundChannel)
    {
        TryQueueSingleEvidenceDialogue(foundEvidenceID, foundChannel);

        pendingAllEvidenceCheck = true;
        StartQueueIfNeeded();
    }

    private void TryQueueSingleEvidenceDialogue(string foundEvidenceID, EvidenceChannel foundChannel)
    {
        if (evidenceDialogueRules == null)
            return;

        foreach (EvidenceDialogueRule rule in evidenceDialogueRules)
        {
            if (rule == null)
                continue;

            if (rule.playOnlyOnce && rule.hasPlayed)
                continue;

            if (rule.evidenceID != foundEvidenceID)
                continue;

            bool channelMatched = (foundChannel & rule.requiredChannel) == rule.requiredChannel;

            if (!channelMatched)
                continue;

            if (TryQueueDialogueByKey(rule.dialogueKey))
                rule.hasPlayed = true;

            return;
        }
    }

    private void TryQueueAllEvidenceDialogues()
    {
        if (allEvidenceDialogueRules == null || EvidenceNotebook.Instance == null)
            return;

        foreach (AllEvidenceDialogueRule rule in allEvidenceDialogueRules)
        {
            if (rule == null)
                continue;

            if (rule.playOnlyOnce && rule.hasPlayed)
                continue;

            if (rule.requiredEvidence == null || rule.requiredEvidence.Length == 0)
                continue;

            if (!AllRequiredEvidenceFound(rule))
                continue;

            if (TryQueueDialogueByKey(rule.dialogueKey))
                rule.hasPlayed = true;

            return;
        }
    }

    private bool AllRequiredEvidenceFound(AllEvidenceDialogueRule rule)
    {
        foreach (RequiredEvidence required in rule.requiredEvidence)
        {
            if (required == null)
                continue;

            bool hasEvidence = EvidenceNotebook.Instance.HasEvidence(
                required.evidenceID,
                required.requiredChannel
            );

            if (logEvidenceChecks)
            {
                EvidenceChannel found = EvidenceNotebook.Instance.GetDiscoveredChannel(required.evidenceID);
                Debug.Log("[DialogueTriggerAction] Check " + required.evidenceID +
                          " | Required: " + required.requiredChannel +
                          " | Found: " + found +
                          " | Pass: " + hasEvidence);
            }

            if (!hasEvidence)
                return false;
        }

        if (logEvidenceChecks)
            Debug.Log("[DialogueTriggerAction] All required evidence found.");

        return true;
    }

    private void HandleReportSubmitted(bool reportCorrect)
    {
        if (!playAfterReportSubmit)
            return;

        string key = reportCorrect ? correctReportKey : incorrectReportKey;
        StartCoroutine(PlayReportDialogueAfterDelay(key));
    }

    private IEnumerator PlayReportDialogueAfterDelay(string key)
    {
        if (submitDialogueDelay > 0f)
            yield return new WaitForSeconds(submitDialogueDelay);

        TryQueueDialogueByKey(key);
    }

    public void ResetPlayedFlags()
    {
        if (actionDialogues != null)
        {
            foreach (DialogueActionEntry entry in actionDialogues)
            {
                if (entry != null)
                    entry.hasPlayed = false;
            }
        }

        if (evidenceDialogueRules != null)
        {
            foreach (EvidenceDialogueRule rule in evidenceDialogueRules)
            {
                if (rule != null)
                    rule.hasPlayed = false;
            }
        }

        if (allEvidenceDialogueRules != null)
        {
            foreach (AllEvidenceDialogueRule rule in allEvidenceDialogueRules)
            {
                if (rule != null)
                    rule.hasPlayed = false;
            }
        }

        pendingAllEvidenceCheck = false;
        dialogueQueue.Clear();
    }
    private IEnumerator RunPostDialogueAction(DialogueActionEntry entry)
    {
        if (entry == null || entry.postDialogueAction == PostDialogueAction.None)
            yield break;

        if (entry.postDialogueDelay > 0f)
            yield return new WaitForSeconds(entry.postDialogueDelay);

        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        switch (entry.postDialogueAction)
        {
            case PostDialogueAction.LoadNextScene:
                int nextIndex = currentIndex + 1;

                if (nextIndex >= SceneManager.sceneCountInBuildSettings)
                {
                    Debug.LogWarning("[DialogueTriggerAction] No next scene in Build Settings.");
                    yield break;
                }

                SceneManager.LoadScene(nextIndex);
                break;

            case PostDialogueAction.ReloadCurrentScene:
                SceneManager.LoadScene(currentIndex);
                break;

            case PostDialogueAction.LoadSceneByName:
                if (string.IsNullOrWhiteSpace(entry.sceneNameToLoad))
                {
                    Debug.LogWarning("[DialogueTriggerAction] Scene name is empty.");
                    yield break;
                }

                SceneManager.LoadScene(entry.sceneNameToLoad);
                break;
        }
    }
}
