using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompCheck : MonoBehaviour
{
    private static GameObject levelCheckPersist;
    public static LevelCompCheck Instance;
    public int curLevel;
    public bool[] lvS;

    private void Awake()
    {
        if (levelCheckPersist == null)
        {
            levelCheckPersist = gameObject;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (levelCheckPersist != gameObject)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            curLevel = SceneManager.GetActiveScene().buildIndex;
        }
    }
    public void MarkCurrentLevelComplete()
    {
        int level = SceneManager.GetActiveScene().buildIndex;

        if (level <= 0)
            return;

        curLevel = level;

        if (lvS != null && level < lvS.Length)
        {
            lvS[level] = true;
            Debug.Log("[LevelCompCheck] Level completed: " + level);
        }
        else
        {
            Debug.LogWarning("[LevelCompCheck] lvS array missing or too small for level: " + level);
        }
    }
}
