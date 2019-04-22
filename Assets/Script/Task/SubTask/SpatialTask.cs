using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialTask : BaseTask {

    public enum SpatialTaskStage
    {
        Waiting,
        Discovered,
        Question,
        Closed
    }

    [SerializeField]
    AudioClip spatialQuestion;

    [SerializeField]
    Color activateColor = Color.yellow;
    [SerializeField]
    Color disableColor = Color.black;

    [SerializeField]
    GameObject lamp;

    private Renderer rend;

    private float timer = 0.0f;
    private float threshold = 30.0f;

    public float angleError = 0.0f;

    public Vector3 endPos;

    private SpatialTaskStage stage = SpatialTaskStage.Waiting;
    public SpatialTaskStage Stage
    {
        get { return stage; }
        set
        {
            stage = value;
            if(stage == SpatialTaskStage.Discovered)
            {
                rend.material.color = activateColor;
                transform.position = endPos;

                SpatialTaskManager.ActivateTask(TaskIndex);
            }
            else if(stage == SpatialTaskStage.Question)
            {
                TaskManager.ExistVoiceQuestion = true;

                AudioPlayer.Play(AudioPlayer.AudioName.Spatial);
                timer = 0.0f;
            }
            else if(stage == SpatialTaskStage.Closed)
            {
                TaskManager.ExistVoiceQuestion = false;

                rend.material.color = disableColor;
                lamp.SetActive(false);
                AudioPlayer.PlaySE(AudioPlayer.AudioName.Done);
            }
        }
    }

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(Stage == SpatialTaskStage.Waiting)
        {
            if (other.gameObject.tag == "Hand" && gameObject.tag != "Hand")
            {
                Stage = SpatialTaskStage.Discovered;
            }
        }
        else if(Stage == SpatialTaskStage.Discovered)
        {
            // to invoke question stage
            if (other.gameObject.tag == "Hand" && gameObject.tag != "Hand")
            {
                Stage = SpatialTaskStage.Question;
            }
        }
    }

    private void Update()
    {
        if(Stage == SpatialTaskStage.Question)
        {
            timer += Time.deltaTime;

            if (InputManager.GetSpatialConfirmButton())
            {
                // calculate error
                Transform controller = ExperimentManager.ControllerStick;
                Vector3 pointing = controller.forward;
                Vector3 groundTruth = transform.position - ExperimentManager.PlayerController.position;

                angleError = Vector3.Angle(pointing, groundTruth);

                bool result = SpatialTaskManager.FinishTask(TaskIndex);
                if (result) Stage = SpatialTaskStage.Closed;
            }
        }
    }

}
