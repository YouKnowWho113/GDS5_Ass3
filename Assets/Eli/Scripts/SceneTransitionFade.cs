using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionFade : MonoBehaviour
{
    public static SceneTransitionFade Instance;

    [System.Serializable]
    public class TransitionTextEntry
    {
        public int fromSceneIndex;
        public int toSceneIndex;
        [TextArea(2, 4)] public string message;
    }

    [Header("UI")]
    public CanvasGroup canvasGroup;
    public TMP_Text messageText;

    [Header("Default Text")]
    [TextArea(2, 4)]
    public string defaultMessage = "TO BE CONTINUED...";

    [Header("Scene Transition Texts")]
    public TransitionTextEntry[] transitionTexts;

    [Header("Timing")]
    public float fadeInDuration = 1f;
    public float holdDuration = 0.8f;
    public float fadeOutDuration = 1f;

    [Header("Gameplay Lock")]
    public string transitionLockReason = "SceneTransition";
    public string notebookLockReason = "Notebook";
    public string dialogueLockReason = "Dialogue";
    public string reportLockReason = "Report";

    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[SceneTransitionFade] Duplicate destroyed in scene: " + gameObject.scene.name);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        HideCanvasInstant();
    }

    public void TransitionToNextScene(string message = "")
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        TransitionToScene(nextIndex, message);
    }

    public void ReloadCurrentScene(string message = "")
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        TransitionToScene(currentIndex, message);
    }

    public void TransitionToSceneByName(string sceneName, string message = "")
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("[SceneTransitionFade] Scene name is empty.");
            return;
        }

        int sceneIndex = GetBuildIndexBySceneName(sceneName);

        if (sceneIndex < 0)
        {
            Debug.LogWarning("[SceneTransitionFade] Scene not found in Build Settings: " + sceneName);
            return;
        }

        TransitionToScene(sceneIndex, message);
    }

    public void TransitionToScene(int sceneIndex, string message = "")
    {
        if (isTransitioning)
        {
            Debug.LogWarning("[SceneTransitionFade] Transition ignored because another transition is already running.");
            return;
        }

        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("[SceneTransitionFade] Invalid scene index: " + sceneIndex);
            return;
        }

        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        Debug.Log("[SceneTransitionFade] Transition requested: " + currentIndex + " -> " + sceneIndex);

        StartCoroutine(TransitionRoutine(currentIndex, sceneIndex, message));
    }

    private int GetBuildIndexBySceneName(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string foundSceneName = Path.GetFileNameWithoutExtension(scenePath);

            if (foundSceneName == sceneName)
                return i;
        }

        return -1;
    }

    private string GetTransitionMessage(int fromSceneIndex, int toSceneIndex)
    {
        Debug.Log("[SceneTransitionFade] Looking for text: " + fromSceneIndex + " -> " + toSceneIndex);

        if (transitionTexts != null)
        {
            foreach (TransitionTextEntry entry in transitionTexts)
            {
                if (entry == null)
                    continue;

                if (entry.fromSceneIndex == fromSceneIndex &&
                    entry.toSceneIndex == toSceneIndex &&
                    !string.IsNullOrWhiteSpace(entry.message))
                {
                    Debug.Log("[SceneTransitionFade] Found transition text: " + entry.message);
                    return entry.message;
                }
            }
        }

        Debug.LogWarning("[SceneTransitionFade] No transition text found for " + fromSceneIndex + " -> " + toSceneIndex + ". Using default.");
        return defaultMessage;
    }

    private IEnumerator TransitionRoutine(int fromSceneIndex, int toSceneIndex, string message)
    {
        isTransitioning = true;

        UnlockCommonGameplayLocks();
        GameplayInputLock.Lock(transitionLockReason);

        string finalMessage = string.IsNullOrWhiteSpace(message)
            ? GetTransitionMessage(fromSceneIndex, toSceneIndex)
            : message;

        if (messageText != null)
            messageText.text = finalMessage;

        ShowCanvasBlockInput();

        yield return FadeTo(1f, fadeInDuration);
        yield return new WaitForSecondsRealtime(holdDuration);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(toSceneIndex);

        while (loadOperation != null && !loadOperation.isDone)
            yield return null;

        yield return FadeTo(0f, fadeOutDuration);

        HideCanvasInstant();
        UnlockCommonGameplayLocks();

        isTransitioning = false;
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (canvasGroup == null)
            yield break;

        float startAlpha = canvasGroup.alpha;

        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private void ShowCanvasBlockInput()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void HideCanvasInstant()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void UnlockCommonGameplayLocks()
    {
        GameplayInputLock.Unlock(notebookLockReason);
        GameplayInputLock.Unlock(dialogueLockReason);
        GameplayInputLock.Unlock(reportLockReason);
        GameplayInputLock.Unlock(transitionLockReason);

        Cursor.visible = true;
    }
}
