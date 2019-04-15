using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialTaskManager : BaseTaskManager {

    public static int NumOfTask
    {
        get { return s_Instance.numOfTask; }
    }

    public static SpatialTaskManager s_Instance;

    void Awake()
    {

        if (s_Instance != null) Destroy(gameObject);
        s_Instance = this;
    }
    public override void Init(List<Vector3> positionList)
    {
        for (int i = 0; i < positionList.Count; i++)
        {
            GameObject gm = Instantiate(taskPrefab, positionList[i], Quaternion.identity);

            TaskList.Add(gm.GetComponent<SpatialTask>());
            TaskList[i].TaskIndex = i;
        }

        base.Init();
    }
    public override void update()
    {

    }
    public override void Clear()
    {
        base.Clear();
    }
}
