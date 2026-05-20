using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoteBookTabs : MonoBehaviour
{
    public GameObject overviewPanel;
    public GameObject foodImagePanel;
    public GameObject texTasPanel;
    public GameObject conclusionPanel;
    public GameObject instructionPanel;
    public GameObject menuPanel;

    public NoteBookEvidenceUI evidenceUI;

    public void OpenOverview()
    {
        overviewPanel.SetActive(true);
        foodImagePanel.SetActive(true);
        texTasPanel.SetActive(false);
        instructionPanel.SetActive(false);
        conclusionPanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    public void OpenTexTas()
    {
        overviewPanel.SetActive(false);
        foodImagePanel.SetActive(false);
        texTasPanel.SetActive(true);
        conclusionPanel.SetActive(false);
        instructionPanel.SetActive(false);
        menuPanel.SetActive(false);

        if (evidenceUI != null)
            evidenceUI.RefreshEvidence();
    }

    public void OpenConclusion()
    {
        overviewPanel.SetActive(false);
        foodImagePanel.SetActive(false);
        texTasPanel.SetActive(false);
        conclusionPanel.SetActive(true);
        instructionPanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    public void OpenInstruction()
    {
        overviewPanel.SetActive(false);
        foodImagePanel.SetActive(false);
        texTasPanel.SetActive(false);
        conclusionPanel.SetActive(false);
        instructionPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void OpenMenu()
    {
        overviewPanel.SetActive(false);
        foodImagePanel.SetActive(false);
        texTasPanel.SetActive(false);
        conclusionPanel.SetActive(false);
        instructionPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}