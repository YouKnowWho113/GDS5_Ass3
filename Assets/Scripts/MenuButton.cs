using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Intro");
    }

   
    public void OpenCredits()
    {
        SceneManager.LoadScene("Credit");
    }

   
    public void ExitGame()
    {
        Debug.Log("Game Closed");

        Application.Quit();
    }
}