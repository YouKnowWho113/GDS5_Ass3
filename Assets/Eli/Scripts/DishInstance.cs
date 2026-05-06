using UnityEngine;

public class DishInstance : MonoBehaviour
{
    [Header("Dish Data")]
    public DishCase dishCase;

    [Header("Register Settings")]
    public bool autoRegisterOnStart = true;
    public bool clearEvidenceOnRegister = true;
    public bool clearReportOnRegister = true;
    public bool resetTriesOnRegister = true;
    public int startingTries = 3;

    private void Start()
    {
        if (autoRegisterOnStart)
            RegisterDish();
    }

    public void RegisterDish()
    {
        if (dishCase == null)
        {
            Debug.LogWarning("[DishInstance] No DishCase assigned on " + gameObject.name);
            return;
        }

        if (EvidenceNotebook.Instance != null && clearEvidenceOnRegister)
            EvidenceNotebook.Instance.ClearEvidence();

        if (DeductionManager.Instance != null)
        {
            DeductionManager.Instance.SetCurrentDish(dishCase);

            if (clearReportOnRegister)
                DeductionManager.Instance.ResetReport();

            if (resetTriesOnRegister)
                DeductionManager.Instance.ResetTries(startingTries);
        }
        else
        {
            Debug.LogWarning("[DishInstance] DeductionManager missing in scene.");
        }

        Debug.Log("[DishInstance] Registered dish: " + dishCase.dishName);
    }
}