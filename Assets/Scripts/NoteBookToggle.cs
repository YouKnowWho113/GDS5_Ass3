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
    public GameObject[] tnt;
    public Image[] foodImage;
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

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            JournalToggle();
        }

        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            curLevel = SceneManager.GetActiveScene().buildIndex;
        }

        if (curPage == 1 && lvS[1])
        {
            ovv[1].SetActive(true);
            ovv[2].SetActive(false); ovv[3].SetActive(false); ovv[4].SetActive(false); ovv[5].SetActive(false); ovv[6].SetActive(false);
            ovv[7].SetActive(false); ovv[8].SetActive(false); ovv[9].SetActive(false); ovv[10].SetActive(false); ovv[0].SetActive(false);

            tnt[1].SetActive(true);
            tnt[2].SetActive(false); tnt[3].SetActive(false); tnt[4].SetActive(false); tnt[5].SetActive(false); tnt[6].SetActive(false);
            tnt[7].SetActive(false); tnt[8].SetActive(false); tnt[9].SetActive(false); tnt[10].SetActive(false); tnt[0].SetActive(false);

            conclu.SetActive(false);
            foodImage[0].color = Color.white;
        }
        else if (curPage == 2 && lvS[2])
        {
            ovv[2].SetActive(true);
            ovv[1].SetActive(false); ovv[3].SetActive(false); ovv[4].SetActive(false); ovv[5].SetActive(false); ovv[6].SetActive(false);
            ovv[7].SetActive(false); ovv[8].SetActive(false); ovv[9].SetActive(false); ovv[10].SetActive(false); ovv[0].SetActive(false);

            tnt[2].SetActive(true);
            tnt[1].SetActive(false); tnt[3].SetActive(false); tnt[4].SetActive(false); tnt[5].SetActive(false); tnt[6].SetActive(false);
            tnt[7].SetActive(false); tnt[8].SetActive(false); tnt[9].SetActive(false); tnt[10].SetActive(false); tnt[0].SetActive(false);

            conclu.SetActive(false);
        }
        else if (curPage == 3 && lvS[3])
        {
            ovv[3].SetActive(true);
            ovv[1].SetActive(false); ovv[2].SetActive(false); ovv[4].SetActive(false); ovv[5].SetActive(false); ovv[6].SetActive(false);
            ovv[7].SetActive(false); ovv[8].SetActive(false); ovv[9].SetActive(false); ovv[10].SetActive(false); ovv[0].SetActive(false);

            tnt[3].SetActive(true);
            tnt[1].SetActive(false); tnt[2].SetActive(false); tnt[4].SetActive(false); tnt[5].SetActive(false); tnt[6].SetActive(false);
            tnt[7].SetActive(false); tnt[8].SetActive(false); tnt[9].SetActive(false); tnt[10].SetActive(false); tnt[0].SetActive(false);

            conclu.SetActive(false);
        }
        else if (curPage == 4 && lvS[4])
        {
            ovv[4].SetActive(true);
            ovv[1].SetActive(false); ovv[2].SetActive(false); ovv[3].SetActive(false); ovv[5].SetActive(false); ovv[6].SetActive(false);
            ovv[7].SetActive(false); ovv[8].SetActive(false); ovv[9].SetActive(false); ovv[10].SetActive(false); ovv[0].SetActive(false);

            tnt[4].SetActive(true);
            tnt[1].SetActive(false); tnt[2].SetActive(false); tnt[3].SetActive(false); tnt[5].SetActive(false); tnt[6].SetActive(false);
            tnt[7].SetActive(false); tnt[8].SetActive(false); tnt[9].SetActive(false); tnt[10].SetActive(false); tnt[0].SetActive(false);

            conclu.SetActive(false);
        }
        else if (curPage == 5 && lvS[5])
        {
            ovv[5].SetActive(true);
            ovv[1].SetActive(false); ovv[2].SetActive(false); ovv[3].SetActive(false); ovv[4].SetActive(false); ovv[6].SetActive(false);
            ovv[7].SetActive(false); ovv[8].SetActive(false); ovv[9].SetActive(false); ovv[10].SetActive(false); ovv[0].SetActive(false);

            tnt[5].SetActive(true);
            tnt[1].SetActive(false); tnt[2].SetActive(false); tnt[3].SetActive(false); tnt[4].SetActive(false); tnt[6].SetActive(false);
            tnt[7].SetActive(false); tnt[8].SetActive(false); tnt[9].SetActive(false); tnt[10].SetActive(false); tnt[0].SetActive(false);

            conclu.SetActive(false);
        }
        else if (curPage == 6 && lvS[6])
        {
            ovv[6].SetActive(true);
            ovv[1].SetActive(false); ovv[2].SetActive(false); ovv[3].SetActive(false); ovv[4].SetActive(false); ovv[5].SetActive(false);
            ovv[7].SetActive(false); ovv[8].SetActive(false); ovv[9].SetActive(false); ovv[10].SetActive(false); ovv[0].SetActive(false);

            tnt[6].SetActive(true);
            tnt[1].SetActive(false); tnt[2].SetActive(false); tnt[3].SetActive(false); tnt[4].SetActive(false); tnt[5].SetActive(false);
            tnt[7].SetActive(false); tnt[8].SetActive(false); tnt[9].SetActive(false); tnt[10].SetActive(false); tnt[0].SetActive(false);

            conclu.SetActive(false);
        }
        else if (curPage == 7 && lvS[7])
        {
            ovv[7].SetActive(true);
            ovv[1].SetActive(false); ovv[2].SetActive(false); ovv[3].SetActive(false); ovv[4].SetActive(false); ovv[5].SetActive(false);
            ovv[6].SetActive(false); ovv[8].SetActive(false); ovv[9].SetActive(false); ovv[10].SetActive(false); ovv[0].SetActive(false);

            tnt[7].SetActive(true);
            tnt[1].SetActive(false); tnt[2].SetActive(false); tnt[3].SetActive(false); tnt[4].SetActive(false); tnt[5].SetActive(false);
            tnt[6].SetActive(false); tnt[8].SetActive(false); tnt[9].SetActive(false); tnt[10].SetActive(false); tnt[0].SetActive(false);

            conclu.SetActive(false);
        }
        else if (curPage == 8 && lvS[8])
        {
            ovv[8].SetActive(true);
            ovv[1].SetActive(false); ovv[2].SetActive(false); ovv[3].SetActive(false); ovv[4].SetActive(false); ovv[5].SetActive(false);
            ovv[6].SetActive(false); ovv[7].SetActive(false); ovv[9].SetActive(false); ovv[10].SetActive(false); ovv[0].SetActive(false);

            tnt[8].SetActive(true);
            tnt[1].SetActive(false); tnt[2].SetActive(false); tnt[3].SetActive(false); tnt[4].SetActive(false); tnt[5].SetActive(false);
            tnt[6].SetActive(false); tnt[7].SetActive(false); tnt[9].SetActive(false); tnt[10].SetActive(false); tnt[0].SetActive(false);

            conclu.SetActive(false);
        }
        else if (curPage == 9 && lvS[9])
        {
            ovv[9].SetActive(true);
            ovv[1].SetActive(false); ovv[2].SetActive(false); ovv[3].SetActive(false); ovv[4].SetActive(false); ovv[5].SetActive(false);
            ovv[6].SetActive(false); ovv[7].SetActive(false); ovv[8].SetActive(false); ovv[10].SetActive(false); ovv[0].SetActive(false);

            tnt[9].SetActive(true);
            tnt[1].SetActive(false); tnt[2].SetActive(false); tnt[3].SetActive(false); tnt[4].SetActive(false); tnt[5].SetActive(false);
            tnt[6].SetActive(false); tnt[7].SetActive(false); tnt[8].SetActive(false); tnt[10].SetActive(false); tnt[0].SetActive(false);

            conclu.SetActive(false);
            foodImage[8].color = Color.white;
        }
        else if (curPage == 10 && lvS[10])
        {
            ovv[10].SetActive(true);
            ovv[1].SetActive(false); ovv[2].SetActive(false); ovv[3].SetActive(false); ovv[4].SetActive(false); ovv[5].SetActive(false);
            ovv[6].SetActive(false); ovv[7].SetActive(false); ovv[8].SetActive(false); ovv[9].SetActive(false); ovv[0].SetActive(false);

            tnt[10].SetActive(true);
            tnt[1].SetActive(false); tnt[2].SetActive(false); tnt[3].SetActive(false); tnt[4].SetActive(false); tnt[5].SetActive(false);
            tnt[6].SetActive(false); tnt[7].SetActive(false); tnt[8].SetActive(false); tnt[9].SetActive(false); tnt[0].SetActive(false);

            conclu.SetActive(false);
        }
        else
        {
            ovv[0].SetActive(true);
            ovv[1].SetActive(false); ovv[2].SetActive(false); ovv[3].SetActive(false); ovv[4].SetActive(false); ovv[5].SetActive(false);
            ovv[6].SetActive(false); ovv[7].SetActive(false); ovv[8].SetActive(false); ovv[9].SetActive(false); ovv[10].SetActive(false);

            tnt[0].SetActive(true);
            tnt[1].SetActive(false); tnt[2].SetActive(false); tnt[3].SetActive(false); tnt[4].SetActive(false); tnt[5].SetActive(false);
            tnt[6].SetActive(false); tnt[7].SetActive(false); tnt[8].SetActive(false); tnt[9].SetActive(false); tnt[10].SetActive(false);

            conclu.SetActive(true);
        }

        if (lvS[1] && lvS[2] && lvS[3] && lvS[4] && lvS[5] && lvS[6] && lvS[7] && lvS[8] && lvS[9] && lvS[10])
        {
            lvS[0] = true;
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
            journal.SetActive(false);

        if (ownsNotebookLock)
        {
            GameplayInputLock.Unlock(gameplayLockReason);
            ownsNotebookLock = false;
        }
    }

    public void MainMenu()
    {
        CloseJournal();
        journalButton.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void ChangePageLeft()
    {
        if (curPage != 1)
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
            curPage++;
        }
        else
        {
            curPage = 1;
        }
    }

    public void LoadLv1()
    {
        CloseJournal();
        curPage = 1;
        if (lvS[1])
        {
            noteBookTabs.OpenOverview();
        }
        else
        {
            noteBookTabs.OpenConclusion();
        }
        SceneManager.LoadScene(1);
    }

    public void LoadLv2()
    {
        CloseJournal();
        curPage = 2;
        if (lvS[2])
        {
            noteBookTabs.OpenOverview();
        }
        else
        {
            noteBookTabs.OpenConclusion();
        }
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