using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "MyScriptableObject/ExperimentData")]
public class ExperimentData : ScriptableObject
{
    [SerializeField]
    public int participant;

    [SerializeField, Range(1,3)]
    public int session = 1;

    [System.Serializable]
    public struct SessionSetting
    {
        public ConditionData.ConditionEnum condition;
        public LevelData.LevelEnum level;        
    }

    public SessionSetting[] SessionSettings = new SessionSetting[3];

    
}