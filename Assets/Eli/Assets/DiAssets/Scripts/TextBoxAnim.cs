using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxAnim : MonoBehaviour
{
    public Animator textBoxAnimator;
    public Animator nameTagAnimator;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            textBoxAnimator.Play("RBoxENtry");
            nameTagAnimator.Play("redTagEntry");
            
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            textBoxAnimator.Play("RBoxContinue");
            nameTagAnimator.Play("redTagContinue");
        }
    }

}
