using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscomfortTaskManager : BaseTaskManager
{
    public static DiscomfortTaskManager s_Instance;

    void Awake()
    {

        if (s_Instance != null) Destroy(gameObject);
        s_Instance = this;
    }

    public override void Init()
    {
        base.Init();
    }
    public override void update()
    {
        // count to invoke voice (NextTask)
    }
    public override void Clear()
    {
        base.Clear();
    }

    public void OpenNewRecord(DiscomfortTask task)
    {
        DiscomfortRecord prevRecord = (RecordList.Count == 0) ? null : RecordList[RecordList.Count - 1] as DiscomfortRecord;

        RecordList.Add(new DiscomfortRecord(prevRecord, task, ExperimentManager.ExpStartTicks));
    }
    public override void CloseRecord()
    {
        //record task result
        RecordList[RecordList.Count - 1].TaskEnd(ExperimentManager.ExpStartTicks);
    }
}
