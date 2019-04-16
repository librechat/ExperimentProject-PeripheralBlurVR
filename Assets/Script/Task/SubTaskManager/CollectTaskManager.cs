using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class CollectTaskManager : BaseTaskManager {

    public static int NumOfTask
    {
        get { return s_Instance.numOfTask; }
    }

    public static CollectTaskManager s_Instance;

    void Awake()
    {

        if (s_Instance != null) Destroy(gameObject);
        s_Instance = this;
    }

    public override void Init(List<Vector3> positionList)
    {
        TaskList = new List<BaseTask>();
        RecordList = new List<Record>();

        for (int i=0; i< positionList.Count; i++)
        {
            GameObject gm = Instantiate(taskPrefab, positionList[i], Quaternion.identity);

            TaskList.Add(gm.GetComponent<CollectTask>());
            TaskList[i].TaskIndex = i;

            gm.SetActive(false);
        }
        currentTaskIndex = NumOfTask-1;

        ActivateNextTask();
    }
    public override void update(float timestep)
    {
        
    }
    public override void Clear()
    {
        base.Clear();
    }

    void ActivateNextTask()
    {
        IEnumerator coroutine = s_Instance.ActivateTaskCoroutine();
        s_Instance.StartCoroutine(coroutine);
    }

    IEnumerator ActivateTaskCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (currentTaskIndex != 0)
        {
            currentTaskIndex--;

            // show new target
            TaskList[currentTaskIndex].gameObject.SetActive(true);

            OpenRecord(TaskList[currentTaskIndex]);
        }
        TaskList.RemoveAt(currentTaskIndex);

        // task end
        if (TaskList.Count == 0)
        {
            ExperimentManager.EndExperiment();
        }

        yield return null;
    }

    public static bool FinishTask(int taskIndex)
    {
        if (ExperimentManager.State != ExperimentManager.ExperimentState.Performing) return false;

        if (taskIndex > s_Instance.TaskList.Count) return false;

        s_Instance.CloseRecord(taskIndex);

        return true;
    }

    #region Records

    public override void OpenRecord(BaseTask task)
    {
        CollectRecord prevRecord = (RecordList.Count == 0) ? null : RecordList[RecordList.Count - 1] as CollectRecord;
        RecordList.Add(new CollectRecord(prevRecord, task as CollectTask, ExperimentManager.ExpStartTicks));
    }
    public override void CloseRecord(int taskIndex)
    {
        //record task result
        int recIndex = TaskList[taskIndex].RecordIndex;
        RecordList[recIndex].CloseRecord();

        // generate next task
        ActivateNextTask();
    }
    void PrintResult()
    {
        string dateformat = "yyyyMMdd-HHmm";
        string filename = DateTime.Now.ToString(dateformat);

        string header = CollectRecord.RecordHeader; //TargetPosition
        string content = "";
        for (int i = 0; i < RecordList.Count; i++)
        {
            content += (RecordList[i].ToString() + "\n");
        }

        string path = Application.streamingAssetsPath + "/" + filename + "_DetailRecords" + ".csv";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(header + "\n" + content);
        writer.Close();
    }
    
    #endregion
}
