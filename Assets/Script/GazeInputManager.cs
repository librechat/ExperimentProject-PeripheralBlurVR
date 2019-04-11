using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeInputManager : MonoBehaviour {

    private Camera sceneCamera;
    private Vector3 standardViewportPoint = new Vector3(0.5f, 0.5f, 10);

    public static Vector2 GazePointLeft
    {
        get { return s_Instance.gazePointLeft; }
    }
    public static Vector2 GazePointRight
    {
        get { return s_Instance.gazePointRight; }
    }
    public static Vector2 GazePointCenter
    {
        get { return s_Instance.gazePointCenter; }
    }

    private Vector2 gazePointLeft;
    private Vector2 gazePointRight;
    private Vector2 gazePointCenter;

    public static GazeInputManager s_Instance;

    void Awake()
    {
        if (s_Instance != null) Destroy(gameObject);
        s_Instance = this;
    }

    void Start()
    {
        PupilData.calculateMovingAverage = false;

        sceneCamera = gameObject.GetComponent<Camera>();
    }

    void OnEnable()
    {
        if (PupilTools.IsConnected)
        {
            PupilTools.IsGazing = true;
            PupilTools.SubscribeTo("gaze");
        }
    }

    void Update()
    {
        if (PupilTools.IsConnected && PupilTools.IsGazing)
        {
            gazePointLeft = PupilData._2D.GetEyePosition(sceneCamera, PupilData.leftEyeID);
            gazePointRight = PupilData._2D.GetEyePosition(sceneCamera, PupilData.rightEyeID);
            gazePointCenter = PupilData._2D.GazePosition;
        }
    }
}
