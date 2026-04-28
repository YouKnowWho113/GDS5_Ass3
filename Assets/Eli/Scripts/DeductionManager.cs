using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeductionManager : MonoBehaviour
{
    public static DeductionManager Instance;

    [Header("Game Data")]
    public List<ChefProfile> allSuspects;
    public ChefProfile trueCulprit;

    [Header("Game State")]
    public int remainingTries = 3;

    private List<string> selectedIngredients = new List<string>();

    void Awake()
    {
        Instance = this;
    }

    public void AddIngredient(string ingredient)
    {
        if (!selectedIngredients.Contains(ingredient))
            selectedIngredients.Add(ingredient);
    }

    public void RemoveIngredient(string ingredient)
    {
        if (selectedIngredients.Contains(ingredient))
            selectedIngredients.Remove(ingredient);
    }

    public void MakeConclusion()
    {
        if (remainingTries <= 0) return;

        remainingTries--;

        bool isCorrect = trueCulprit.signatureIngredients.Count == selectedIngredients.Count &&
                         trueCulprit.signatureIngredients.All(selectedIngredients.Contains);

        if (isCorrect)
        {
            Debug.Log("CORRECT! The cook is: " + trueCulprit.chefName);
        }
        else
        {
            Debug.Log("INCORRECT! You have " + remainingTries + " tries left.");

            if (remainingTries <= 0)
            {
                Debug.Log("GAME OVER! The real cook is: " + trueCulprit.chefName);
            }
        }
    }
}