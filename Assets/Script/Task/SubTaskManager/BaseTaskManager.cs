using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class BaseTaskManager : MonoBehaviour {

    [SerializeField]
    protected int numOfTask = 0;
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

    public virtual void update(float timestep)
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

    public virtual void OpenRecord(BaseTask task)
    {
        
    }

    public virtual void CloseRecord(int taskIndex)
    {
        
    }

    public void Print()
    {
        string dateformat = "yyyyMMdd-HHmm";
        string filename = DateTime.Now.ToString(dateformat);

        string header = "";

        if (TaskType == TaskManager.TaskTypeEnum.Collect) header = CollectRecord.RecordHeader;
        else if (TaskType == TaskManager.TaskTypeEnum.Discomfort) header = DiscomfortRecord.RecordHeader;
        else if (TaskType == TaskManager.TaskTypeEnum.Spatial) header = SpatialRecord.RecordHeader;

        string content = "";
        for (int i = 0; i < RecordList.Count; i++)
        {
            content += (RecordList[i].ToString() + "\n");
        }

        string path = Application.streamingAssetsPath + "/" + filename + "_DetailRecords_" + TaskType.ToString() + ".csv";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(header + "\n" + content);
        writer.Close();
    }
}
