using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    [SerializeField]
    List<Transform> m_ModelList;

    private Renderer rend;

    private float timer = 0.0f;
    private float threshold = 30.0f;

    public float angleError = 0.0f;
    public float angleErrorOnPlane = 0.0f;
    public string taskPosition;
    public string playerPosition;
    public string controllerDirection;

    public Vector3 startPos;
    public Vector3 endPos;

    public long startTick;
    public long discoveredTick;
    public long questionedTick;
    public long closedTick;

    private SpatialTaskStage stage = SpatialTaskStage.Waiting;
    public SpatialTaskStage Stage
    {
        get { return stage; }
        set
        {
            stage = value;
            if(stage == SpatialTaskStage.Discovered)
            {
                //rend.material.color = activateColor;
                AudioPlayer.PlaySE(AudioPlayer.AudioName.SpatialDiscovered);

                //StartCoroutine(FadeOutCoroutine());
                startPos = transform.position;
                transform.position = endPos;
                Light light = lamp.GetComponent<Light>();
                light.intensity = 0.8f;
                light.color = Color.blue;

                discoveredTick = DateTime.Now.Ticks;

                SpatialTaskManager.ActivateTask(TaskIndex);
            }
            else if(stage == SpatialTaskStage.Question)
            {
                //TaskManager.ExistVoiceQuestion = true;
                questionedTick = DateTime.Now.Ticks;

                //AudioPlayer.Play(AudioPlayer.AudioName.Spatial);
                int index = TaskIndex;
                //if (ExperimentManager.CurrentExpSetting.level == LevelData.LevelEnum.Level_B) index = (20 - 1) - TaskIndex;

                AudioPlayer.PlaySpatialQuestion(index);


                timer = 0.0f;
            }
            else if(stage == SpatialTaskStage.Closed)
            {
                //TaskManager.ExistVoiceQuestion = false;
                closedTick = DateTime.Now.Ticks;

                //rend.material.color = disableColor;
                AudioPlayer.PlaySE(AudioPlayer.AudioName.Done);

                //StartCoroutine(DisableCoroutine());
                lamp.GetComponent<Light>().intensity = 0.0f;
                gameObject.SetActive(false);
            }
        }
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        int index = TaskIndex;
        //if (ExperimentManager.CurrentExpSetting.level == LevelData.LevelEnum.Level_B) index = (20 - 1) - TaskIndex;

        Transform model = Instantiate(m_ModelList[index], this.transform);
        rend = model.GetComponent<Renderer>();

        startTick = DateTime.Now.Ticks;
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
                Vector3 groundTruth = startPos - ExperimentManager.PlayerController.position;

                angleError = Vector3.Angle(pointing, groundTruth);
                angleErrorOnPlane = Vector3.SignedAngle(new Vector3(pointing.x, 0, pointing.z), new Vector3(groundTruth.x, 0, groundTruth.z), Vector3.up);

                taskPosition = startPos.ToString("F3");
                playerPosition = ExperimentManager.PlayerController.position.ToString("F3");
                controllerDirection = pointing.ToString("F3");

                Stage = SpatialTaskStage.Closed;
                bool result = SpatialTaskManager.FinishTask(TaskIndex);
                if (!result)
                {

                } 
            }
        }
    }

    IEnumerator FadeOutCoroutine()
    {
        float timer = 0.0f;
        float fadeout_time = 0.3f;
        float dissolve_amount = 0.0f;

        Light light = lamp.GetComponent<Light>();
        float intensity = 1.0f;

        while (timer < fadeout_time)
        {
            timer += Time.deltaTime;
            dissolve_amount = timer / fadeout_time;
            intensity = 1.0f - timer / fadeout_time;

            light.intensity = intensity;
            rend.material.SetFloat("_DissolveAmount", dissolve_amount);
            
            yield return null;
        }

        transform.position = endPos;
        light.intensity = 0.8f;
        rend.material.SetFloat("_DissolveAmount", 0.0f);

        yield return null;
    }

    IEnumerator DisableCoroutine()
    {
        float timer = 0.0f;
        float fadeout_time = 0.3f;
        float dissolve_amount = 0.0f;

        Light light = lamp.GetComponent<Light>();
        float intensity = 0.8f;

        while (timer < fadeout_time)
        {
            timer += Time.deltaTime;
            dissolve_amount = timer / fadeout_time;
            intensity = 1.0f - timer / fadeout_time;

            light.intensity = intensity;
            rend.material.SetFloat("_DissolveAmount", dissolve_amount);

            yield return null;
        }
        light.intensity = 0.0f;

        gameObject.SetActive(false);

        yield return null;
    }

}
