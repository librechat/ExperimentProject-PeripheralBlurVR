using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour {

    [SerializeField]
    protected GameObject targetPrefab;

    public int NumOfTargets;

    public virtual List<TaskTarget> Init(Transform playerController)
    {
        return new List<TaskTarget>();
    }

}
