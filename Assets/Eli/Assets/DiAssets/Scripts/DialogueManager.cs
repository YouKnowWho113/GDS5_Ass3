using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("fix this" + gameObject.name);
        }
        else
        {
            instance = this;
        }
    }

    public Animator textBoxAnimator;
    public Animator nameTagAnimator;
    public Animator boxTextAnimator;
    public Animator nameTextAnimator;
    public Animator bumperAnimator;
    public void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            StopAllCoroutines();
            if (dialogueInfo.Count > 0)
            {
                SFXManager.instance?.PlayDialogueNext();
            }
            DequeueDialogue();
            textBoxAnim();
        }
            
    }

    private bool continueCheck;
    public void textBoxAnim()
    {
        if(shadowCheck == true)
          {
            if (continueCheck == true)
            {
                textBoxAnimator.Play("YBoxContinue");
                nameTagAnimator.Play("yelTagContinue");
                boxTextAnimator.Play("textBoxEntry", 0, 0f);
                nameTextAnimator.Play("nameTextEntry", 0, 0f);
                bumperAnimator.Play("bumperEntry", 0, 0f);
            }
            else if (continueCheck == false)
            {
                textBoxAnimator.Play("YBoxEntry");
                nameTagAnimator.Play("yelTagEntry");
                boxTextAnimator.Play("textBoxEntry", 0, 0f);
                nameTextAnimator.Play("nameTextEntry", 0, 0f);
                bumperAnimator.Play("bumperEntry", 0, 0f);
            }
         }
            else if(shadowCheck == false)
            {
            if (continueCheck == true)
            {
                textBoxAnimator.Play("RBoxContinue");
                nameTagAnimator.Play("redTagContinue");
                boxTextAnimator.Play("textBoxEntry", 0, 0f);
                nameTextAnimator.Play("nameTextEntry", 0, 0f);
                bumperAnimator.Play("bumperEntry", 0, 0f);
            }
            else if (continueCheck == false)
            {
                textBoxAnimator.Play("RBoxENtry");
                nameTagAnimator.Play("redTagEntry");
                boxTextAnimator.Play("textBoxEntry", 0, 0f);
                nameTextAnimator.Play("nameTextEntry", 0, 0f);
                bumperAnimator.Play("bumperEntry", 0, 0f);
            }
        }
    }

    public GameObject dialogueBox;

    public TextMeshProUGUI dialogueName;
    public TextMeshProUGUI dialogueText;
    public Image dialoguePortrait;
    public Image dialogueEyes;
    public Image dialogueMouths;
    public Image dialogueTextBox;

    private Sprite eyeBlink;
    private Sprite mouth2;
    private Sprite mouth3;
    private int xEyes;
    private int yEyes;
    private float xSprite;
    private float ySprite;
    private bool shadowCheck;
    


    public Queue<DialogueBase.Info> dialogueInfo = new Queue<DialogueBase.Info>();

    public void EnqueueDialogue(DialogueBase db)
    {
        dialogueBox.SetActive(true);
        dialogueInfo.Clear();

        foreach (DialogueBase.Info info in db.dialogueInfo)
        {
            dialogueInfo.Enqueue(info);
        }
        SFXManager.instance?.PlayDialogueOpen();
        DequeueDialogue();
        textBoxAnim();
    }

    public void DequeueDialogue()
    {
        if(dialogueInfo.Count == 0)
        {
            StopAllCoroutines();
            EndDialogue();
            return;
        }
        DialogueBase.Info info = dialogueInfo.Dequeue();

        dialogueName.text = info.character.myName;
        dialogueText.text = info.charText;
        info.ChangeEmotion();
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

        if (continueCheck == false)
        {
            StartCoroutine("PortraitMove");
            StartCoroutine("SpriteFade");
        }
        StartCoroutine("EyeAnimate");
        StartCoroutine("MouthAnimate");
    }

    private float slideDistance;
    private float spriteOpacity;
    private float faceOpacity;

    public IEnumerator PortraitMove()
    {
        slideDistance = 12;
        while (slideDistance >= 0)
            {
            dialoguePortrait.rectTransform.anchoredPosition = new Vector3(xSprite - slideDistance, ySprite, 0);

            dialogueEyes.rectTransform.anchoredPosition = new Vector3(xEyes - slideDistance, yEyes, 0);
            dialogueMouths.rectTransform.anchoredPosition = new Vector3(xEyes - slideDistance, yEyes, 0);

            slideDistance += -4;
            yield return new WaitForSeconds(.05f);
        }
    }

    public IEnumerator SpriteFade()
    {
        spriteOpacity = 0.34f;
        while (spriteOpacity <= 1f)
        {
            dialoguePortrait.color = new Color(1f, 1f, 1f, spriteOpacity);
            spriteOpacity += 0.33f;
            yield return new WaitForSeconds(.05f);
        }
    }

    private bool animationBool;

    public IEnumerator EyeAnimate()
    {
        while (true)
        {
            dialogueEyes.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(Random.Range(2f, 4.0f));

            dialogueEyes.color = new Color(1f, 1f, 1f, 1f);
            dialogueEyes.sprite = eyeBlink;
            yield return new WaitForSeconds(0.15f);

            dialogueEyes.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(Random.Range(2f, 6.0f));

            dialogueEyes.color = new Color(1f, 1f, 1f, 1f);
            dialogueEyes.sprite = eyeBlink;
            yield return new WaitForSeconds(0.15f);

            dialogueEyes.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(0.15f);

            dialogueEyes.color = new Color(1f, 1f, 1f, 1f);
            dialogueEyes.sprite = eyeBlink;
            yield return new WaitForSeconds(0.15f);

            dialogueEyes.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(Random.Range(3f, 4.0f));
        }
    }

    public IEnumerator MouthAnimate()
    {
        while(true)
        {
            dialogueMouths.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));

            dialogueMouths.color = new Color(1f, 1f, 1f, 1f);
            dialogueMouths.sprite = mouth2;
            yield return new WaitForSeconds(0.1f);

            dialogueMouths.sprite = mouth3;
            yield return new WaitForSeconds(0.2f);

            dialogueMouths.sprite = mouth2;
            yield return new WaitForSeconds(0.1f);

            dialogueMouths.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));

            dialogueMouths.color = new Color(1f, 1f, 1f, 1f);
            dialogueMouths.sprite = mouth2;
            yield return new WaitForSeconds(0.1f);

            dialogueMouths.sprite = mouth3;
            yield return new WaitForSeconds(0.2f);

            dialogueMouths.sprite = mouth2;
            yield return new WaitForSeconds(0.1f);

            dialogueMouths.sprite = mouth3;
            yield return new WaitForSeconds(0.2f);

            dialogueMouths.sprite = mouth2;
            yield return new WaitForSeconds(0.1f);

            dialogueMouths.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(.05f);
        }
    }
       
    public void EndDialogue()
    {
        SFXManager.instance?.PlayDialogueClose();
        dialogueBox.SetActive(false);
    }
}
