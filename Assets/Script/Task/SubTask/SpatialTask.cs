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

    private Renderer rend;
    private AudioSource audioSource;

    private float timer = 0.0f;
    private float threshold = 30.0f;

    public float angleError = 0.0f;

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
                SpatialTaskManager.ActivateTask(TaskIndex);
            }
            else if(stage == SpatialTaskStage.Question)
            {
                // spatialQuestion
                audioSource.PlayOneShot(spatialQuestion);
                timer = 0.0f;
            }
            else if(stage == SpatialTaskStage.Closed)
            {
                rend.material.color = disableColor;
            }
        }
    }

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
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
    }

    private void Update()
    {
        if (Stage == SpatialTaskStage.Discovered)
        {
            // count relative distance between participant and this
            Vector3 playerPos = ExperimentManager.PlayerController.position;
            Vector3 raycastDirection = playerPos - transform.position;
            float distance = raycastDirection.magnitude + 0.1f;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, raycastDirection, out hit, distance))
            {
                if(hit.collider.tag == "Wall")
                {
                    Stage = SpatialTaskStage.Question;
                }
            }
        }
        else if(Stage == SpatialTaskStage.Question)
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
