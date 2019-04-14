using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour {

    [Header("Task Options")]
    [SerializeField]
    public MazeTask CurrentTask;

    List<CollectTarget> CollectTargetList;
    private int currentCollectTargetIndex = -1;

    // List<SpatialTarget> SpatialTargetList;

    public static TaskManager s_Instance;

    void Awake () {

        if (s_Instance != null) Destroy(gameObject);
        s_Instance = this;
    }

    public static void Init(Transform playerController)
    {
        if (s_Instance.CollectTargetList == null) s_Instance.CollectTargetList = new List<CollectTarget>();
        else Clear();

        // init collect task
        s_Instance.CollectTargetList = s_Instance.CurrentTask.Init(playerController);

        for (int i = 0; i < s_Instance.CollectTargetList.Count; i++)
        {
            s_Instance.CollectTargetList[i].gameObject.SetActive(false);
        }
        s_Instance.currentCollectTargetIndex = s_Instance.CollectTargetList.Count;

        // init spatial task

        //for(int i = 0; i < s_Instance.)
    }

    public void update(float timestep)
    {

    }

    public static void Clear()
    {
        if (s_Instance.CollectTargetList != null && s_Instance.CollectTargetList.Count != 0)
        {
            for (int i = 0; i < s_Instance.CollectTargetList.Count; i++)
            {
                Destroy(s_Instance.CollectTargetList[i].gameObject);
            }
            s_Instance.CollectTargetList.Clear();
        }
    }

    IEnumerator GenerateNextCollectCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (currentCollectTargetIndex != 0)
        {
            currentCollectTargetIndex--;

            // show new target
            CollectTargetList[currentCollectTargetIndex].gameObject.SetActive(true);

            ExperimentManager.OpenNewRecord(CollectTargetList[currentCollectTargetIndex]);
        }
        s_Instance.CollectTargetList.RemoveAt(currentCollectTargetIndex);

        // task end
        if (CollectTargetList.Count == 0)
        {
            ExperimentManager.EndExperiment();
        }

        yield return null;
    }

    public static void NextTarget()
    {
        IEnumerator coroutine = s_Instance.GenerateNextCollectCoroutine ();
        s_Instance.StartCoroutine(coroutine);
    }

    public static bool EliminateTarget(int targetIndex)
    {
        Debug.Log(ExperimentManager.State);
        if (ExperimentManager.State != ExperimentManager.ExperimentState.Performing) return false;

        if (targetIndex > s_Instance.CollectTargetList.Count) return false;

        ExperimentManager.CloseRecord();

        return true;
    }
}
