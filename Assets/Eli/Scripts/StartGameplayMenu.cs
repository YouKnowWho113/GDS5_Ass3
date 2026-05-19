using UnityEngine;
using UnityEngine.UI;

public class StartGameplayMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject startMenuPanel;
    public Button tableButton;

    [Header("Dialogue - Old Trigger")]
    public DialogueTrigger dialogueTrigger;

    [Header("Dialogue - Action Trigger")]
    public DialogueTriggerAction dialogueTriggerAction;
    public string dialogueKeyAfterTableClick = "after_table_click";

    [Header("Dialogue Settings")]
    public bool playDialogueAfterTableClick = true;

    [Header("Cursor")]
    public Texture2D menuCursorTexture;
    public Vector2 menuCursorHotspot = Vector2.zero;

    [Header("Gameplay Cursor")]
    public bool hideSystemCursorWhenGameplayStarts = true;

    private void Awake()
    {
        if (tableButton != null)
        {
            tableButton.onClick.RemoveListener(BeginGameplay);
            tableButton.onClick.AddListener(BeginGameplay);
        }
    }

    private void Start()
    {
        if (startMenuPanel != null)
            startMenuPanel.SetActive(true);

        SetMenuCursor();
    }

    private void OnDestroy()
    {
        if (tableButton != null)
            tableButton.onClick.RemoveListener(BeginGameplay);
    }

    public void BeginGameplay()
    {
        if (startMenuPanel != null)
            startMenuPanel.SetActive(false);

        if (hideSystemCursorWhenGameplayStarts)
            Cursor.visible = false;

        if (!playDialogueAfterTableClick)
            return;

        // Preferred: action/key-based dialogue trigger.
        if (dialogueTriggerAction != null)
        {
            dialogueTriggerAction.PlayByKey(dialogueKeyAfterTableClick);
            return;
        }

        // Fallback: old dialogue trigger.
        if (dialogueTrigger != null)
        {
            dialogueTrigger.PlaySceneStartManually();
            return;
        }

        Debug.LogWarning("[StartGameplayMenu] No DialogueTrigger or DialogueTriggerAction assigned.");
    }

    private void SetMenuCursor()
    {
        Cursor.visible = true;

        if (menuCursorTexture != null)
            Cursor.SetCursor(menuCursorTexture, menuCursorHotspot, CursorMode.Auto);
    }
}