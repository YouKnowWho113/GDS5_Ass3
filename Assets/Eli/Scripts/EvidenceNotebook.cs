using System.Collections.Generic;
using UnityEngine;

public class EvidenceNotebook : MonoBehaviour
{
    public static EvidenceNotebook Instance;

    private HashSet<string> visibleEvidence = new HashSet<string>();
    private HashSet<string> audibleEvidence = new HashSet<string>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddVisibleEvidence(string evidenceID)
    {
        if (visibleEvidence.Add(evidenceID))
            Debug.Log("[Evidence] Visible found: " + evidenceID);
    }

    public void AddAudibleEvidence(string evidenceID)
    {
        if (audibleEvidence.Add(evidenceID))
            Debug.Log("[Evidence] Audible found: " + evidenceID);
    }

    public bool HasVisibleEvidence(string evidenceID)
    {
        return visibleEvidence.Contains(evidenceID);
    }

    public bool HasAudibleEvidence(string evidenceID)
    {
        return audibleEvidence.Contains(evidenceID);
    }

    public bool HasEvidence(string evidenceID, EvidenceChannel channel)
    {
        switch (channel)
        {
            case EvidenceChannel.Visible:
                return HasVisibleEvidence(evidenceID);

            case EvidenceChannel.Audible:
                return HasAudibleEvidence(evidenceID);

            case EvidenceChannel.Both:
                return HasVisibleEvidence(evidenceID) && HasAudibleEvidence(evidenceID);

            default:
                return false;
        }
    }
}