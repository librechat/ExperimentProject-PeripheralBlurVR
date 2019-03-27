using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptableObject/ConditionData ")]
public class ConditionData : ScriptableObject {

    public enum ConditionEnum {
        Baseline = 0,
        ConditionA,
        ConditionB
    };


    public enum WindowPositionEnum
    {
        Static = 0,
        Dynamic
    };
    public enum WindowSizeEnum
    {
        Small,
        Medium,
        Large
    };
    public enum ImageSourceEnum
    {
        CG,
        Video360
    };

    public bool BlurInPeripheral;
    public WindowSizeEnum WindowSize;
    public WindowPositionEnum WindowPosition;
    public ImageSourceEnum ImageSource;
}

[CreateAssetMenu(menuName = "MyScriptableObject/WindowSizeConfig ")]
public class WindowSizeConfig : ScriptableObject
{
    [System.Serializable]
    public struct Radius
    {
        public ConditionData.WindowSizeEnum Name;
        public float Inner;
        public float Outer;
    }

    public InputManager.HmdType HmdType;
    public List<Radius> RadiusList;
}