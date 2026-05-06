using System;
using System.Collections.Generic;
using UnityEngine;

public class DeductionManager : MonoBehaviour
{
    public static DeductionManager Instance;

    [Header("Current Dish")]
    public DishCase currentDish;

    [Header("Game State")]
    public int remainingTries = 3;
    public int maxSelectedEvidence = 3;

    [Header("Rules")]
    public bool requireEvidenceBeforeCorrect = true;

    public event Action OnSelectedEvidenceChanged;
    public event Action<bool> OnReportSubmitted;

    private readonly HashSet<string> selectedEvidence = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetCurrentDish(DishCase dishCase)
    {
        currentDish = dishCase;
        ResetReport();

        Debug.Log("[DeductionManager] Current dish set to: " +
                  (currentDish != null ? currentDish.dishName : "None"));
    }

    public bool AddIngredient(string evidenceID)
    {
        return AddEvidence(evidenceID);
    }

    public bool RemoveIngredient(string evidenceID)
    {
        return RemoveEvidence(evidenceID);
    }

    public bool AddEvidence(string evidenceID)
    {
        if (string.IsNullOrWhiteSpace(evidenceID))
            return false;

        if (selectedEvidence.Contains(evidenceID))
            return true;

        if (selectedEvidence.Count >= maxSelectedEvidence)
        {
            Debug.Log("[Report] Cannot select more than " + maxSelectedEvidence + " evidence.");
            return false;
        }

        selectedEvidence.Add(evidenceID);
        Debug.Log("[Report] Selected evidence: " + evidenceID);

        OnSelectedEvidenceChanged?.Invoke();
        return true;
    }

    public bool RemoveEvidence(string evidenceID)
    {
        if (string.IsNullOrWhiteSpace(evidenceID))
            return false;

        bool removed = selectedEvidence.Remove(evidenceID);

        if (removed)
        {
            Debug.Log("[Report] Removed evidence: " + evidenceID);
            OnSelectedEvidenceChanged?.Invoke();
        }

        return removed;
    }

    public bool ToggleEvidence(string evidenceID)
    {
        if (IsSelected(evidenceID))
            return RemoveEvidence(evidenceID);

        return AddEvidence(evidenceID);
    }

    public bool IsSelected(string evidenceID)
    {
        return selectedEvidence.Contains(evidenceID);
    }

    public bool CanSelectMore()
    {
        return selectedEvidence.Count < maxSelectedEvidence;
    }

    public int GetSelectedCount()
    {
        return selectedEvidence.Count;
    }

    public List<string> GetSelectedEvidence()
    {
        return new List<string>(selectedEvidence);
    }

    public void ResetReport()
    {
        selectedEvidence.Clear();
        OnSelectedEvidenceChanged?.Invoke();

        Debug.Log("[Report] Report selections cleared.");
    }

    public void ResetTries(int tries)
    {
        remainingTries = tries;
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

        if (requireEvidenceBeforeCorrect)
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

        OnReportSubmitted?.Invoke(reportCorrect);
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
            if (!DishContainsEvidence(selected))
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

                EvidenceChannel foundChannel =
                    EvidenceNotebook.Instance.GetDiscoveredChannel(requirement.evidenceID);

                Debug.Log(
                    "[Report] Missing required evidence for: " +
                    requirement.evidenceID +
                    " | Required: " +
                    requirement.evidenceChannel +
                    " | Found: " +
                    foundChannel
                );
            }
        }
    }

    private bool DishContainsEvidence(string evidenceID)
    {
        foreach (EvidenceRequirement requirement in currentDish.correctEvidence)
        {
            if (requirement.evidenceID == evidenceID)
                return true;
        }

        return false;
    }
}