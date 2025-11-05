using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetExperiment : MonoBehaviour
{
    public ChecklistManager checklist;

    public void experimentReset(){
        ExperimentManager expMgr = FindObjectOfType<ExperimentManager>();
        if (expMgr != null)
            expMgr.OnResetButtonPressed();
        StartCoroutine(checklist.ResetChecklist());
    }
}
