using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour {

    public enum TaskTypeEnum
    {
        Collect,
        Spatial,
        Discomfort
    }

    [SerializeField]
    BaseEnvBuilder builder;

    [SerializeField]
    List<BaseTaskManager> subTaskManagers;

    [SerializeField]
    Transform playGround;
    public static Transform PlayGround
    {
        get { return s_Instance.playGround; }
    }

    public static bool ExistVoiceQuestion = false;

    public static TaskManager s_Instance;

    void Awake()
    {
        if (s_Instance != null) Destroy(gameObject);
        s_Instance = this;
    }

    public void Init(Transform playerController)
    {
        builder.Init(playerController);
        // builder
        for (int i = 0; i < subTaskManagers.Count; i++)
        {
            switch (subTaskManagers[i].TaskType)
            {
                case TaskTypeEnum.Collect:
                    subTaskManagers[i].Init(builder.CollectTaskPosList);
                    break;
                case TaskTypeEnum.Spatial:
                    SpatialTaskManager spManager = subTaskManagers[i] as SpatialTaskManager;
                    spManager.Init(builder.SpatialInfoList);
                    break;
                case TaskTypeEnum.Discomfort:
                    subTaskManagers[i].Init();
                    break;
                default:
                    break;
            }
            
        }
    }

    public void update(float timestep)
    {
        for (int i = 0; i < subTaskManagers.Count; i++) subTaskManagers[i].update(timestep);
    }

    public void End()
    {
        for (int i = 0; i < subTaskManagers.Count; i++) subTaskManagers[i].Clear();
    }


    public void PrintResult()
    {
        for (int i = 0; i < subTaskManagers.Count; i++) subTaskManagers[i].Print();
    }
}
