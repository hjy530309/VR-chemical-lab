using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetExperiment : MonoBehaviour
{
    public ChecklistManager checklist;

    public void experimentReset(){
        StartCoroutine(checklist.ResetChecklist());
    }
}
