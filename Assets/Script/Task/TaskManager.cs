using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour {

    public enum TaskDataName
    {
        RotationRandom = 0,
        TranslationRandom,
        MultiplePredifined,
        None
    };

    [Header("Task Options")]
    [SerializeField]
    public TaskDataName CurrentTask;

    [Header("Task Details")]

    [SerializeField]
    float ReachableDistance = 0.8f;
    [SerializeField]
    float NavigationMaxDistance = 20;

    [SerializeField]
    List<TaskData> TaskDataList;

    [Header("Object Reference")]

    [SerializeField]
    GameObject targetPrefab;

    // TODO: to be removed later
    [SerializeField]
    List<GameObject> targetAnchorList;

    List<TaskTarget> targetList;

    private TaskData currentTaskData;
    private int currentTargetIndex = -1;

    public static TaskManager s_Instance;

    void Awake () {

        if (s_Instance != null) Destroy(gameObject);
        s_Instance = this;

        currentTaskData = TaskDataList[(int)CurrentTask];
    }
	
	void Update () {
		
	}

    public static void Init(Vector3 initialPos, Quaternion initialRotation, Vector3 forward)
    {
        if (s_Instance.targetList == null) s_Instance.targetList = new List<TaskTarget>();
        else Clear();

        int targetCount = s_Instance.currentTaskData.NumOfTargets;
        if (s_Instance.currentTaskData.Generation == TaskData.GenerationType.Predefined)
        {

            if (targetCount > s_Instance.currentTaskData.PositionList.Count) targetCount = s_Instance.currentTaskData.PositionList.Count;
            for (int i = 0; i < targetCount; i++)
            {
                GameObject gm = Instantiate(s_Instance.targetPrefab, s_Instance.currentTaskData.PositionList[i], Quaternion.identity);
                s_Instance.targetList.Add(gm.GetComponent<TaskTarget>());
                s_Instance.targetList[i].TargetIndex = i;
            }
        }
        else
        {
            if (s_Instance.currentTaskData.Task == TaskData.TaskType.Rotation)
            {
                for (int i = 0; i < targetCount; i++)
                {

                    Vector2 circle;
                    Vector3 pos;
                    float distance = s_Instance.ReachableDistance;

                    do
                    {
                        circle = UnityEngine.Random.insideUnitCircle.normalized;
                        pos = initialPos + new Vector3(circle.x, 0, circle.y) * s_Instance.ReachableDistance;

                        distance = s_Instance.ReachableDistance;
                        if (i != 0)
                        {
                            distance = (pos - s_Instance.targetList[i - 1].gameObject.transform.position).magnitude;
                        }

                    } while (distance < s_Instance.ReachableDistance / 2.2f);


                    GameObject gm = Instantiate(s_Instance.targetPrefab);
                    gm.transform.position = pos;
                    s_Instance.targetList.Add(gm.GetComponent<TaskTarget>());
                    s_Instance.targetList[i].TargetIndex = i;
                }
            }
            if (s_Instance.currentTaskData.Task == TaskData.TaskType.Translation)
            {
                for (int i = 0; i < targetCount; i++)
                {
                    float offsetForward = s_Instance.ReachableDistance + s_Instance.NavigationMaxDistance / i + s_Instance.ReachableDistance * UnityEngine.Random.Range(-0.5f, 0.5f);
                    Vector3 pos = initialPos + offsetForward * forward;
                    GameObject gm = Instantiate(s_Instance.targetPrefab);
                    gm.transform.position = pos;
                    s_Instance.targetList.Add(gm.GetComponent<TaskTarget>());
                    s_Instance.targetList[i].TargetIndex = i;
                }
            }
        }

        for (int i = 0; i < s_Instance.targetList.Count; i++)
        {
            s_Instance.targetList[i].gameObject.SetActive(false);
        }
    }

    public static void Clear()
    {
        if (s_Instance.targetList != null && s_Instance.targetList.Count != 0)
        {
            for (int i = 0; i < s_Instance.targetList.Count; i++)
            {
                Destroy(s_Instance.targetList[i].gameObject);
            }
            s_Instance.targetList.Clear();
        }
    }

    IEnumerator GenerateNextCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (currentTargetIndex != 0)
        {
            currentTargetIndex--;

            // show new target
            targetList[currentTargetIndex].gameObject.SetActive(true);

            ExperimentManager.OpenNewRecord(targetList[currentTargetIndex]);
        }
        s_Instance.targetList.RemoveAt(currentTargetIndex);

        // task end
        if (targetList.Count == 0)
        {
            ExperimentManager.EndExperiment();
        }

        yield return null;
    }

    public static void NextTarget()
    {
        IEnumerator coroutine = s_Instance.GenerateNextCoroutine ();
        s_Instance.StartCoroutine(coroutine);
    }

    public static bool EliminateTarget(int targetIndex)
    {
        if (ExperimentManager.State != ExperimentManager.ExperimentState.Performing) return false;

        if (targetIndex >= s_Instance.targetList.Count) return false;

        ExperimentManager.CloseRecord();

        return true;
    }

    void TurnAnchorsIntoData()
    {
        // turn targetAnchorList into TaskData.PositionList
    }
}
