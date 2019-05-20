using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PupilLabs;

public class GazeInputManager : MonoBehaviour {

    [Header("Reference")]
    public SubscriptionsController subscriptionsController;
    public Transform cameraTransform;
    private Camera camera;

    private GazeRecorder m_Recorder;

    [Header("Gaze Tracking Settings")]
    [Range(0f, 1f)]
    public float confidenceThreshold = 0.6f;

    private GazeListener gazeListener = null;
    private bool isGazing = false;

    [Header("Gaze Data Handle Settings")]
    [SerializeField]
    private bool usingRawSignal = true;
    private float lerpSpeed = 2.0f;

    #region Data Accessors

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
    #endregion

    #region Raw Data

    private Vector2 gazePointCenter;
    private Vector2 gazePointLeft;
    private Vector2 gazePointRight;

    public Vector3 localGazeDirection;
    public Vector3 gazeNormalLeft, gazeNormalRight;
    public Vector3 eyeCenterLeft, eyeCenterRight;

    private bool blink = false;
    #endregion

    public static GazeInputManager s_Instance;    

    void Awake()
    {
        if (s_Instance != null) Destroy(gameObject);
        s_Instance = this;

        m_Recorder = GetComponent<GazeRecorder>();
        camera = cameraTransform.GetComponent<Camera>();
    }

    void OnEnable()
    {
        if (subscriptionsController == null) return;
        if (cameraTransform == null)
        {
            enabled = false;
            return;
        }
        if (gazeListener == null)
        {
            gazeListener = new GazeListener(subscriptionsController);
        }
        gazeListener.OnReceive3dGaze += ReceiveGaze;
        isGazing = true;
    }

    void OnDisable()
    {
        if (!isGazing) return;

        isGazing = false;
        if (gazeListener != null) gazeListener.OnReceive3dGaze -= ReceiveGaze;
    }

    void ReceiveGaze(GazeData gazeData)
    {
        if (gazeData.Confidence < confidenceThreshold) return;

        localGazeDirection = gazeData.GazeDirection;

        if (gazeData.IsEyeDataAvailable(0))
        {
            gazeNormalLeft = gazeData.GazeNormal0;
            eyeCenterLeft = gazeData.EyeCenter0;
        }
        if (gazeData.IsEyeDataAvailable(1))
        {
            gazeNormalRight = gazeData.GazeNormal1;
            eyeCenterRight = gazeData.EyeCenter1;
        }
    }

    void Update()
    {
        // consider not to adopt gaze position: confidence, blink
        
        if(!isGazing) return;

        // if (blink) return;

        if (InputManager.Hardware == InputManager.HmdType.Recorder)
        {
            localGazeDirection = m_Recorder.localGazeDirection;
            eyeCenterLeft = m_Recorder.eyeCenterLeft;
            eyeCenterRight = m_Recorder.eyeCenterRight;
            gazeNormalLeft = m_Recorder.gazeNormalLeft;
            gazeNormalRight = m_Recorder.gazeNormalRight;
        }

        Vector2 rawPointCenter = GetScreenPoint(localGazeDirection);
        Vector2 rawPointLeft = GetScreenPoint(eyeCenterLeft, gazeNormalLeft);
        Vector2 rawPointRight = GetScreenPoint(eyeCenterRight, gazeNormalRight);

        if (usingRawSignal)
        {
            gazePointCenter = rawPointCenter;
            gazePointLeft = rawPointLeft;
            gazePointRight = rawPointRight;
        }
        else
        {
            UpdateGazePoint(rawPointCenter, gazePointCenter);
            UpdateGazePoint(rawPointLeft, gazePointCenter);
            UpdateGazePoint(rawPointRight, gazePointCenter);
        }
    }

    Vector2 GetScreenPoint(Vector3 localDirection)
    {
        Vector3 origin = cameraTransform.position;
        Vector3 direction = cameraTransform.TransformDirection(localDirection);

        Vector2 point = camera.WorldToScreenPoint(origin + direction);       

        point.x /= camera.pixelWidth;
        point.y /= camera.pixelHeight;

        Debug.Log(point);

        return point;
    }
    Vector2 GetScreenPoint(Vector3 eyePos, Vector3 localDirection)
    {
        Vector3 origin = cameraTransform.position;
        Vector3 direction = cameraTransform.TransformDirection(localDirection);
        Vector3 pos = cameraTransform.TransformPoint(eyePos);

        return camera.WorldToScreenPoint(eyePos + direction);
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
                gazePoint = Vector2.Lerp(gazePoint, rawPoint, lerpSpeed * Time.deltaTime);
            }
        }
    }
}
