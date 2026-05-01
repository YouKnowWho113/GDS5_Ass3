using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDishCase", menuName = "FoodDetective/Dish Case")]
public class DishCase : ScriptableObject
{
    [Header("Dish Info")]
    public string dishName;

    [TextArea(2, 5)]
    public string dishDescription;

    [Header("Correct Evidence Answers")]
    public List<EvidenceRequirement> correctEvidence = new List<EvidenceRequirement>();
}

[System.Serializable]
public class EvidenceRequirement
{
    [Tooltip("Use IDs like sweet, salty, spicy, crunchy, dry.")]
    public string evidenceID;

    [Tooltip("Visible = image only, Audible = sound only, Both = visible + audible.")]
    public EvidenceChannel evidenceChannel;
}