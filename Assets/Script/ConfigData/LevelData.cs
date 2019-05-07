using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptableObject/LevelData ")]
public class LevelData : ScriptableObject
{

    public enum LevelEnum
    {
        Level_A = 0,
        Level_B,
        Level_C
    };

    public LevelEnum levelName;
    public string fileName;
    public MazeBuilder.Vector2i mapSize;
    public MazeBuilder.Vector2i entryPos;
    public float entryRotation;
}
