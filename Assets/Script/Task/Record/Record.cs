using UnityEngine;
using System;

public class Record {

    public int recordIndex;

    public float timeStamp;     // now - expstart
    public float executeTime;   // end - start

    protected float startTimeStamp;
    protected float endTimeStamp;

    public virtual void TaskEnd(long expStartTicks)
    {
        endTimeStamp = TicksToSecond(DateTime.Now.Ticks - expStartTicks);
        timeStamp = endTimeStamp;
        executeTime = endTimeStamp - startTimeStamp;
    }

    public virtual string ToString()
    {
        return "";
    }

    protected float TicksToSecond(long ticks)
    {
        return (float)ticks / (float)TimeSpan.TicksPerSecond;
        //return (float)ticks / 1000000.0f;
    }
}
