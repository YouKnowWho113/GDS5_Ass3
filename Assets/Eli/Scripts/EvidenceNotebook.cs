using System;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceNotebook : MonoBehaviour
{
    public static EvidenceNotebook Instance;

    public event Action<string, EvidenceChannel> OnEvidenceAdded;
    public event Action OnEvidenceCleared;

    private readonly Dictionary<string, EvidenceChannel> discoveredEvidence =
        new Dictionary<string, EvidenceChannel>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddVisibleEvidence(string evidenceID)
    {
        AddEvidence(evidenceID, EvidenceChannel.Visible);
    }

    public void AddAudibleEvidence(string evidenceID)
    {
        AddEvidence(evidenceID, EvidenceChannel.Audible);
    }

    public void AddEvidence(string evidenceID, EvidenceChannel channel)
    {
        if (string.IsNullOrWhiteSpace(evidenceID))
            return;

        EvidenceChannel oldChannel = EvidenceChannel.None;

        if (discoveredEvidence.ContainsKey(evidenceID))
            oldChannel = discoveredEvidence[evidenceID];

        EvidenceChannel newChannel = oldChannel | channel;

        if (newChannel == oldChannel)
            return;

        discoveredEvidence[evidenceID] = newChannel;

        Debug.Log("[EvidenceNotebook] Found " + channel + " evidence: " + evidenceID);

        OnEvidenceAdded?.Invoke(evidenceID, newChannel);
    }

    public bool HasVisibleEvidence(string evidenceID)
    {
        return HasEvidence(evidenceID, EvidenceChannel.Visible);
    }

    public bool HasAudibleEvidence(string evidenceID)
    {
        return HasEvidence(evidenceID, EvidenceChannel.Audible);
    }

    public bool HasEvidence(string evidenceID, EvidenceChannel requiredChannel)
    {
        if (string.IsNullOrWhiteSpace(evidenceID))
            return false;

        if (requiredChannel == EvidenceChannel.None)
            return true;

        if (!discoveredEvidence.TryGetValue(evidenceID, out EvidenceChannel foundChannel))
            return false;

        return (foundChannel & requiredChannel) == requiredChannel;
    }

    public EvidenceChannel GetDiscoveredChannel(string evidenceID)
    {
        if (string.IsNullOrWhiteSpace(evidenceID))
            return EvidenceChannel.None;

        if (discoveredEvidence.TryGetValue(evidenceID, out EvidenceChannel channel))
            return channel;

        return EvidenceChannel.None;
    }

    public bool HasAnyEvidence(string evidenceID)
    {
        return GetDiscoveredChannel(evidenceID) != EvidenceChannel.None;
    }

    public Dictionary<string, EvidenceChannel> GetAllDiscoveredEvidence()
    {
        return new Dictionary<string, EvidenceChannel>(discoveredEvidence);
    }

    public void ClearEvidence()
    {
        discoveredEvidence.Clear();
        Debug.Log("[EvidenceNotebook] Cleared all evidence.");

        OnEvidenceCleared?.Invoke();
    }
}