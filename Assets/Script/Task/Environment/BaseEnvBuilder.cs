using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnvBuilder : MonoBehaviour {

    public int NumOfCollectTargets;

    public List<Vector3> CollectTaskPosList;
    public List<Vector3> SpatialTaskPosList;

    public virtual void Init(Transform playerController)
    {
        return;
    }

}
