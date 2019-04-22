using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnvBuilder : MonoBehaviour {

    public class SpatialTaskData
    {
        public Vector3 startPos;
        public Vector3 endPos;
    }

    public List<Vector3> CollectTaskPosList;
    public List<SpatialTaskData> SpatialInfoList;

    public virtual void Init(Transform playerController)
    {
        return;
    }

}
