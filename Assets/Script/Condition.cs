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

public static class WindowSizeConstants
{
    public struct RadiusSet
    {
        public float Small, Medium, Large;
        public RadiusSet(float s, float m, float l){
            Small = s;
            Medium = m;
            Large = l;
        }
    }
    public static RadiusSet InnerRadius = new RadiusSet(0.2f, 0.3f, 0.4f);
    public static RadiusSet OuterRadius = new RadiusSet(0.3f, 0.4f, 0.5f);
}