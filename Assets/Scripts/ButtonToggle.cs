using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    [Header("Button Components")]
    public Button targetButton;
    public Image buttonImage;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite pressedSprite;

    private bool isPressed = false;

    private void Start()
    {
        
        buttonImage.sprite = normalSprite;

       
        targetButton.onClick.AddListener(ToggleSprite);
    }

    private void ToggleSprite()
    {
        isPressed = !isPressed;

        if (isPressed)
        {
            buttonImage.sprite = pressedSprite;
        }
        else
        {
            buttonImage.sprite = normalSprite;
        }
    }
}