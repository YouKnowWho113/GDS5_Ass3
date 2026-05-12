using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DialogueButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(GetNextLine);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(GetNextLine);
    }

    public void GetNextLine()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("[DialogueButton] DialogueManager missing.");
            return;
        }

        DialogueManager.Instance.AdvanceDialogue();
    }
}