using UnityEngine;
using System;

public class Record {

    public int taskIndex;
    public int recordIndex;

    protected float startTimeStamp;
    protected float endTimeStamp;

    public virtual void CloseRecord(BaseTask task)
    {
        
    }

    protected float TicksToSecond(long ticks)
    {
        return (float)ticks / (float)TimeSpan.TicksPerSecond;
        //return (float)ticks / 1000000.0f;
    }
}
