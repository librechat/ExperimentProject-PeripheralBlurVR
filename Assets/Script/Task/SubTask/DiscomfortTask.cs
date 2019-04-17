using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscomfortTask : BaseTask {

    [SerializeField]
    private AudioClip discomfortQuestion;

    private bool activated = false;
    private float timer = 0.0f;
    private float threshold = 30.0f;

    void Awake()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(discomfortQuestion);
        activated = true;
    }

    void Update()
    {
        if (activated)
        {
            // to check if overtime
            timer += Time.deltaTime;

            if (timer > threshold || InputManager.GetDiscomfortConfirmButton())
            {
                bool result = DiscomfortTaskManager.FinishTask(TaskIndex);
                if (result)
                {
                    activated = false;
                    AudioPlayer.Play(AudioPlayer.AudioName.Done);
                }
            }
        }
    }
}
