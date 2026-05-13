using System.Collections;
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

    public EvidenceUI[] evidenceObjects;

    public void RefreshEvidence()
    {
        if (EvidenceNotebook.Instance == null)
            return;

        foreach (EvidenceUI obj in evidenceObjects)
        {
            if (obj.targetObject == null)
                continue;

            bool unlocked =
                EvidenceNotebook.Instance.HasAnyEvidence(obj.evidenceID);

            Debug.Log(obj.evidenceID + " = " + unlocked);

            obj.targetObject.SetActive(unlocked);
        }
    }
}