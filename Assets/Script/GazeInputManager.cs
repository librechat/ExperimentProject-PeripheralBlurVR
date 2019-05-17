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

    public static Vector2 RawGazePointLeft
    {
        get { return s_Instance.rawGazePointLeft;}
    }
    public static Vector2 RawGazePointRight
    {
        get { return s_Instance.rawGazePointRight;}
    }
    public static Vector2 RawGazePointCenter
    {
        get { return s_Instance.rawGazePointCenter;}
    }

    private Vector2 gazePointLeft;
    private Vector2 gazePointRight;
    private Vector2 gazePointCenter;

    private Vector2 rawGazePointLeft;
    private Vector2 rawGazePointRight;
    private Vector2 rawGazePointCenter;

    private bool blink = false;

    private float speed = 2.0f;

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
        sceneCamera = VisaulEffectManager.SceneCamera;

        /* PupilData.calculateMovingAverage = false;
        PupilTools.OnConnected += StartSubscription;
        PupilTools.OnDisconnecting += StopSubscription;
        PupilTools.OnReceiveData += CustomReceiveData; */
    }

    void OnDisable()
    {
        /*
        PupilTools.OnConnected -= StartSubscription;
        PupilTools.OnDisconnecting -= StopSubscription;
        PupilTools.OnReceiveData -= CustomReceiveData;
        */
    }

    void StartSubscription()
    {
        /*
        PupilTools.IsGazing = true;
        PupilTools.SubscribeTo("gaze");

        
        PupilTools.SubscribeTo ("blinks");
        PupilTools.Send(new Dictionary<string, object> {
            { "subject", "start_plugin" }
            ,{ "name", "Blink_Detection" }
            ,{
                "args", new Dictionary<string,object> {
                    { "history_length", 0.2f }
                    ,{ "onset_confidence_threshold", 0.5f }
                    ,{ "offset_confidence_threshold", 0.5f }
                }
            }
        });
        */
    }

    void StopSubscription()
    {
        /*
        PupilTools.UnSubscribeFrom("gaze");
        PupilTools.IsGazing = false;
        
        PupilTools.Send(new Dictionary<string, object> {
            { "subject","stop_plugin" }
            ,{ "name", "Blink_Detection" }
        });

        PupilTools.UnSubscribeFrom("blinks");
        */
    }

    void CustomReceiveData(string topic, Dictionary<string, object> dictionary, byte[] thirdFrame = null)
    {
        if (topic == "blinks")
        {
            if (dictionary.ContainsKey("timestamp"))
            {
                // Debug.Log("Blink detected: " + dictionary["timestamp"].ToString());
                blink = true;
                return;
            }
        }
        blink = false;
        return;
    }

    void Update()
    {
        // consider not to adopt gaze position: confidence, blink
        
        /*
        
        if (blink) return;

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
        else return;

        if (usingRawSignal)
        {
            gazePointLeft = rawGazePointLeft;
            gazePointRight = rawGazePointRight;
            gazePointCenter = rawGazePointCenter;
        }
        else
        {
            UpdateGazePoint(rawGazePointLeft, gazePointLeft);
            UpdateGazePoint(rawGazePointRight, gazePointRight);
            UpdateGazePoint(rawGazePointCenter, gazePointCenter);
        }
        */
    }

    void UpdateGazePoint(Vector2 rawPoint, Vector2 gazePoint)
    {
        if (gazePoint == null) gazePoint = rawPoint;
        else
        {
            Vector2 delta = gazePoint - rawPoint;
            
            if (delta.magnitude < 0.05f)
            {
                // gazePoint = rawPoint;
                gazePoint = Vector2.Lerp(gazePoint, rawPoint, speed * Time.deltaTime);
            }
        }
    }
}
