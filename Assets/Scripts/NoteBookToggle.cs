using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoteBookToggle : MonoBehaviour
{
    private static GameObject journalPersist;
    public GameObject journal;
    public GameObject journalButton;
    public NoteBookEvidenceUI evidenceUI;
    int curLevel;

    private void Awake()
    {
        if (journalPersist == null)
        {
            journalPersist = gameObject;
            DontDestroyOnLoad(gameObject);
        }

        else if (journalPersist != gameObject)
            Destroy(gameObject);

        journal.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            JournalToggle();
        }
    }

    public void JournalToggle()
    {
        bool newState = !journal.activeSelf;

        journal.SetActive(newState);

        if (newState)
        {
            evidenceUI.RefreshEvidence();
        }
    }

    public void MainMenu()
    {
        journal.SetActive(false);
        journalButton.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void LoadLv1()
    {
        curLevel = 1;
        journal.SetActive(false);
        SceneManager.LoadScene(1);
    }

    public void LoadLv2()
    {
        curLevel = 2;
        journal.SetActive(false);
        SceneManager.LoadScene(2);
    }

    public void LoadLv3()
    {
        curLevel = 3;
        journal.SetActive(false);
        SceneManager.LoadScene(3);
    }

    public void LoadLv4()
    {
        curLevel = 4;
        journal.SetActive(false);
        SceneManager.LoadScene(4);
    }

    public void LoadLv5()
    {
        curLevel = 5;
        journal.SetActive(false);
        SceneManager.LoadScene(5);
    }

    public void LoadLv6()
    {
        curLevel = 6;
        journal.SetActive(false);
        SceneManager.LoadScene(6);
    }

    public void LoadLv7()
    {
        curLevel = 7;
        journal.SetActive(false);
        SceneManager.LoadScene(7);
    }

    public void LoadLv8()
    {
        curLevel = 8;
        journal.SetActive(false);
        SceneManager.LoadScene(8);
    }

    public void LoadLv9()
    {
        curLevel = 9;
        journal.SetActive(false);
        SceneManager.LoadScene(9);
    }

    public void LoadLv10()
    {
        curLevel = 10;
        journal.SetActive(false);
        SceneManager.LoadScene(10);
    }
}