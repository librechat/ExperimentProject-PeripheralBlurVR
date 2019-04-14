using UnityEngine;
using System;

public class CollectRecord: Record
{

    public static string RecordHeader = "RecordIndex,TargetIndex,ExecuteTime,Discomfort,PrevDistance,StartTimeStamp,EndTimeStamp,DiscomfortTimeStamp";

    public int targetIndex;
    
    public float prevDistance;  // distance to previous target
    public int discomfort;
    public Vector3 targetPosition;

    public CollectRecord(CollectRecord preRecord, CollectTarget target, long expStartTicks)
    {
        targetIndex = target.TargetIndex;
        recordIndex = (preRecord == null)? 0: preRecord.recordIndex + 1;

        startTimeStamp = TicksToSecond(DateTime.Now.Ticks - expStartTicks);
        targetPosition = target.gameObject.transform.position;
        prevDistance = (recordIndex == 0) ? 0.0f : (targetPosition - preRecord.targetPosition).magnitude;

        //discomfort = (recordIndex == 0) ? -1 : preRecord.discomfort;
    }

    public override void TaskEnd(long expStartTicks)
    {
        endTimeStamp = TicksToSecond(DateTime.Now.Ticks - expStartTicks);
        timeStamp = endTimeStamp;
        executeTime = endTimeStamp - startTimeStamp;
    }

    /* public void RecordDiscomfort(int discomf, long expStartTicks)
    {
        discomfort = discomf;
        discomfortTimeStamp = TicksToSecond(DateTime.Now.Ticks - expStartTicks);
    }*/

    public override string ToString()
    {
        return recordIndex.ToString() + "," +
            targetIndex.ToString() + "," +
            executeTime.ToString() + "," +
            discomfort.ToString() + "," +
            //targetPosition.ToString("F3") + "," +
            prevDistance.ToString() + "," +
            startTimeStamp.ToString() + "," +
            endTimeStamp.ToString();
    }        
}