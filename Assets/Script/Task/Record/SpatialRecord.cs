using UnityEngine;
using System;

public class SpatialRecord : Record {

    public static string RecordHeader = "Index,TaskIndex,StartTimeStamp,EndTimeStamp,ResponseTime,Error";

    public float angleError = 0.0f;

    public SpatialRecord(SpatialRecord preRecord, SpatialTask task, long expStartTicks)
    {
        taskIndex = task.TaskIndex;
        recordIndex = (preRecord == null) ? 0 : preRecord.recordIndex + 1;

        startTimeStamp = TicksToSecond(DateTime.Now.Ticks - expStartTicks);
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
            angleError.ToString("F3");

        //targetPosition.ToString("F3") + "," +
    }
}
