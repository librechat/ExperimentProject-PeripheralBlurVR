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
    public void Init(List<BaseEnvBuilder.SpatialTaskData> infoList)
    {
        TaskList = new List<BaseTask>();
        RecordList = new List<Record>();

        for (int i = 0; i < infoList.Count; i++)
        {
            GameObject gm = Instantiate(taskPrefab, infoList[i].startPos, Quaternion.identity);

            SpatialTask task = gm.GetComponent<SpatialTask>();
            TaskList.Add(task);

            task.endPos = infoList[i].endPos;
            task.TaskIndex = i;

            gm.SetActive(false);
        }

        currentTaskIndex = 0;
        ActivateNextTask();
    }
    public override void update(float timestep)
    {

    }
    public override void Clear()
    {
        base.Clear();
    }
    public static void ActivateTask(int taskIndex)
    {
        s_Instance.OpenRecord(s_Instance.TaskList[taskIndex]);
    }
    public static bool FinishTask(int taskIndex)
    {
        if (ExperimentManager.State != ExperimentManager.ExperimentState.Performing) return false;

        if (taskIndex > s_Instance.TaskList.Count) return false;

        s_Instance.CloseRecord(taskIndex);

        return true;
    }

    public override void OpenRecord(BaseTask task)
    {
        task.RecordIndex = RecordList.Count;
        SpatialRecord prevRecord = (RecordList.Count == 0) ? null : RecordList[RecordList.Count - 1] as SpatialRecord;
        RecordList.Add(new SpatialRecord(prevRecord, task as SpatialTask));
    }
    public override void CloseRecord(int taskIndex)
    {
        // fill in error
        SpatialTask task = TaskList[taskIndex] as SpatialTask;
        SpatialRecord rec = RecordList[task.RecordIndex] as SpatialRecord;

        // record task result
        rec.CloseRecord(task);

        ActivateNextTask();
    }

    void ActivateNextTask()
    {
        if (currentTaskIndex < NumOfTask)
        {
            // show new target
            TaskList[currentTaskIndex].gameObject.SetActive(true);
            currentTaskIndex++;
        }
        else
        {
            ExperimentManager.TryEndExperiment("spatial");
        }
    }
}
