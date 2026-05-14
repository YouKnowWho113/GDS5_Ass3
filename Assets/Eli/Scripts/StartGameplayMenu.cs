using UnityEngine;
using UnityEngine.UI;

public class StartGameplayMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject startMenuPanel;
    public Button tableButton;

    [Header("Dialogue")]
    public DialogueTrigger dialogueTrigger;
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

        if (playDialogueAfterTableClick && dialogueTrigger != null)
            dialogueTrigger.PlaySceneStartManually();
    }

    private void SetMenuCursor()
    {
        Cursor.visible = true;

        if (menuCursorTexture != null)
            Cursor.SetCursor(menuCursorTexture, menuCursorHotspot, CursorMode.Auto);
    }
}