using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueButton : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            GetNextLine();
        }
    }
  public void GetNextLine()
    {
        DialogueManager.instance.DequeueDialogue();
        DialogueManager.instance.textBoxAnim();
    }
}
