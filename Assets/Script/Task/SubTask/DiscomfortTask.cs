using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DiscomfortTask : BaseTask {

    public enum DiscomfortTaskStage
    {
        Waiting,
        Question,
        Closed
    }

    [SerializeField]
    private AudioClip discomfortQuestion;

    private float timer = 0.0f;
    private float threshold = 30.0f;

    public long startTick;
    public long questionedTick;
    public long closedTick;

    private DiscomfortTaskStage stage = DiscomfortTaskStage.Waiting;
    public DiscomfortTaskStage Stage
    {
        get { return stage; }
        set
        {
            stage = value;
            if (stage == DiscomfortTaskStage.Question)
            {
                //TaskManager.ExistVoiceQuestion = true;
                questionedTick = DateTime.Now.Ticks;

                AudioPlayer.Play(AudioPlayer.AudioName.Discomfort);
                timer = 0.0f;               
            }
            else if (stage == DiscomfortTaskStage.Closed)
            {
                //TaskManager.ExistVoiceQuestion = false;
                closedTick = DateTime.Now.Ticks;
                Debug.Log("discomfort closed");
                AudioPlayer.PlaySE(AudioPlayer.AudioName.Done);
            }
        }
    }

    void Awake()
    {
        
    }

    private void Start()
    {
        startTick = DateTime.Now.Ticks;
    }

    void Update()
    {
        if (Stage == DiscomfortTaskStage.Waiting)
        {
            if (true)
            {
                Stage = DiscomfortTaskStage.Question;
            }
        }
        else if (Stage == DiscomfortTaskStage.Question)
        {
            // to check if overtime
            timer += Time.deltaTime;

            // timer > threshold
            if (InputManager.GetDiscomfortConfirmButton())
            {
                Stage = DiscomfortTaskStage.Closed;
                bool result = DiscomfortTaskManager.FinishTask(TaskIndex);
                if (!result)
                {
                    // Stage = DiscomfortTaskStage.Closed;
                    Debug.Log("error happens in discomfort task");
                }
            }
        }
    }
}
