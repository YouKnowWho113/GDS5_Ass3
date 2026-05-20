using System.Collections;
using UnityEngine;
using TMPro;

public class ToBeContinued : MonoBehaviour
{
    public static ToBeContinued Instance;

    [Header("References")]
    public CanvasGroup canvasGroup;
    public TMP_Text messageText;

    [Header("Text")]
    public string message = "TO BE CONTINUED...";

    [Header("Fade")]
    public float fadeDuration = 1.5f;
    public float finalAlpha = 1f;

    [Header("Gameplay Lock")]
    public bool lockGameplay = true;
    public string lockReason = "ToBeContinued";

    private Coroutine fadeRoutine;

    private void Awake()
    {
        Instance = this;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (messageText != null)
            messageText.text = message;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        gameObject.SetActive(false);
    }

    public void Play()
    {
        gameObject.SetActive(true);

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        if (lockGameplay)
            GameplayInputLock.Lock(lockReason);

        if (messageText != null)
            messageText.text = message;

        if (canvasGroup == null)
            yield break;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        float timer = 0f;
        canvasGroup.alpha = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, finalAlpha, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = finalAlpha;
    }
}