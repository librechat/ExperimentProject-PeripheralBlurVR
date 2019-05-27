using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

[ExecuteInEditMode]
public class ExperimentManager : MonoBehaviour {

    public enum ExperimentState {
        Prepare,
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

    [Header("Experiment Setting Data")]
    [SerializeField]
    private ExperimentData m_ExperimentSettingConfig;
    public static ExperimentData.SessionSetting CurrentExpSetting
    {
        get
        {
            return instance.m_ExperimentSettingConfig.SessionSettings[instance.m_ExperimentSettingConfig.session-1];
        }
    }

    ConditionData.ConditionEnum m_ConditionName;
    public static string ConditionName
    {
        get {
            return instance.m_ConditionName.ToString();
        }
    }
    [SerializeField]
    private bool usingGazeInfo = true;

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
    [SerializeField]
    VRRecorderManager recordManager;

    [Header("Object Reference")]

    [SerializeField]
    Transform vrRig;
    public static Transform VRRig
    {
        get { return instance.vrRig; }
    }


    [SerializeField]
    Transform playerController;
    public static Transform PlayerController
    {
        get { return instance.playerController; }
    }
    [SerializeField]
    Transform controllerStick;
    public static Transform ControllerStick
    {
        get { return instance.controllerStick; }
    }
    [SerializeField]
    GameObject gazeManager;

    [SerializeField]
    Text hintText;   

    /* Experiment Records */
    public static long ExpStartTicks
    {
        get { return instance.expStartTime.Ticks; }
    }
    private DateTime expStartTime;
    private DateTime expEndTime;

    float stickCounter = 0.0f;
    float stickSpeed = 7.0f;
    float stickMax = 1.0f;

    [SerializeField]
    TaskManager taskManager;


    public static ExperimentManager instance;
    
	void Awake () {
        if (instance != null) Destroy(gameObject);
        instance = this;

        m_ConditionName = CurrentExpSetting.condition;
        m_Condition = m_ConditionList[(int)m_ConditionName];

        if (!usingGazeInfo)
        {
            gazeManager.SetActive(false);
            visualEffectManager.Init();
        }
        else gazeManager.SetActive(true);

    }

    void OnValidate()
    {
        visualEffectManager.SetParameters(m_ConditionList[(int)m_ConditionName]);
    }
	
	void FixedUpdate () {
#if UNITY_EDITOR
        if(!Application.isPlaying){
            return;
        }
#endif
        recordManager._Update(Time.fixedDeltaTime);
        if (state == ExperimentState.Prepare)
        {
            /*if (InputManager.GetStartButton())
            {
                
            }*/
            if (!usingGazeInfo)
            {
                if (InputManager.GetStartButton()) StartExperiment();
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
            else;

            taskManager.update(Time.deltaTime);

        }
        else if (state == ExperimentState.Pause)
        {
            if (InputManager.GetResumeButton())
            {
                state = ExperimentState.Performing;
            }
        }
        else;
	}

    public static void StartExperiment()
    {
        Debug.Log("start experiment");

        instance.state = ExperimentState.Performing;

        instance.expStartTime = DateTime.Now;
        instance.taskManager.Init(instance.playerController);
        
        // play start hint audio?
    }

    bool m_CollectEnd = false;
    bool m_SpatialEnd = false;

    public static void TryEndExperiment(string source)
    {
        if (source == "collect") instance.m_CollectEnd = true;
        if (source == "spatial") instance.m_SpatialEnd = true;

        if (instance.m_CollectEnd && instance.m_SpatialEnd) ExperimentManager.EndExperiment();
    }

    public static void EndExperiment()
    {
        AudioPlayer.Play(AudioPlayer.AudioName.Final);
        instance.state = ExperimentState.Relax;
        instance.expEndTime = DateTime.Now;        

        float time = (float)(instance.expEndTime.Ticks - instance.expStartTime.Ticks) / (float)TimeSpan.TicksPerSecond;
        //long time = (ExpEndTime.Ticks - ExpStartTime.Ticks) / 1000000;
        Debug.Log("Time Cost: " + time.ToString("F3"));
        //hintText.text = "The End. Total Cost: " + time;
        //instance.hintText.text = "End of Experiment!! Thank U";

        instance.PrintSummary();
        instance.recordManager.Stop();
    }

    void PrintSummary()
    {
        taskManager.PrintResult();

        string condition = m_ConditionName.ToString();

        float time = (float)(expEndTime.Ticks - expStartTime.Ticks) / (float)TimeSpan.TicksPerSecond;
        string dateformat = "yyyyMMdd-HHmm";
        string filename = DateTime.Now.ToString(dateformat) + "-" + condition;

        string line =
            condition + "\n"+
            DateTime.Now.ToString(dateformat) +
           "\nExpTotalTime: " + time.ToString("F3");
        string path = Application.streamingAssetsPath + "/" + filename + ".txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(line);
        writer.Close();
    }
}
