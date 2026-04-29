using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class DialogueBase : ScriptableObject
{
    [System.Serializable]
    public class Info
    {
        public CharacterProfile character;
        [TextArea(4, 8)]
        public string charText;

        public EmotionType characterEmotion;

        public bool continueCheck;
        
        public void ChangeEmotion()
        {
            character.Emotion = characterEmotion;
        }

    }

    public Info[] dialogueInfo;
}
