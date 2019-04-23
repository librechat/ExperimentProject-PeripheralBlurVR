using UnityEngine;
using System;

public class CollectRecord: Record
{

    public static string RecordHeader = "Index,TaskIndex,StartTimeStamp,EndTimeStamp,ExecuteTime,PrevDistance"; //,TravelDistance

    public float prevDistance;  // distance to previous target
    public Vector3 targetPosition;

    public CollectRecord(CollectRecord preRecord, CollectTask task, long expStartTicks)
    {
        taskIndex = task.TaskIndex;
        recordIndex = (preRecord == null)? 0: preRecord.recordIndex + 1;

        startTimeStamp = TicksToSecond(DateTime.Now.Ticks - expStartTicks);
        targetPosition = task.gameObject.transform.position;
        prevDistance = (recordIndex == 0) ? 0.0f : (targetPosition - preRecord.targetPosition).magnitude;
    }

    public override void CloseRecord()
    {
        endTimeStamp = TicksToSecond(DateTime.Now.Ticks - ExperimentManager.ExpStartTicks);
        timeStamp = endTimeStamp;
        executeTime = endTimeStamp - startTimeStamp;
    }

    public override string ToString()
    {
        return recordIndex.ToString() + "," +
            taskIndex.ToString() + "," +
            startTimeStamp.ToString() + "," +
            endTimeStamp.ToString() + "," +
            executeTime.ToString() + "," +            
            prevDistance.ToString() ;
        
        //targetPosition.ToString("F3") + "," +
    }
}