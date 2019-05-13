using UnityEngine;
using System;

public class DiscomfortRecord : Record {

    public static string RecordHeader = "Index,TaskIndex,StartTimeStamp,QustionedTimeStamp,EndTimeStamp,Discomfort";

    float questionedTimeStamp = 0.0f;
    public int discomfortScore;

    public DiscomfortRecord(DiscomfortRecord preRecord, DiscomfortTask task)
    {
        taskIndex = task.TaskIndex;
        recordIndex = (preRecord == null) ? 0 : preRecord.recordIndex + 1;

        

        discomfortScore = 0; // will be record manully, remain empty here
    }

    public override void CloseRecord(BaseTask task)
    {
        DiscomfortTask discomfort = task as DiscomfortTask;

        long expStartTicks = ExperimentManager.ExpStartTicks;
        startTimeStamp = TicksToSecond(discomfort.startTick - expStartTicks);
        questionedTimeStamp = TicksToSecond(discomfort.questionedTick - expStartTicks);
        endTimeStamp = TicksToSecond(discomfort.closedTick - expStartTicks);
    }

    public override string ToString()
    {
        return recordIndex.ToString() + "," +
            taskIndex.ToString() + "," +
            startTimeStamp.ToString() + "," +
            questionedTimeStamp.ToString() + "," +
            endTimeStamp.ToString() + "," +
            discomfortScore.ToString();
    }
}
