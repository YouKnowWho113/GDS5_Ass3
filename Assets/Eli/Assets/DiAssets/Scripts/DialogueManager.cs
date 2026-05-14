using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public static DialogueManager instance; // legacy support

    [Header("Input")]
    public bool allowSubmitInput = true;
    public string submitButtonName = "Submit";

    [Header("Button Object")]
    public Button nextLineButton;
    public bool useNextLineButton = true;

    [Header("Animators")]
    public Animator textBoxAnimator;
    public Animator nameTagAnimator;
    public Animator boxTextAnimator;
    public Animator nameTextAnimator;
    public Animator bumperAnimator;

    [Header("UI")]
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueName;
    public TextMeshProUGUI dialogueText;
    public Image dialoguePortrait;
    public Image dialogueEyes;
    public Image dialogueMouths;
    public Image dialogueTextBox;

    private readonly Queue<DialogueBase.Info> dialogueInfo = new Queue<DialogueBase.Info>();

    private Sprite eyeBlink;
    private Sprite mouth2;
    private Sprite mouth3;

    private int xEyes;
    private int yEyes;
    private float xSprite;
    private float ySprite;

    private bool shadowCheck;
    private bool continueCheck;
    private bool dialogueActive;

    private Coroutine eyeRoutine;
    private Coroutine mouthRoutine;
    private Coroutine portraitMoveRoutine;
    private Coroutine spriteFadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        instance = this;

        if (dialogueBox != null)
            dialogueBox.SetActive(false);
    }

    private void OnEnable()
    {
        RegisterNextButton();
    }

    private void OnDisable()
    {
        UnregisterNextButton();
    }

    private void OnDestroy()
    {
        UnregisterNextButton();
    }

    private void Update()
    {
        if (!allowSubmitInput)
            return;

        if (!dialogueActive)
            return;

        if (Input.GetButtonDown(submitButtonName))
        {
            AdvanceDialogue();
        }
    }

    private void RegisterNextButton()
    {
        if (!useNextLineButton || nextLineButton == null)
            return;

        nextLineButton.onClick.RemoveListener(AdvanceDialogue);
        nextLineButton.onClick.AddListener(AdvanceDialogue);
    }

    private void UnregisterNextButton()
    {
        if (nextLineButton == null)
            return;

        nextLineButton.onClick.RemoveListener(AdvanceDialogue);
    }

    public void EnqueueDialogue(DialogueBase db)
    {
        if (db == null)
        {
            Debug.LogWarning("[DialogueManager] No DialogueBase assigned.");
            return;
        }

        if (dialogueBox == null)
        {
            Debug.LogWarning("[DialogueManager] Dialogue box missing.");
            return;
        }

        dialogueInfo.Clear();

        foreach (DialogueBase.Info info in db.dialogueInfo)
        {
            dialogueInfo.Enqueue(info);
        }

        dialogueActive = true;
        dialogueBox.SetActive(true);

        if (SFXManager.instance != null)
            SFXManager.instance.PlayDialogueOpen();

        ShowNextLine(false);
    }

    public void AdvanceDialogue()
    {
        if (!dialogueActive)
            return;

        ShowNextLine(true);
    }

    // Legacy name so older scripts still work.
    public void DequeueDialogue()
    {
        AdvanceDialogue();
    }

    private void ShowNextLine(bool playNextSfx)
    {
        if (dialogueInfo.Count == 0)
        {
            EndDialogue();
            return;
        }

        if (playNextSfx && SFXManager.instance != null)
            SFXManager.instance.PlayDialogueNext();

        StopFaceCoroutines();

        DialogueBase.Info info = dialogueInfo.Dequeue();
        ApplyDialogueLine(info);
        PlayTextBoxAnim();

        if (!continueCheck)
        {
            if (dialoguePortrait != null)
                portraitMoveRoutine = StartCoroutine(PortraitMove());

            if (dialoguePortrait != null)
                spriteFadeRoutine = StartCoroutine(SpriteFade());
        }

        if (dialogueEyes != null && eyeBlink != null)
            eyeRoutine = StartCoroutine(EyeAnimate());

        if (dialogueMouths != null && mouth2 != null && mouth3 != null)
            mouthRoutine = StartCoroutine(MouthAnimate());
    }

    private void ApplyDialogueLine(DialogueBase.Info info)
    {
        if (info == null)
            return;

        if (dialogueText != null)
            dialogueText.text = info.charText;

        if (info.character == null)
        {
            if (dialogueName != null)
                dialogueName.text = "";

            return;
        }

        info.ChangeEmotion();

        if (dialogueName != null)
            dialogueName.text = info.character.myName;

        if (dialoguePortrait != null)
            dialoguePortrait.sprite = info.character.EmotionBaseDisplay;

        eyeBlink = info.character.EmotionBlinkDisplay;
        mouth2 = info.character.EmotionMouth2Display;
        mouth3 = info.character.EmotionMouth3Display;

        xEyes = info.character.xFace;
        yEyes = info.character.yFace;
        xSprite = info.character.xBase;
        ySprite = info.character.yBase;

        shadowCheck = info.character.shadowStatus;
        continueCheck = info.continueCheck;

        if (dialogueEyes != null)
            dialogueEyes.sprite = eyeBlink;

        if (dialogueMouths != null)
            dialogueMouths.sprite = mouth2;
    }

    private void PlayTextBoxAnim()
    {
        if (shadowCheck)
        {
            if (continueCheck)
            {
                PlayAnimator(textBoxAnimator, "YBoxContinue");
                PlayAnimator(nameTagAnimator, "yelTagContinue");
            }
            else
            {
                PlayAnimator(textBoxAnimator, "YBoxEntry");
                PlayAnimator(nameTagAnimator, "yelTagENtry");
            }
        }
        else
        {
            if (continueCheck)
            {
                PlayAnimator(textBoxAnimator, "RBoxContinue");
                PlayAnimator(nameTagAnimator, "redTagContinue");
            }
            else
            {
                PlayAnimator(textBoxAnimator, "RBoxENtry");
                PlayAnimator(nameTagAnimator, "redTagEntry");
            }
        }

        PlayAnimator(boxTextAnimator, "textBoxEntry");
        PlayAnimator(nameTextAnimator, "nameTextEntry");
        PlayAnimator(bumperAnimator, "bumperEntry");
    }

    private void PlayAnimator(Animator animator, string stateName)
    {
        if (animator == null || string.IsNullOrWhiteSpace(stateName))
            return;

        animator.Play(stateName, 0, 0f);
    }

    private IEnumerator PortraitMove()
    {
        float slideDistance = 12f;

        while (slideDistance >= 0f)
        {
            if (dialoguePortrait != null)
                dialoguePortrait.rectTransform.anchoredPosition =
                    new Vector3(xSprite - slideDistance, ySprite, 0f);

            if (dialogueEyes != null)
                dialogueEyes.rectTransform.anchoredPosition =
                    new Vector3(xEyes - slideDistance, yEyes, 0f);

            if (dialogueMouths != null)
                dialogueMouths.rectTransform.anchoredPosition =
                    new Vector3(xEyes - slideDistance, yEyes, 0f);

            slideDistance -= 4f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator SpriteFade()
    {
        float spriteOpacity = 0.34f;

        while (spriteOpacity <= 1f)
        {
            if (dialoguePortrait != null)
                dialoguePortrait.color = new Color(1f, 1f, 1f, spriteOpacity);

            spriteOpacity += 0.33f;
            yield return new WaitForSeconds(0.05f);
        }

        if (dialoguePortrait != null)
            dialoguePortrait.color = Color.white;
    }

    private IEnumerator EyeAnimate()
    {
        while (dialogueActive)
        {
            SetImageAlpha(dialogueEyes, 0f);
            yield return new WaitForSeconds(Random.Range(2f, 4f));

            SetImageAlpha(dialogueEyes, 1f);

            if (dialogueEyes != null)
                dialogueEyes.sprite = eyeBlink;

            yield return new WaitForSeconds(0.15f);

            SetImageAlpha(dialogueEyes, 0f);
            yield return new WaitForSeconds(Random.Range(2f, 6f));
        }
    }

    private IEnumerator MouthAnimate()
    {
        while (dialogueActive)
        {
            SetImageAlpha(dialogueMouths, 0f);
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));

            SetImageAlpha(dialogueMouths, 1f);

            if (dialogueMouths != null)
                dialogueMouths.sprite = mouth2;

            yield return new WaitForSeconds(0.1f);

            if (dialogueMouths != null)
                dialogueMouths.sprite = mouth3;

            yield return new WaitForSeconds(0.2f);

            if (dialogueMouths != null)
                dialogueMouths.sprite = mouth2;

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        if (image == null)
            return;

        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }

    private void StopFaceCoroutines()
    {
        if (eyeRoutine != null)
        {
            StopCoroutine(eyeRoutine);
            eyeRoutine = null;
        }

        if (mouthRoutine != null)
        {
            StopCoroutine(mouthRoutine);
            mouthRoutine = null;
        }

        if (portraitMoveRoutine != null)
        {
            StopCoroutine(portraitMoveRoutine);
            portraitMoveRoutine = null;
        }

        if (spriteFadeRoutine != null)
        {
            StopCoroutine(spriteFadeRoutine);
            spriteFadeRoutine = null;
        }
    }

    public void EndDialogue()
    {
        StopFaceCoroutines();

        dialogueInfo.Clear();
        dialogueActive = false;

        if (dialogueBox != null)
            dialogueBox.SetActive(false);

        if (SFXManager.instance != null)
            SFXManager.instance.PlayDialogueClose();
    }

    public bool IsDialogueActive()
    {
        return dialogueActive;
    }
}