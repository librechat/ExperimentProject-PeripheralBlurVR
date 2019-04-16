using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscomfortTaskManager : BaseTaskManager
{
    [SerializeField, Tooltip("in second")]
    int invokeInterval = 120;

    public static DiscomfortTaskManager s_Instance;

    void Awake()
    {

        if (s_Instance != null) Destroy(gameObject);
        s_Instance = this;
    }

    public override void Init()
    {
        TaskList = new List<BaseTask>();
        RecordList = new List<Record>();

        StartCoroutine(ActivateTaskCoroutine());
    }
    public override void update(float timestep)
    {
        // count to invoke voice (NextTask)
    }
    public override void Clear()
    {
        base.Clear();
    }

    IEnumerator ActivateTaskCoroutine()
    {
        

        yield return new WaitForSeconds((float) invokeInterval);

        Debug.Log("activate a discomfort task");
        // generate a new task
        GameObject gm = Instantiate(taskPrefab);
        DiscomfortTask task = gm.GetComponent<DiscomfortTask>();
        task.TaskIndex = TaskList.Count;
        currentTaskIndex = TaskList.Count;
        TaskList.Add(task);

        OpenRecord(TaskList[currentTaskIndex]);

        yield return null;
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
        DiscomfortRecord prevRecord = (RecordList.Count == 0) ? null : RecordList[RecordList.Count - 1] as DiscomfortRecord;
        RecordList.Add(new DiscomfortRecord(prevRecord, task as DiscomfortTask, ExperimentManager.ExpStartTicks));
    }
    public override void CloseRecord(int taskIndex)
    {
        //record task result
        int recIndex = TaskList[taskIndex].RecordIndex;
        RecordList[recIndex].CloseRecord();
    }
}
