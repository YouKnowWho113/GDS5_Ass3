using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour

{
    public DialogueBase dialogue;
    public bool playOnStart = true;

    private bool hasPlayed = false;

    private void Start()
    {
        if (playOnStart)
        {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        if (hasPlayed) return;
        if (dialogue == null) return;
        if (DialogueManager.instance == null) return;

        hasPlayed = true;
        DialogueManager.instance.EnqueueDialogue(dialogue);
    }

    private void Update()
    {
        if (playOnStart) return;

        if (!hasPlayed &&
            Input.GetKeyDown(KeyCode.Space) &&
            DialogueManager.instance != null &&
            !DialogueManager.instance.dialogueBox.activeSelf)
        {
            TriggerDialogue();
        }
    }
}