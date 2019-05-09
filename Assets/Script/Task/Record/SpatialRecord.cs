using UnityEngine;
using System;

public class SpatialRecord : Record {

    public static string RecordHeader = "Index,TaskIndex,StartTimeStamp,DiscoveredTimeStamp,QuestionedTimeStamp,EndTimeStamp,ResponseTime,Error";

    public float discoveredTimeStamp = 0.0f;
    public float questionedTimeStamp = 0.0f;
    public float angleError = 0.0f;

    public SpatialRecord(SpatialRecord preRecord, SpatialTask task)
    {
        taskIndex = task.TaskIndex;
        recordIndex = (preRecord == null) ? 0 : preRecord.recordIndex + 1;
    }

    public override void CloseRecord(BaseTask task)
    {
        SpatialTask spatial = task as SpatialTask;

        angleError = spatial.angleError;

        long expStartTicks = ExperimentManager.ExpStartTicks;
        startTimeStamp = TicksToSecond(spatial.startTick - expStartTicks);
        discoveredTimeStamp = TicksToSecond(spatial.discoveredTick - expStartTicks);
        questionedTimeStamp = TicksToSecond(spatial.questionedTick - expStartTicks);
        endTimeStamp = TicksToSecond(spatial.closedTick - expStartTicks);
    }

    public override string ToString()
    {
        return recordIndex.ToString() + "," +
            taskIndex.ToString() + "," +
            startTimeStamp.ToString() + "," +
            discoveredTimeStamp.ToString() + "," +
            questionedTimeStamp.ToString() + "," +
            endTimeStamp.ToString() + "," +
            angleError.ToString("F3");

        //targetPosition.ToString("F3") + "," +
    }
}
