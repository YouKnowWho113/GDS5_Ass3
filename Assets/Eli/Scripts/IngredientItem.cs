using UnityEngine;
using UnityEngine.UI;

public class IngredientItem : MonoBehaviour
{
    public string ingredientID;
    public Image outlineImage;

    private bool isSelected = false;

    public void ToggleIngredient()
    {
        isSelected = !isSelected;
        outlineImage.enabled = isSelected;

        if (isSelected)
        {
            DeductionManager.Instance.AddIngredient(ingredientID);
        }
        else
        {
            DeductionManager.Instance.RemoveIngredient(ingredientID);
        }
    }
}