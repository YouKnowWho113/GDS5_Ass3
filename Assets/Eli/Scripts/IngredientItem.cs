using UnityEngine;
using UnityEngine.UI;

public class IngredientItem : MonoBehaviour
{
    [Header("Report Answer")]
    public string ingredientID;

    [Header("UI")]
    public Image outlineImage;
    public Button button;

    [Header("Optional Status Icons")]
    public GameObject visibleFoundIcon;
    public GameObject audibleFoundIcon;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        RefreshUI();
    }

    private void OnEnable()
    {
        if (DeductionManager.Instance != null)
            DeductionManager.Instance.OnSelectedEvidenceChanged += RefreshUI;

        if (EvidenceNotebook.Instance != null)
        {
            EvidenceNotebook.Instance.OnEvidenceAdded += HandleEvidenceChanged;
            EvidenceNotebook.Instance.OnEvidenceCleared += RefreshUI;
        }

        RefreshUI();
    }

    private void OnDisable()
    {
        if (DeductionManager.Instance != null)
            DeductionManager.Instance.OnSelectedEvidenceChanged -= RefreshUI;

        if (EvidenceNotebook.Instance != null)
        {
            EvidenceNotebook.Instance.OnEvidenceAdded -= HandleEvidenceChanged;
            EvidenceNotebook.Instance.OnEvidenceCleared -= RefreshUI;
        }
    }

    public void ToggleIngredient()
    {
        if (DeductionManager.Instance == null)
        {
            Debug.LogWarning("[IngredientItem] DeductionManager missing in scene.");
            return;
        }

        DeductionManager.Instance.ToggleEvidence(ingredientID);
        RefreshUI();
    }

    public void SetSelected(bool selected)
    {
        if (DeductionManager.Instance == null)
            return;

        if (selected)
            DeductionManager.Instance.AddEvidence(ingredientID);
        else
            DeductionManager.Instance.RemoveEvidence(ingredientID);

        RefreshUI();
    }

    private void RefreshUI()
    {
        bool isSelected = false;

        if (DeductionManager.Instance != null)
            isSelected = DeductionManager.Instance.IsSelected(ingredientID);

        if (outlineImage != null)
            outlineImage.enabled = isSelected;

        RefreshEvidenceStatus();
    }

    private void RefreshEvidenceStatus()
    {
        EvidenceChannel foundChannel = EvidenceChannel.None;

        if (EvidenceNotebook.Instance != null)
            foundChannel = EvidenceNotebook.Instance.GetDiscoveredChannel(ingredientID);

        if (visibleFoundIcon != null)
            visibleFoundIcon.SetActive((foundChannel & EvidenceChannel.Visible) == EvidenceChannel.Visible);

        if (audibleFoundIcon != null)
            audibleFoundIcon.SetActive((foundChannel & EvidenceChannel.Audible) == EvidenceChannel.Audible);
    }

    private void HandleEvidenceChanged(string evidenceID, EvidenceChannel channel)
    {
        if (evidenceID == ingredientID)
            RefreshUI();
    }
}