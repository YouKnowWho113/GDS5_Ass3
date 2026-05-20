using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NoteBookToggle : MonoBehaviour
{
    private static GameObject journalPersist;
    public GameObject journal;
    public GameObject journalButton;
    public GameObject[] ovv;
    public Image[] foodImage;
    public GameObject[] tnt;
    public GameObject conclu;

    public NoteBookEvidenceUI evidenceUI;
    public NoteBookTabs noteBookTabs;
    public int curLevel;
    public int curPage;

    [Header("Input Lock")]
    public string gameplayLockReason = "Notebook";
    public bool showSystemCursorWhenOpen = true;

    private bool ownsNotebookLock;

    public bool[] lvS;

    private void Awake()
    {
        if (journalPersist == null)
        {
            journalPersist = gameObject;
            DontDestroyOnLoad(gameObject);
        }

        else if (journalPersist != gameObject)
        {
            Destroy(gameObject);
        }

        CloseJournal();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            JournalToggle();
        }

        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            curLevel = SceneManager.GetActiveScene().buildIndex;
        }

        if (curPage == curLevel && !lvS[curLevel])
        {
<<<<<<< Updated upstream
=======
            //tntPanel.SetActive(true);
>>>>>>> Stashed changes
            conclu.SetActive(true);
        }
        else
        {
<<<<<<< Updated upstream
=======
            //tntPanel.SetActive(false);
>>>>>>> Stashed changes
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

                    if (lvS[i])
                    {
                        ovv[i].SetActive(true); tnt[i].SetActive(true);
                        ovv[j].SetActive(false); tnt[j].SetActive(false);
                        ovv[11].SetActive(false); tnt[11].SetActive(false);
                        foodImage[i].color = Color.white;
                    }
                    else
                    {
                        ovv[i].SetActive(false); tnt[i].SetActive(false);
                        ovv[j].SetActive(false); tnt[j].SetActive(false);
                        ovv[11].SetActive(true); tnt[11].SetActive(true);
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
            curPage = curLevel;
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

    public void ChangePageLeft()
    {
        if (curPage != 0)
        {
            curPage--;
        }
        else
        {
            curPage = 10;
        }
    }

    public void ChangePageRight()
    {
        if (curPage != 10)
        {
            Debug.Log("Page not 0");
            curPage++;
        }
        else
        {
            Debug.Log("Page 0)");
            curPage = 0;
        }
    }

    public void LoadLv1()
    {
        curPage = 1;
        if (lvS[1])
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
        if (lvS[2])
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
        if (lvS[3])
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
        if (lvS[4])
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
        if (lvS[5])
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
        if (lvS[6])
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
        if (lvS[7])
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
        if (lvS[8])
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
        if (lvS[9])
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
        if (lvS[10])
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