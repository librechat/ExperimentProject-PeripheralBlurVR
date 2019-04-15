using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTaskManager : MonoBehaviour {

    [SerializeField]
    protected int numOfTask;
    [SerializeField]
    protected GameObject taskPrefab;

    
    public TaskManager.TaskTypeEnum TaskType;

    protected List<BaseTask> TaskList;
    protected List<Record> RecordList;
    protected int currentTaskIndex = -1;

    public virtual void Init()
    {

    }

    public virtual void Init(List<Vector3> posList)
    {
        //TaskList = new List<BaseTask>();
        //RecordList = new List<Record>();
    }

    public virtual void update()
    {

    }

    public virtual void Clear()
    {
        if (TaskList != null && TaskList.Count != 0)
        {
            for (int i = 0; i < TaskList.Count; i++)
            {
                Destroy(TaskList[i].gameObject);
            }
            TaskList.Clear();
        }
    }

    public virtual void OpenNewRecord(BaseTask task)
    {
        
    }

    public virtual void CloseRecord()
    {
        
    }
}
