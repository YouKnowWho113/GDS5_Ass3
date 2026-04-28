using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewChef", menuName = "FoodDetective/Chef Profile")]
public class ChefProfile : ScriptableObject
{
    public string chefName;
    public Sprite chefPortrait;
    public List<string> signatureIngredients;

    [TextArea(3, 5)]
    public string flavorText;
}