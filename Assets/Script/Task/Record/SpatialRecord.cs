using UnityEngine;
using System;

public class SpatialRecord : Record {

    public static string RecordHeader = "Index,TaskIndex,StartTimeStamp,EndTimeStamp,ResponseTime,Error";

    public SpatialRecord(SpatialRecord preRecord, SpatialTask task, long expStartTicks)
    {
        taskIndex = task.TaskIndex;
        recordIndex = (preRecord == null) ? 0 : preRecord.recordIndex + 1;

        startTimeStamp = TicksToSecond(DateTime.Now.Ticks - expStartTicks);
    }

    public override void TaskEnd(long expStartTicks)
    {
        base.TaskEnd(expStartTicks);
    }
}
