using UnityEngine;

[CreateAssetMenu(
    fileName = "NewFoodDialogue",
    menuName = "FoodDetective/Food Dialogue"
)]
public class FoodDialogue : ScriptableObject
{
    [Header("Action Dialogues")]
    public DialogueTriggerAction.DialogueActionEntry[] actionDialogues;

    [Header("Single Evidence Dialogue Rules")]
    public DialogueTriggerAction.EvidenceDialogueRule[] evidenceDialogueRules;

    [Header("All Evidence Dialogue Rules")]
    public DialogueTriggerAction.AllEvidenceDialogueRule[] allEvidenceDialogueRules;

    [Header("Report Submit")]
    public bool playAfterReportSubmit = true;
    public string correctReportKey = "after_correct_submit";
    public string incorrectReportKey = "after_wrong_submit";
    public float submitDialogueDelay = 0.2f;

    [Header("Wait Conditions")]
    public bool waitUntilCurrentDialogueEnds = true;
    public bool waitForFoodScanAudioBeforeAllEvidenceDialogue = true;
    public float foodScanAudioWaitTimeout = 6f;
}
