using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptableObject/ConditionData ")]
public class ConditionData : ScriptableObject {

    public enum ConditionEnum {
        Baseline = 0,
        Static_Small,
        Static_Medium,
        Static_Large,
        Dynamic_Small,
        Dynamic_Medium,
        Dynamic_Large
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