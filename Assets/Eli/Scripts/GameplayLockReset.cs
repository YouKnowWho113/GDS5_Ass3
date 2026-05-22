using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayLockReset : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameplayInputLock.ClearAllLocks();
        Cursor.visible = true;
        Debug.Log("[GameplayLockReset] Cleared gameplay locks on scene load.");
    }
}