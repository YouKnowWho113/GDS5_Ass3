using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NoteBookToggle : MonoBehaviour
{
    //private static GameObject journalPersist;
    public GameObject journal;
    public GameObject journalButton;
    public GameObject[] ovv;
    public Image[] foodImage;
    public GameObject tnt;
    public GameObject tntPanel;
    public GameObject conclu;
    public GameObject levelCompCheck;

    public GameObject leftArrow;
    public GameObject rightArrow;

    public NoteBookEvidenceUI evidenceUI;
    public NoteBookTabs noteBookTabs;
    public int curPage;

    [Header("Input Lock")]
    public string gameplayLockReason = "Notebook";
    public bool showSystemCursorWhenOpen = true;

    private bool ownsNotebookLock;
    
    private void Awake()
    {
        if (levelCompCheck == null)
        {
            levelCompCheck = GameObject.Find("LevelCompCheck");
        }

        CloseJournal();
    }
    
    public void ChangePageLeft()
    {
        if (curPage != 0)
        {
            curPage--;
        }
    }

    public void ChangePageRight()
    {
        if (curPage != 10)
        {
            curPage++;
        }
    }

    public void Update()
    {
        if (curPage == 0)
        {
            leftArrow.SetActive(false);
        }
        else
        {
            leftArrow.SetActive(true);
        }

        if (curPage >= 10)
        {
            rightArrow.SetActive(false);
        }
        else
        {
            rightArrow.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            JournalToggle();
        }

        if (curPage == levelCompCheck.GetComponent<LevelCompCheck>().curLevel
            && !levelCompCheck.GetComponent<LevelCompCheck>().lvS[levelCompCheck.GetComponent<LevelCompCheck>().curLevel])
        {
            tntPanel.SetActive(true);
            conclu.SetActive(true);
        }
        else
        {
            tntPanel.SetActive(false);
            conclu.SetActive(false);
        }

        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                if (j != i && i == curPage)
                {
                    foodImage[i].gameObject.SetActive(true);
                    foodImage[j].gameObject.SetActive(false);

                    if (levelCompCheck.GetComponent<LevelCompCheck>().lvS[i])
                    {
                        ovv[i].SetActive(true);
                        ovv[j].SetActive(false);
                        ovv[11].SetActive(false);
                        foodImage[i].color = Color.white;
                    }
                    else
                    {
                        ovv[i].SetActive(false);
                        ovv[j].SetActive(false);
                        ovv[11].SetActive(true);
                        foodImage[i].color = Color.black;
                    }
                }
            }
        }
    }

    public void JournalToggle()
    {
        bool newState = !journal.activeSelf;

        if (newState)
        {
            noteBookTabs.OpenConclusion();
            journal.SetActive(true);

            GameplayInputLock.Lock(gameplayLockReason);
            ownsNotebookLock = true;

            if (showSystemCursorWhenOpen)
                Cursor.visible = true;

            if (evidenceUI != null)
                evidenceUI.RefreshEvidence();
        }
        else
        {
            CloseJournal();
        }
    }

    private void CloseJournal()
    {
        if (journal != null)
        {
            curPage = levelCompCheck.GetComponent<LevelCompCheck>().curLevel;
            journal.SetActive(false);
        }

        if (ownsNotebookLock)
        {
            GameplayInputLock.Unlock(gameplayLockReason);
            ownsNotebookLock = false;
        }
    }

    public void MainMenu()
    {
        journalButton.SetActive(false);
        CloseJournal();
        SceneManager.LoadScene(0);
    }

    public void LoadLv1()
    {
        curPage = 1;
        if (levelCompCheck.GetComponent<LevelCompCheck>().lvS[1])
        {
            noteBookTabs.OpenOverview();
        }
        else
        {
            noteBookTabs.OpenConclusion();
        }
        CloseJournal();
        SceneManager.LoadScene(1);
    }

    public void LoadLv2()
    {
        curPage = 2;
        if (levelCompCheck.GetComponent<LevelCompCheck>().lvS[2])
        {
            noteBookTabs.OpenOverview();
        }
        else
        {
            noteBookTabs.OpenConclusion();
        }
        CloseJournal();
        SceneManager.LoadScene(2);
    }

    public void LoadLv3()
    {
        curPage = 3;
        if (levelCompCheck.GetComponent<LevelCompCheck>().lvS[3])
        {
            noteBookTabs.OpenOverview();
        }
        else
        {
            noteBookTabs.OpenConclusion();
        }
        CloseJournal();
        SceneManager.LoadScene(3);
    }

    public void LoadLv4()
    {
        curPage = 4;
        if (levelCompCheck.GetComponent<LevelCompCheck>().lvS[4])
        {
            noteBookTabs.OpenOverview();
        }
        else
        {
            noteBookTabs.OpenConclusion();
        }
        CloseJournal();
        SceneManager.LoadScene(4);
    }

    public void LoadLv5()
    {
        curPage = 5;
        if (levelCompCheck.GetComponent<LevelCompCheck>().lvS[5])
        {
            noteBookTabs.OpenOverview();
        }
        else
        {
            noteBookTabs.OpenConclusion();
        }
        CloseJournal();
        SceneManager.LoadScene(5);
    }

    public void LoadLv6()
    {
        curPage = 6;
        if (levelCompCheck.GetComponent<LevelCompCheck>().lvS[6])
        {
            noteBookTabs.OpenOverview();
        }
        else
        {
            noteBookTabs.OpenConclusion();
        }
        CloseJournal();
        SceneManager.LoadScene(6);
    }

    public void LoadLv7()
    {
        curPage = 7;
        if (levelCompCheck.GetComponent<LevelCompCheck>().lvS[7])
        {
            noteBookTabs.OpenOverview();
        }
        else
        {
            noteBookTabs.OpenConclusion();
        }
        CloseJournal();
        SceneManager.LoadScene(7);
    }

    public void LoadLv8()
    {
        curPage = 8;
        if (levelCompCheck.GetComponent<LevelCompCheck>().lvS[8])
        {
            noteBookTabs.OpenOverview();
        }
        else
        {
            noteBookTabs.OpenConclusion();
        }
        CloseJournal();
        SceneManager.LoadScene(8);
    }

    public void LoadLv9()
    {
        curPage = 9;
        if (levelCompCheck.GetComponent<LevelCompCheck>().lvS[9])
        {
            noteBookTabs.OpenOverview();
        }
        else
        {
            noteBookTabs.OpenConclusion();
        }
        CloseJournal();
        SceneManager.LoadScene(9);
    }

    public void LoadLv10()
    {
        curPage = 10;
        if (levelCompCheck.GetComponent<LevelCompCheck>().lvS[10])
        {
            noteBookTabs.OpenOverview();
        }
        else
        {
            noteBookTabs.OpenConclusion();
        }
        CloseJournal();
        SceneManager.LoadScene(10);
    }
}