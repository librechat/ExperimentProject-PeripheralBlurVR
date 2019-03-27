﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class ExperimentManager : MonoBehaviour {

    public enum ExperimentState {
        Prepare = 0,
        Performing,
        Pause,
        Relax
    };

    public static ExperimentState State
    {
        get { return instance.state; }
        set
        {
            instance.state = value;
        }
    }
    private ExperimentState state = ExperimentState.Prepare;

    [SerializeField]
    ConditionData.ConditionEnum m_ConditionName;
    [SerializeField]
    List<ConditionData> m_ConditionList;
    public static ConditionData Condition {
        get{
            if (instance.m_Condition == null)
            {
                instance.m_Condition = instance.m_ConditionList[(int)instance.m_ConditionName];
            }
            return instance.m_Condition;
        }
    }
    private ConditionData m_Condition;

    [Header("Object Reference")]

    [SerializeField]
    Transform playerController;
   
    [SerializeField]
    Text hintText;
    [SerializeField]
    Slider discomfortSlider;
    [SerializeField]
    GameObject discomfortPanel;
    

    /* Experiment Records */
    DateTime ExpStartTime;
    DateTime ExpEndTime;
    List<Record> RecordList;

    float stickCounter = 0.0f;
    float stickSpeed = 7.0f;
    float stickMax = 1.0f;


    public static ExperimentManager instance;
    
	void Awake () {
        if (instance != null) Destroy(gameObject);
        instance = this;

        m_Condition = m_ConditionList[(int)m_ConditionName];
	}
	
	void Update () {
        if (state == ExperimentState.Prepare)
        {
            if (InputManager.GetStartButton())
            {
                Debug.Log("Get start button");
                ExpStartTime = DateTime.Now;

                RecordList = new List<Record>();
                TaskManager.Init(playerController);
                TaskManager.NextTarget();

                // TODO: display start hint?

                state = ExperimentState.Performing;
            }
        }
        else if (state == ExperimentState.Performing)
        {
            if (InputManager.GetPauseButton())
            {
                state = ExperimentState.Pause;
            }
            else if (InputManager.GetQuitButton())
            {
                EndExperiment();
                TaskManager.Clear();
                state = ExperimentState.Prepare;
            }
            else ;

            if(discomfortPanel.activeSelf){
                if (InputManager.GetLStickToRight())
                {
                    stickCounter += stickSpeed * Time.deltaTime;
                    if (stickCounter > stickMax)
                    {
                        discomfortSlider.value++;
                        stickCounter = 0;
                    }
                
                }
                else if (InputManager.GetLStickToLeft())
                {
                    stickCounter += stickSpeed * Time.deltaTime;
                    if (stickCounter > stickMax)
                    {
                        discomfortSlider.value--;
                        stickCounter = 0;
                    }
                }
                else stickCounter = 0.0f;
                if (InputManager.GetLStickConfirm())
                {              
                    OnDiscomfortAnswered(discomfortSlider.value);
                } 
            }         

        }
        else if (state == ExperimentState.Pause)
        {
            if (InputManager.GetResumeButton())
            {
                state = ExperimentState.Performing;
            }
        }
        else ;
	}

    public void OnDiscomfortAnswered(float value)
    {

        RecordList[RecordList.Count - 1].RecordDiscomfort((int)value, instance.ExpStartTime.Ticks);
        discomfortPanel.SetActive(false);

        if ((int)value == 10)
        {
            EndExperiment();
            return;
        }

        TaskManager.NextTarget();
    }    

    public static void EndExperiment()
    {
        instance.ExpEndTime = DateTime.Now;
        instance.state = ExperimentState.Relax;

        float time = (float)(instance.ExpEndTime.Ticks - instance.ExpStartTime.Ticks) / (float)TimeSpan.TicksPerSecond;
        //long time = (ExpEndTime.Ticks - ExpStartTime.Ticks) / 1000000;
        Debug.Log("Time Cost: " + time.ToString("F3"));
        //hintText.text = "The End. Total Cost: " + time;
        instance.hintText.text = "End of Experiment!! Thank U";

        instance.PrintResult();
    }

    public static void OpenNewRecord(TaskTarget target)
    {
        Record prevRecord = (instance.RecordList.Count == 0) ? null : instance.RecordList[instance.RecordList.Count - 1];

        instance.RecordList.Add(new Record(prevRecord, target, instance.ExpStartTime.Ticks));
    }
    public static void CloseRecord()
    {
        //record task result
        instance.RecordList[instance.RecordList.Count - 1].TaskEnd(instance.ExpStartTime.Ticks);

        //pop discomfort panel or generate next task
        if (instance.RecordList.Count % 5 == 0)
        {
            instance.discomfortPanel.SetActive(true);
        }
        else
        {
            TaskManager.NextTarget();
        }
    }

    void PrintResult()
    {
        float time = (float)(ExpEndTime.Ticks - ExpStartTime.Ticks) / (float)TimeSpan.TicksPerSecond;
        string dateformat = "yyyyMMdd-HHmm";
        string filename = DateTime.Now.ToString(dateformat);

        string header = "RecordIndex,TargetIndex,ExecuteTime,Discomfort,PrevDistance,StartTimeStamp,EndTimeStamp,DiscomfortTimeStamp"; //TargetPosition
        string content = "";
        for (int i = 0; i < RecordList.Count; i++)
        {
            content += (RecordList[i].ToString() + "\n");
        }

        string path = Application.streamingAssetsPath +"/" + filename + "_DetailRecords" + ".csv";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine( header +"\n"+ content );
        writer.Close();


        string line =
           DateTime.Now.ToString(dateformat) +
           "\nExpTotalTime: " + time.ToString("F3");

        path = Application.streamingAssetsPath + "/" + filename + "_ExperimentInfo" + ".txt";
        writer = new StreamWriter(path, true);
        writer.WriteLine(line);
        writer.Close();
    }   
}
