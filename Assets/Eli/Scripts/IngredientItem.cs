using UnityEngine;
using UnityEngine.UI;

public class IngredientItem : MonoBehaviour
{
    [Header("Report Answer")]
    public string ingredientID;

    [Header("UI")]
    public Image outlineImage;

    private bool isSelected;

    private void Awake()
    {
        if (outlineImage != null)
            outlineImage.enabled = false;
    }

    public void ToggleIngredient()
    {
        isSelected = !isSelected;

        if (outlineImage != null)
            outlineImage.enabled = isSelected;

        if (DeductionManager.Instance == null)
        {
            Debug.LogWarning("[IngredientItem] DeductionManager missing in scene.");
            return;
        }

        if (isSelected)
        {
            DeductionManager.Instance.AddIngredient(ingredientID);
        }
        else
        {
            DeductionManager.Instance.RemoveIngredient(ingredientID);
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (outlineImage != null)
            outlineImage.enabled = isSelected;
    }
}