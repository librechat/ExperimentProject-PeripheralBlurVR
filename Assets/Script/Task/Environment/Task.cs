using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour {

    [SerializeField]
    protected GameObject collectTargetPrefab;

    [SerializeField]
    protected GameObject spatialTargetPrefab;

    public int NumOfCollectTargets;

    public int NumOfSpatialTargets;

    public virtual List<CollectTarget> Init(Transform playerController)
    {
        return new List<CollectTarget>();
    }

}
