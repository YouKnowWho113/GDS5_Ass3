using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBookToggle : MonoBehaviour
{
    public GameObject notebookPanel;
    public NoteBookEvidenceUI evidenceUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool newState = !notebookPanel.activeSelf;

            notebookPanel.SetActive(newState);

            if (newState)
            {
                evidenceUI.RefreshEvidence();
            }
        }
    }
}