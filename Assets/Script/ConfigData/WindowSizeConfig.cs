using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptableObject/WindowSizeConfig ")]
public class WindowSizeConfig : ScriptableObject
{
    [System.Serializable]
    public struct Radius
    {
        public ConditionData.WindowSizeEnum Name;

        [Range(0.0f, 1.0f)]
        public float Inner;
        [Range(0.0f, 1.0f)]
        public float Outer;
    }

    public InputManager.HmdType HmdType;
    public List<Radius> RadiusList;
}
