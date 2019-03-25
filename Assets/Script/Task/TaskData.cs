using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptableObject/TaskData ")]
public class TaskData : ScriptableObject
{
    public enum TaskType
    {
        None = 0,
        Rotation,
        Translation,
        Multiple
    };

    public enum GenerationType
    {
        Random = 0,
        Predefined
    };

    public TaskType Task;
    public GenerationType Generation;
    public int NumOfTargets;
    public List<Vector3> PositionList;

    public bool CameraRotationTrack;
    public bool CameraPositionTrack;
    public bool PlayerRotationCtrl;
    public bool PlayerPositionCtrl;
}
