﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private DiscomfortTaskStage stage = DiscomfortTaskStage.Waiting;
    public DiscomfortTaskStage Stage
    {
        get { return stage; }
        set
        {
            stage = value;
            if (stage == DiscomfortTaskStage.Question)
            {
                TaskManager.ExistVoiceQuestion = true;

                AudioPlayer.Play(AudioPlayer.AudioName.Discomfort);
                timer = 0.0f;               
            }
            else if (stage == DiscomfortTaskStage.Closed)
            {
                TaskManager.ExistVoiceQuestion = false;

                AudioPlayer.PlaySE(AudioPlayer.AudioName.Done);
            }
        }
    }

    void Awake()
    {
        if(!TaskManager.ExistVoiceQuestion){
            Stage = DiscomfortTaskStage.Question;
        }
    }

    void Update()
    {
        if (Stage == DiscomfortTaskStage.Waiting)
        {
            if (!TaskManager.ExistVoiceQuestion)
            {
                Stage = DiscomfortTaskStage.Question;
            }
        }
        else if (Stage == DiscomfortTaskStage.Question)
        {
            // to check if overtime
            timer += Time.deltaTime;

            if (timer > threshold || InputManager.GetDiscomfortConfirmButton())
            {
                bool result = DiscomfortTaskManager.FinishTask(TaskIndex);
                if (result)
                {
                    Stage = DiscomfortTaskStage.Closed;
                }
            }
        }
    }
}
