﻿using UnityEngine;
using System;

public class DiscomfortRecord : Record {

    public static string RecordHeader = "Index,TaskIndex,StartTimeStamp,EndTimeStamp,ResponseTime,Discomfort";
    public int discomfort;

    public DiscomfortRecord(DiscomfortRecord preRecord, DiscomfortTask task, long expStartTicks)
    {
        taskIndex = task.TaskIndex;
        recordIndex = (preRecord == null) ? 0 : preRecord.recordIndex + 1;

        startTimeStamp = TicksToSecond(DateTime.Now.Ticks - expStartTicks);

        discomfort = 0; // will be record manully, remain empty here
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
            discomfort.ToString();
    }
}
