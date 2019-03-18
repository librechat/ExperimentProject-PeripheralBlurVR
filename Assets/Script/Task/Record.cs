using UnityEngine;
using System;

public class Record
{
    public int recordIndex;
    public int targetIndex;
    public float timeStamp;     // now - expstart
    public float executeTime;   // end - start
    public float prevDistance;  // distance to previous target
    public int discomfort;
    public Vector3 targetPosition;

    private float startTimeStamp;
    private float endTimeStamp;
    private float discomfortTimeStamp = -1.0f;

    public Record(Record preRecord, TaskTarget target, long expStartTicks)
    {
        targetIndex = target.TargetIndex;
        recordIndex = (preRecord == null)? 0: preRecord.recordIndex + 1;

        startTimeStamp = TicksToSecond(DateTime.Now.Ticks - expStartTicks);
        targetPosition = target.gameObject.transform.position;
        prevDistance = (recordIndex == 0) ? 0.0f : (targetPosition - preRecord.targetPosition).magnitude;

        discomfort = (recordIndex == 0) ? -1 : preRecord.discomfort;
    }

    public void TaskEnd(long expStartTicks)
    {
        endTimeStamp = TicksToSecond(DateTime.Now.Ticks - expStartTicks);
        timeStamp = endTimeStamp;
        executeTime = endTimeStamp - startTimeStamp;
    }

    public void RecordDiscomfort(int discomf, long expStartTicks)
    {
        discomfort = discomf;
        discomfortTimeStamp = TicksToSecond(DateTime.Now.Ticks - expStartTicks);
    }

    public string ToString()
    {
        return recordIndex.ToString() + "," +
            targetIndex.ToString() + "," +
            executeTime.ToString() + "," +
            discomfort.ToString() + "," +
            //targetPosition.ToString("F3") + "," +
            prevDistance.ToString() + "," +
            startTimeStamp.ToString() + "," +
            endTimeStamp.ToString() + "," +
            discomfortTimeStamp.ToString();
    }

    private float TicksToSecond(long ticks){
        return (float)ticks / (float)TimeSpan.TicksPerSecond;
        //return (float)ticks / 1000000.0f;
    }
        
}