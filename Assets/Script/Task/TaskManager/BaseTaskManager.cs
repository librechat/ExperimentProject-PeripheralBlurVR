using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTaskManager : MonoBehaviour {

    protected List<CollectRecord> RecordList;

    public virtual void Init()
    {
        RecordList = new List<CollectRecord>();
    }

    public virtual void update()
    {

    }

    public virtual void OpenNewRecord(BaseTask task)
    {
        
    }

    public virtual void CloseRecord()
    {
        
    }
}
