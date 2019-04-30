using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeInputManager : MonoBehaviour {

    private Camera sceneCamera;
    private Vector3 standardViewportPoint = new Vector3(0.5f, 0.5f, 10);

    public static Vector2 GazePointLeft
    {
        get { return s_Instance.rawGazePointLeft; }
    }
    public static Vector2 GazePointRight
    {
        get { return s_Instance.rawGazePointRight; }
    }
    public static Vector2 GazePointCenter
    {
        get { return s_Instance.rawGazePointCenter; }
    }

    private Vector2 rawGazePointLeft;
    private Vector2 rawGazePointRight;
    private Vector2 rawGazePointCenter;

    private GazeRecorder m_Recorder;
    public static GazeInputManager s_Instance;

    void Awake()
    {
        if (s_Instance != null) Destroy(gameObject);
        s_Instance = this;

        m_Recorder = GetComponent<GazeRecorder>();
    }

    void Start()
    {
        PupilData.calculateMovingAverage = false;

        sceneCamera = VisaulEffectManager.SceneCamera;
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
        if (InputManager.Hardware == InputManager.HmdType.Recorder)
        {
            rawGazePointLeft = m_Recorder.RawGazePointLeft;
            rawGazePointRight = m_Recorder.RawGazePointRight;
            rawGazePointCenter = m_Recorder.RawGazePointCenter;
        }
        else if (PupilTools.IsConnected && PupilTools.IsGazing)
        {
            rawGazePointLeft = PupilData._2D.GetEyePosition(sceneCamera, PupilData.leftEyeID);
            rawGazePointRight = PupilData._2D.GetEyePosition(sceneCamera, PupilData.rightEyeID);
            rawGazePointCenter = PupilData._2D.GazePosition;
        }
    }
}
