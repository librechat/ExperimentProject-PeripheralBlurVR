using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

[ExecuteInEditMode]
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

    [Header("Manager")]
    [SerializeField]
    VisaulEffectManager visualEffectManager;

    [Header("Object Reference")]

    [SerializeField]
    Transform playerController;
   
    [SerializeField]
    Text hintText;   

    /* Experiment Records */
    public static long ExpStartTicks
    {
        get { return instance.expStartTime.Ticks; }
    }
    private DateTime expStartTime;
    private DateTime expEndTime;
    List<CollectRecord> RecordList;

    float stickCounter = 0.0f;
    float stickSpeed = 7.0f;
    float stickMax = 1.0f;

    [SerializeField]
    TaskManager taskManager;


    public static ExperimentManager instance;
    
	void Awake () {
        if (instance != null) Destroy(gameObject);
        instance = this;

        m_Condition = m_ConditionList[(int)m_ConditionName];
	}

    void OnValidate()
    {
        visualEffectManager.SetParameters(m_ConditionList[(int)m_ConditionName]);
    }
	
	void Update () {
#if UNITY_EDITOR
        if(!Application.isPlaying){
            return;
        }
#endif
        if (state == ExperimentState.Prepare)
        {
            if (InputManager.GetStartButton())
            {
                Debug.Log("Get start button");
                expStartTime = DateTime.Now;

                RecordList = new List<CollectRecord>();

                taskManager.Init(playerController);

                // display start hint?

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
                taskManager.End();
                state = ExperimentState.Prepare;
            }
            else ;
            
            taskManager.update(Time.deltaTime);
            
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

    public static void EndExperiment()
    {
        instance.expEndTime = DateTime.Now;
        instance.state = ExperimentState.Relax;

        float time = (float)(instance.expEndTime.Ticks - instance.expStartTime.Ticks) / (float)TimeSpan.TicksPerSecond;
        //long time = (ExpEndTime.Ticks - ExpStartTime.Ticks) / 1000000;
        Debug.Log("Time Cost: " + time.ToString("F3"));
        //hintText.text = "The End. Total Cost: " + time;
        instance.hintText.text = "End of Experiment!! Thank U";

        // instance.PrintResult();
    }

    void PrintSummary()
    {

    }
}
