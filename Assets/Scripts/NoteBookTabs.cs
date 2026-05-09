using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBookTabs : MonoBehaviour
{
    public GameObject overviewPanel;
    public GameObject tastePanel;
    public GameObject texturePanel;

    public void OpenOverview()
    {
        overviewPanel.SetActive(true);
        tastePanel.SetActive(false);
        texturePanel.SetActive(false);
    }

    public void OpenTaste()
    {
        overviewPanel.SetActive(false);
        tastePanel.SetActive(true);
        texturePanel.SetActive(false);
    }

    public void OpenTexture()
    {
        overviewPanel.SetActive(false);
        tastePanel.SetActive(false);
        texturePanel.SetActive(true);
    }
}