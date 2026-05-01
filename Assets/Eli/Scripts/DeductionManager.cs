using System.Collections.Generic;
using UnityEngine;

public class DeductionManager : MonoBehaviour
{
    public static DeductionManager Instance;

    [Header("Current Dish")]
    public DishCase currentDish;

    [Header("Game State")]
    public int remainingTries = 3;

    private readonly List<string> selectedEvidence = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddIngredient(string evidenceID)
    {
        AddEvidence(evidenceID);
    }

    public void RemoveIngredient(string evidenceID)
    {
        RemoveEvidence(evidenceID);
    }

    public void AddEvidence(string evidenceID)
    {
        if (string.IsNullOrWhiteSpace(evidenceID))
            return;

        if (!selectedEvidence.Contains(evidenceID))
        {
            selectedEvidence.Add(evidenceID);
            Debug.Log("[Report] Added evidence answer: " + evidenceID);
        }
    }

    public void RemoveEvidence(string evidenceID)
    {
        if (selectedEvidence.Contains(evidenceID))
        {
            selectedEvidence.Remove(evidenceID);
            Debug.Log("[Report] Removed evidence answer: " + evidenceID);
        }
    }

    public void MakeConclusion()
    {
        if (currentDish == null)
        {
            Debug.LogWarning("[DeductionManager] No DishCase assigned.");
            return;
        }

        if (EvidenceNotebook.Instance == null)
        {
            Debug.LogWarning("[DeductionManager] EvidenceNotebook missing in scene.");
            return;
        }

        if (remainingTries <= 0)
        {
            Debug.Log("[Report] No tries left.");
            return;
        }

        remainingTries--;

        bool reportCorrect = true;

        CheckMissingAnswers(ref reportCorrect);
        CheckWrongAnswers(ref reportCorrect);
        CheckEvidenceChannels(ref reportCorrect);

        if (reportCorrect)
        {
            Debug.Log("[Report] CORRECT REPORT: " + currentDish.dishName);
        }
        else
        {
            Debug.Log("[Report] INCORRECT REPORT. Tries left: " + remainingTries);

            if (remainingTries <= 0)
            {
                Debug.Log("[Report] GAME OVER. Correct dish was: " + currentDish.dishName);
            }
        }
    }

    private void CheckMissingAnswers(ref bool reportCorrect)
    {
        foreach (EvidenceRequirement requirement in currentDish.correctEvidence)
        {
            if (!selectedEvidence.Contains(requirement.evidenceID))
            {
                reportCorrect = false;
                Debug.Log("[Report] Missing answer: " + requirement.evidenceID);
            }
        }
    }

    private void CheckWrongAnswers(ref bool reportCorrect)
    {
        foreach (string selected in selectedEvidence)
        {
            bool existsInDish = false;

            foreach (EvidenceRequirement requirement in currentDish.correctEvidence)
            {
                if (requirement.evidenceID == selected)
                {
                    existsInDish = true;
                    break;
                }
            }

            if (!existsInDish)
            {
                reportCorrect = false;
                Debug.Log("[Report] Wrong answer selected: " + selected);
            }
        }
    }

    private void CheckEvidenceChannels(ref bool reportCorrect)
    {
        foreach (EvidenceRequirement requirement in currentDish.correctEvidence)
        {
            if (!selectedEvidence.Contains(requirement.evidenceID))
                continue;

            bool hasRequiredEvidence = EvidenceNotebook.Instance.HasEvidence(
                requirement.evidenceID,
                requirement.evidenceChannel
            );

            if (!hasRequiredEvidence)
            {
                reportCorrect = false;
                Debug.Log(
                    "[Report] Missing required " +
                    requirement.evidenceChannel +
                    " evidence for: " +
                    requirement.evidenceID
                );
            }
        }
    }

    public void ClearSelectedEvidence()
    {
        selectedEvidence.Clear();
        Debug.Log("[Report] Cleared selected report answers.");
    }

    public List<string> GetSelectedEvidence()
    {
        return new List<string>(selectedEvidence);
    }
}