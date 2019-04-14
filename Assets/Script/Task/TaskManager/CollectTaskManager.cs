using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class CollectTaskManager : BaseTaskManager {

    public override void Init()
    {
        base.Init();
    }
    public override void update()
    {

    }

    void NextTask()
    {

    }

    public void OpenNewRecord(CollectTask target)
    {
        CollectRecord prevRecord = (RecordList.Count == 0) ? null : RecordList[RecordList.Count - 1];

        // RecordList.Add(new CollectRecord(prevRecord, target, ExperimentManager.ExpStartTicks));
    }
    public override void CloseRecord()
    {
        //record task result
        RecordList[RecordList.Count - 1].TaskEnd(ExperimentManager.ExpStartTicks);

        // generate next task
        NextTask();

        //pop discomfort panel or generate next task
        
        /*if (RecordList.Count % 5 == 0)
        {
            discomfortPanel.SetActive(true);
        }
        else
        {
            TaskManager.NextTarget();
        }*/
    }
    void PrintResult()
    {
        string dateformat = "yyyyMMdd-HHmm";
        string filename = DateTime.Now.ToString(dateformat);

        string header = CollectRecord.RecordHeader; //TargetPosition
        string content = "";
        for (int i = 0; i < RecordList.Count; i++)
        {
            content += (RecordList[i].ToString() + "\n");
        }

        string path = Application.streamingAssetsPath + "/" + filename + "_DetailRecords" + ".csv";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(header + "\n" + content);
        writer.Close();
    }
}
