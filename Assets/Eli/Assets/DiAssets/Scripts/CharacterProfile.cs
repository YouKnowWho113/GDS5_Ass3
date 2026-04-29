using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Profile", menuName = "Character Profile")]
public class CharacterProfile : ScriptableObject
{
    public string myName;

    private Sprite basePortrait;
    private Sprite eyeEmoSpriteBlink;
    private Sprite mouthEmoSprite2;
    private Sprite mouthEmoSprite3;

    public float xBase;
    public float yBase;
    public int xFace;
    public int yFace;
    private string myEmotion;


    public bool shadowStatus;

    public Sprite EmotionBaseDisplay
    {
        get
        {
            SetEmotionType(Emotion);
            return basePortrait;
        }
    }

    public Sprite EmotionBlinkDisplay
    {
        get
        {
            SetEmotionType(Emotion);
            return eyeEmoSpriteBlink;
        }
    }

    public Sprite EmotionMouth2Display
    {
        get
        {
            SetEmotionType(Emotion);
            return mouthEmoSprite2;
        }
    }
    public Sprite EmotionMouth3Display
    {
        get
        {
            SetEmotionType(Emotion);
            return mouthEmoSprite3;
        }
    }

    [System.Serializable]
    public class EmotionBase
    {
        public Sprite neutralBase;
        public Sprite happyBase;
        public Sprite angeryBase;
        public Sprite sadBase;
        public Sprite surprisedBase;
        public Sprite hurtBase;
        public Sprite blushBase;
        public Sprite custom1Base;
        public Sprite custom2Base;
        public Sprite custom3Base;
    }

    [System.Serializable]
    public class EmotionBlink
    {
        public Sprite neutralBlink;
        public Sprite happyBlink;
        public Sprite angeryBlink;
        public Sprite sadBlink;
        public Sprite surprisedBlink;
        public Sprite hurtBlink;
        public Sprite blushBlink;
        public Sprite custom1Blink;
        public Sprite custom2Blink;
        public Sprite custom3Blink;
    }

    [System.Serializable]
    public class EmotionMouth2
    {
        public Sprite neutralMouth2;
        public Sprite happyMouth2;
        public Sprite angeryMouth2;
        public Sprite sadMouth2;
        public Sprite surprisedMouth2;
        public Sprite hurtMouth2;
        public Sprite blushMouth2;
        public Sprite custom1Mouth2;
        public Sprite custom2Mouth2;
        public Sprite custom3Mouth2;
    }

    [System.Serializable]
    public class EmotionMouth3
    {
        public Sprite neutralMouth3;
        public Sprite happyMouth3;
        public Sprite angeryMouth3;
        public Sprite sadMouth3;
        public Sprite surprisedMouth3;
        public Sprite hurtMouth3;
        public Sprite blushMouth3;
        public Sprite custom1Mouth3;
        public Sprite custom2Mouth3;
        public Sprite custom3Mouth3;
    }

    public EmotionBase emotionBase;
    public EmotionBlink emotionBlink;
    public EmotionMouth2 emotionMouth2;
    public EmotionMouth3 emotionMouth3;

    public EmotionType Emotion { get; set; }

    public void SetEmotionType(EmotionType newEmotion)
    {
        Emotion = newEmotion;
        switch (Emotion)
        {
            case EmotionType.Neutral:
                basePortrait = emotionBase.neutralBase;
                eyeEmoSpriteBlink = emotionBlink.neutralBlink;
                mouthEmoSprite2 = emotionMouth2.neutralMouth2;
                mouthEmoSprite3 = emotionMouth3.neutralMouth3;
                break;
            case EmotionType.Happy:
                basePortrait = emotionBase.happyBase;
                eyeEmoSpriteBlink = emotionBlink.happyBlink;
                mouthEmoSprite2 = emotionMouth2.happyMouth2;
                mouthEmoSprite3 = emotionMouth3.happyMouth3;
                break;
            case EmotionType.Angery:
                basePortrait = emotionBase.angeryBase;
                eyeEmoSpriteBlink = emotionBlink.angeryBlink;
                mouthEmoSprite2 = emotionMouth2.angeryMouth2;
                mouthEmoSprite3 = emotionMouth3.angeryMouth3;
                break;
            case EmotionType.Sad:
                basePortrait = emotionBase.sadBase;
                eyeEmoSpriteBlink = emotionBlink.sadBlink;
                mouthEmoSprite2 = emotionMouth2.sadMouth2;
                mouthEmoSprite3 = emotionMouth3.sadMouth3;
                break;
            case EmotionType.Surprised:
                basePortrait = emotionBase.surprisedBase;
                eyeEmoSpriteBlink = emotionBlink.surprisedBlink;
                mouthEmoSprite2 = emotionMouth2.surprisedMouth2;
                mouthEmoSprite3 = emotionMouth3.surprisedMouth3;
                break;
            case EmotionType.Hurt:
                basePortrait = emotionBase.hurtBase;
                eyeEmoSpriteBlink = emotionBlink.hurtBlink;
                mouthEmoSprite2 = emotionMouth2.hurtMouth2;
                mouthEmoSprite3 = emotionMouth3.hurtMouth3;
                break;
            case EmotionType.Blush:
                basePortrait = emotionBase.blushBase;
                eyeEmoSpriteBlink = emotionBlink.blushBlink;
                mouthEmoSprite2 = emotionMouth2.blushMouth2;
                mouthEmoSprite3 = emotionMouth3.blushMouth3;
                break;
            case EmotionType.Custom1:
                basePortrait = emotionBase.custom1Base;
                eyeEmoSpriteBlink = emotionBlink.custom1Blink;
                mouthEmoSprite2 = emotionMouth2.custom1Mouth2;
                mouthEmoSprite3 = emotionMouth3.custom1Mouth3;
                break;
            case EmotionType.Custom2:
                basePortrait = emotionBase.custom2Base;
                eyeEmoSpriteBlink = emotionBlink.custom2Blink;
                mouthEmoSprite2 = emotionMouth2.custom2Mouth2;
                mouthEmoSprite3 = emotionMouth3.custom2Mouth3;
                break;
            case EmotionType.Custom3:
                basePortrait = emotionBase.custom3Base;
                eyeEmoSpriteBlink = emotionBlink.custom3Blink;
                mouthEmoSprite2 = emotionMouth2.custom3Mouth2;
                mouthEmoSprite3 = emotionMouth3.custom3Mouth3;
                break;
        }
    }
}

    public enum EmotionType
    {
        Neutral,
        Happy,
        Angery,
        Sad,
        Surprised,
        Hurt,
        Blush,
        Custom1,
        Custom2,
        Custom3,
    }
