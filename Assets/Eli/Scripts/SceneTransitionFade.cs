using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionFade : MonoBehaviour
{
    public static SceneTransitionFade Instance;

    [Header("UI")]
    public CanvasGroup canvasGroup;
    public TMP_Text messageText;

    [Header("Text")]
    public string defaultMessage = "TO BE CONTINUED...";

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
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
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

        StartCoroutine(TransitionRoutine(sceneIndex, message));
    }

    private IEnumerator TransitionRoutine(int sceneIndex, string message)
    {
        isTransitioning = true;

        UnlockCommonGameplayLocks();
        GameplayInputLock.Lock(transitionLockReason);

        if (messageText != null)
            messageText.text = string.IsNullOrWhiteSpace(message) ? defaultMessage : message;

        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        yield return FadeTo(1f, fadeInDuration);
        yield return new WaitForSeconds(holdDuration);

        SceneManager.LoadScene(sceneIndex);

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
