using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class TaskManager : MonoBehaviour {

    public enum State {
        Prepare = 0,
        Performing,
        Pause,
        Relax
    };

    public enum TaskDataName {
        RotationRandom = 0,
        TranslationRandom,
        MultiplePredifined,
        None
    };

    /*public enum ControllerRotSpeed {
        None = 0,
        Slow,
        Fast
    };

    public enum ControllerNavSpeed {
        None = 0,
        Slow,
        Fast
    };*/

    public State state = State.Prepare;
    Vector3 initialPos;
    [Header("Object Reference")]
    Quaternion initialRotation;
    [SerializeField]
    Transform playerController;
    [SerializeField]
    OVRManager CameraManager;
    [SerializeField]
    GameObject targetPrefab;
    [SerializeField]
    Text hintText;
    [SerializeField]
    Slider discomfortSlider;
    [SerializeField]
    GameObject discomfortPanel;
    
    [Header("Task Options")]
    [SerializeField]
    public TaskDataName CurrentTask;
    [SerializeField]
    bool Blurred = true;

    [Header("Task Details")]

    [SerializeField]
    float ReachableDistance;
    [SerializeField]
    float NavigationMaxDistance;

    [SerializeField]
    List<TaskData> TaskDataList;
    [SerializeField]
    GaussianCameraBlur gaussian;
   

    private TaskData currentTaskData;

    // to be removed later
    [SerializeField]
    List<GameObject> targetAnchorList;

    List<TaskTarget> targetList;

    /* Experiment Records*/
    DateTime ExpStartTime;
    DateTime ExpEndTime;
    List<Record> RecordList;

    float stickCounter = 0.0f;
    float stickSpeed = 7.0f;
    float stickMax = 1.0f;


    public static TaskManager instance;
    
	void Awake () {
        if (instance != null) Destroy(gameObject);
        instance = this;

        currentTaskData = TaskDataList[(int)CurrentTask];
        gaussian.BlurEnabled = Blurred;

        //Debug.Log(TimeSpan.TicksPerSecond);
	}
	
	// Update is called once per frame
	void Update () {
        if (state == State.Prepare)
        {
            if (InputManager.GetStartButton())
            {
                initialPos = playerController.position;
                initialRotation = playerController.rotation;

                ExpStartTime = DateTime.Now;
                Init();

                OVRPlayerController player = playerController.GetComponent<OVRPlayerController>();

                CameraManager.usePositionTracking = currentTaskData.CameraPositionTrack;
                CameraManager.useRotationTracking = currentTaskData.CameraRotationTrack;
                player.EnableLinearMovement = currentTaskData.PlayerPositionCtrl;
                player.EnableRotation = currentTaskData.PlayerRotationCtrl;
                
                //TODO: display start hint

                
            }
        }
        else if (state == State.Performing)
        {
            if (InputManager.GetPauseButton())
            {
                state = State.Pause;
            }
            else if (InputManager.GetQuitButton())
            {
                EndExperiment();
                ClearTargets();
                state = State.Prepare;
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
        else if (state == State.Pause)
        {
            if (InputManager.GetResumeButton())
            {
                state = State.Performing;
            }
        }
        else ;
	}

    void Init() {

        //playerController.position = initialPos;
        //playerController.rotation = initialRotation;

        if (targetList == null) targetList = new List<TaskTarget>();
        else ClearTargets();

        int targetCount = currentTaskData.NumOfTargets;
        if (currentTaskData.Generation == TaskData.GenerationType.Predefined)
        {
            
            if (targetCount > currentTaskData.PositionList.Count) targetCount = currentTaskData.PositionList.Count;
            for (int i = 0; i < targetCount; i++)
            {
                GameObject gm = Instantiate(targetPrefab, currentTaskData.PositionList[i], Quaternion.identity);
                targetList.Add(gm.GetComponent<TaskTarget>());
                targetList[i].TargetIndex = i;
            }
        }
        else
        {
            if (currentTaskData.Task == TaskData.TaskType.Rotation)
            {
                for (int i = 0; i < targetCount; i++)
                {

                    Vector2 circle;
                    Vector3 pos;
                    float distance = ReachableDistance;

                    do
                    {
                        circle = UnityEngine.Random.insideUnitCircle.normalized;
                        pos = initialPos + new Vector3(circle.x, 0, circle.y) * ReachableDistance;

                        distance = ReachableDistance;
                        if (i != 0)
                        {
                            distance = (pos - targetList[i - 1].gameObject.transform.position).magnitude;
                        }

                    } while (distance < ReachableDistance / 2.2f);


                    GameObject gm = Instantiate(targetPrefab);
                    gm.transform.position = pos;
                    targetList.Add(gm.GetComponent<TaskTarget>());
                    targetList[i].TargetIndex = i;
                }
            }
            if (currentTaskData.Task == TaskData.TaskType.Translation)
            {
                for (int i = 0; i < targetCount; i++)
                {
                    float offsetForward = ReachableDistance + NavigationMaxDistance / i + ReachableDistance * UnityEngine.Random.Range(-0.5f, 0.5f);
                    Vector3 pos = initialPos + offsetForward * playerController.forward;
                    GameObject gm = Instantiate(targetPrefab);
                    gm.transform.position = pos;
                    targetList.Add(gm.GetComponent<TaskTarget>());
                    targetList[i].TargetIndex = i;
                }
            }
        }

        for (int i = 0; i < targetList.Count - 1; i++)
        {
            targetList[i].gameObject.SetActive(false);
        }
        RecordList = new List<Record>();
        RecordList.Add(new Record(RecordList[RecordList.Count - 1], targetList[targetList.Count - 1], ExpStartTime.Ticks));

        state = State.Performing;
    }

    void ClearTargets()
    {
        if (targetList != null && targetList.Count != 0)
        {
            for (int i = 0; i < targetList.Count; i++)
            {
                Destroy(targetList[i].gameObject);
            }
            targetList.Clear();
        }
    }

    public static bool Eliminate(int targetIndex)
    {
        if (instance.state != State.Performing) return false;

        if (targetIndex >= instance.targetList.Count) return false;

        //record task result
        instance.RecordList[instance.RecordList.Count - 1].TaskEnd(instance.ExpStartTime.Ticks);

        //pop discomfort panel or generate next task
        if (instance.RecordList.Count % 5 == 0)
        {
            instance.discomfortPanel.SetActive(true);
        }
        else
        {
            IEnumerator coroutine = instance.Generate(targetIndex);
            instance.StartCoroutine(coroutine);
        }
                
        return true;
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

        IEnumerator coroutine = instance.Generate(RecordList[RecordList.Count - 1].targetIndex);
        instance.StartCoroutine(coroutine);
    }

    IEnumerator Generate(int targetIndex)
    {
        yield return new WaitForSeconds(0.5f);

        if (targetIndex != 0)
        {
            // show new target
            targetList[targetIndex - 1].gameObject.SetActive(true);

            RecordList.Add(new Record(RecordList[RecordList.Count - 1], targetList[targetList.Count - 1], ExpStartTime.Ticks));
        }
        instance.targetList.RemoveAt(targetIndex);

        // task end
        if (targetList.Count == 0)
        {
            EndExperiment();
        }

        yield return null;
    }

    void EndExperiment()
    {
        ExpEndTime = DateTime.Now;
        state = State.Relax;

        float time = (float)(ExpEndTime.Ticks - ExpStartTime.Ticks) / (float)TimeSpan.TicksPerSecond;
        //long time = (ExpEndTime.Ticks - ExpStartTime.Ticks) / 1000000;
        Debug.Log("Time Cost: " + time.ToString("F3"));
        //hintText.text = "The End. Total Cost: " + time;
        hintText.text = "End of Experiment!! Thank U";

        PrintResult();
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
           "\nInitPos: "      + initialPos.ToString("F3") +
           "\nExpTotalTime: " + time.ToString("F3");

        path = Application.streamingAssetsPath + "/" + filename + "_ExperimentInfo" + ".txt";
        writer = new StreamWriter(path, true);
        writer.WriteLine(line);
        writer.Close();
    }

    void TurnAnchorsIntoData()
    {
        // turn targetAnchorList into TaskData.PositionList
    }    
}
