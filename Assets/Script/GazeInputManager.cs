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

    private Vector2 rawGazePointLeft;
    private Vector2 rawGazePointRight;
    private Vector2 rawGazePointCenter;

    private GazeRecorder m_Recorder;
    public static GazeInputManager s_Instance;

    [SerializeField]
    private bool usingRawSignal = true;

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
            gazePointLeft = m_Recorder.RawGazePointLeft;
            gazePointRight = m_Recorder.RawGazePointRight;
            gazePointCenter = m_Recorder.RawGazePointCenter;
        }
        else if (PupilTools.IsConnected && PupilTools.IsGazing)
        {
            if (usingRawSignal)
            {
                gazePointLeft = PupilData._2D.GetEyePosition(sceneCamera, PupilData.leftEyeID);
                gazePointRight = PupilData._2D.GetEyePosition(sceneCamera, PupilData.rightEyeID);
                gazePointCenter = PupilData._2D.GazePosition;
            }
            else
            {
                rawGazePointLeft = PupilData._2D.GetEyePosition(sceneCamera, PupilData.leftEyeID);
                rawGazePointRight = PupilData._2D.GetEyePosition(sceneCamera, PupilData.rightEyeID);
                rawGazePointCenter = PupilData._2D.GazePosition;

                UpdateGazePoint(rawGazePointLeft, gazePointLeft);
                UpdateGazePoint(rawGazePointRight, gazePointRight);
                UpdateGazePoint(rawGazePointCenter, gazePointCenter);
            }
            
        }
    }

    void UpdateGazePoint(Vector2 rawPoint, Vector2 gazePoint)
    {
        if (gazePoint == null) gazePoint = rawPoint;
        else
        {
            Vector2 delta = gazePoint - rawPoint;
            
            if (delta.magnitude < 0.5f)
            {
                gazePoint = rawPoint;
            }
        }
    }
}
