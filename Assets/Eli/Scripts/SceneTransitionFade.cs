using System.Collections;
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
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
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

    public void TransitionToScene(int sceneIndex, string message = "")
    {
        if (isTransitioning)
            return;

        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("[SceneTransitionFade] Invalid scene index: " + sceneIndex);
            return;
        }

        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        StartCoroutine(TransitionRoutine(currentIndex, sceneIndex, message));
    }

    private string GetTransitionMessage(int fromSceneIndex, int toSceneIndex)
    {
        if (transitionTexts != null)
        {
            foreach (TransitionTextEntry entry in transitionTexts)
            {
                if (entry.fromSceneIndex == fromSceneIndex &&
                    entry.toSceneIndex == toSceneIndex &&
                    !string.IsNullOrWhiteSpace(entry.message))
                {
                    return entry.message;
                }
            }
        }

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

        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        yield return FadeTo(1f, fadeInDuration);
        yield return new WaitForSeconds(holdDuration);

        SceneManager.LoadScene(toSceneIndex);

        yield return null;

        UnlockCommonGameplayLocks();

        yield return FadeTo(0f, fadeOutDuration);

        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

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
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
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