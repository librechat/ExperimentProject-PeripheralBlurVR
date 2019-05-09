using UnityEngine;
using System;

public class CollectRecord: Record
{

    public static string RecordHeader = "Index,TaskIndex,StartTimeStamp,EndTimeStamp"; // ,PrevDistance,TravelDistance

    // public float prevDistance;  // distance to previous target
    // public Vector3 targetPosition;

    public CollectRecord(CollectRecord preRecord, CollectTask task)
    {
        taskIndex = task.TaskIndex;
        recordIndex = (preRecord == null)? 0: preRecord.recordIndex + 1;

        

        
        // targetPosition = task.gameObject.transform.position;
        // prevDistance = (recordIndex == 0) ? 0.0f : (targetPosition - preRecord.targetPosition).magnitude;
    }

    public override void CloseRecord(BaseTask task)
    {
        CollectTask collect = task as CollectTask;
        long expStartTicks = ExperimentManager.ExpStartTicks;
        startTimeStamp = TicksToSecond(collect.startTick - expStartTicks);
        endTimeStamp = TicksToSecond(collect.closedTick - expStartTicks);
    }

    public override string ToString()
    {
        return recordIndex.ToString() + "," +
            taskIndex.ToString() + "," +
            startTimeStamp.ToString() + "," +
            endTimeStamp.ToString();
        
            //prevDistance.ToString() ;
    }
}