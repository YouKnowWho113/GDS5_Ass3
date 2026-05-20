using System.Collections.Generic;
using UnityEngine;

public class NoteBookEvidenceUI : MonoBehaviour
{
    [System.Serializable]
    public class EvidenceUI
    {
        public string evidenceID;
        public GameObject targetObject;
    }

    [Header("Global Evidence UI")]
    public Transform evidenceRoot;
    public EvidenceUI[] evidenceObjects;

    [Header("Options")]
    public bool autoBuildFromChildren = true;
    public bool includeInactiveChildren = true;
    public bool hideAllOnStart = true;

    private Dictionary<string, GameObject> evidenceMap =
        new Dictionary<string, GameObject>();

    private void Awake()
    {
        BuildMap();

        if (hideAllOnStart)
            HideAllEvidence();
    }

    private void OnEnable()
    {
        if (EvidenceNotebook.Instance != null)
            EvidenceNotebook.Instance.OnEvidenceAdded += HandleEvidenceAdded;

        RefreshEvidence();
    }

    private void OnDisable()
    {
        if (EvidenceNotebook.Instance != null)
            EvidenceNotebook.Instance.OnEvidenceAdded -= HandleEvidenceAdded;
    }

    public void RefreshEvidence()
    {
        BuildMap();

        if (LevelCompCheck.Instance == null)
        {
            HideAllEvidence();
            return;
        }

        foreach (var pair in evidenceMap)
        {
            string evidenceID = pair.Key;
            GameObject targetObject = pair.Value;

            bool unlocked = LevelCompCheck.Instance.HasUnlockedEvidence(evidenceID);

            targetObject.SetActive(unlocked);
        }
    }

    private void HandleEvidenceAdded(string evidenceID, EvidenceChannel channel)
    {
        RefreshEvidence();
    }

    private void BuildMap()
    {
        evidenceMap.Clear();

        if (autoBuildFromChildren && evidenceRoot != null)
        {
            foreach (Transform child in evidenceRoot)
            {
                string id = NormalizeID(child.name);

                if (!evidenceMap.ContainsKey(id))
                    evidenceMap.Add(id, child.gameObject);
            }
        }

        if (evidenceObjects != null)
        {
            foreach (EvidenceUI obj in evidenceObjects)
            {
                if (obj == null || obj.targetObject == null || string.IsNullOrWhiteSpace(obj.evidenceID))
                    continue;

                string id = NormalizeID(obj.evidenceID);
                evidenceMap[id] = obj.targetObject;
            }
        }
    }

    private void HideAllEvidence()
    {
        BuildMap();

        foreach (var pair in evidenceMap)
        {
            if (pair.Value != null)
                pair.Value.SetActive(false);
        }
    }

    private string NormalizeID(string raw)
    {
        return raw.Trim().ToLower().Replace(" ", "");
    }
}