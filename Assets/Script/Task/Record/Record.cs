using UnityEngine;
using System;

public class Record {

    public int taskIndex;
    public int recordIndex;

    public float timeStamp;     // now - expstart
    public float executeTime;   // end - start

    protected float startTimeStamp;
    protected float endTimeStamp;

    public virtual void CloseRecord()
    {
        
    }

    protected float TicksToSecond(long ticks)
    {
        return (float)ticks / (float)TimeSpan.TicksPerSecond;
        //return (float)ticks / 1000000.0f;
    }
}
