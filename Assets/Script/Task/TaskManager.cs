using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour {

    [Header("Task Options")]
    [SerializeField]
    public Task CurrentTask;

    List<TaskTarget> targetList;

    private int currentTargetIndex = -1;

    public static TaskManager s_Instance;

    void Awake () {

        if (s_Instance != null) Destroy(gameObject);
        s_Instance = this;
    }

    public static void Init(Transform playerController)
    {
        if (s_Instance.targetList == null) s_Instance.targetList = new List<TaskTarget>();
        else Clear();

        /*
        ======== Camera limits, depends on task and HMDType ========
        OVRManager cameraManager;
        cameraManager.usePositionTracking = s_Instance.currentTaskData.CameraPositionTrack;
        cameraManager.useRotationTracking = s_Instance.currentTaskData.CameraRotationTrack;

        OVRPlayerController player = playerController.GetComponent<OVRPlayerController>();
        player.EnableLinearMovement = s_Instance.currentTaskData.PlayerPositionCtrl;
        player.EnableRotation = s_Instance.currentTaskData.PlayerRotationCtrl;
        */
        
        // init task
        s_Instance.targetList = s_Instance.CurrentTask.Init(playerController);

        for (int i = 0; i < s_Instance.targetList.Count; i++)
        {
            s_Instance.targetList[i].gameObject.SetActive(false);
        }
        s_Instance.currentTargetIndex = s_Instance.targetList.Count;
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
        Debug.Log(ExperimentManager.State);
        if (ExperimentManager.State != ExperimentManager.ExperimentState.Performing) return false;

        if (targetIndex > s_Instance.targetList.Count) return false;

        ExperimentManager.CloseRecord();

        return true;
    }

    void TurnAnchorsIntoData()
    {
        // turn targetAnchorList into TaskData.PositionList
    }
}
