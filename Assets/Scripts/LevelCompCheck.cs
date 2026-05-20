using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompCheck : MonoBehaviour
{
    private static GameObject levelCheckPersist;
    public int curLevel;
    public bool[] lvS;

    private void Awake()
    {
        if (levelCheckPersist == null)
        {
            levelCheckPersist = gameObject;
            DontDestroyOnLoad(gameObject);
        }

        else if (levelCheckPersist != gameObject)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            curLevel = SceneManager.GetActiveScene().buildIndex;
        }
    }
}
