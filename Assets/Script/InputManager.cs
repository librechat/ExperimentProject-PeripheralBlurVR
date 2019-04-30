using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

public class InputManager: MonoBehaviour{

    public enum HmdType
    {
        Vive,
        Oculus,
        Recorder,

        None
    };

    public static HmdType Hardware
    {
        get
        {
            if(hardware == HmdType.None)
            {
                string model = XRDevice.model != null ? XRDevice.model : "";
                if (model.IndexOf("Rift") >= 0)
                {
                    hardware = HmdType.Oculus;
                }
                else
                {
                    hardware = HmdType.Vive;
                }
            }
            return hardware;
        }
        set { hardware = value; }
    }
    private static HmdType hardware = HmdType.None;

    private InputRecorder m_Recorder;
    public static InputManager instance;

    void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;

        m_Recorder = GetComponent<InputRecorder>();
    }

    public static bool GetStartButton(){

        if (Hardware == HmdType.Recorder) {
            return instance.m_Recorder.startBtn;
        }

        bool result = false;

        if (Input.GetKeyDown(KeyCode.A)) result = true;
        // should be removed in formal study
        else if (Hardware == HmdType.Oculus) result = Input.GetButtonDown("Oculus_GearVR_A");
        else if (Hardware == HmdType.Vive) result = SteamVR_Actions._default.Confirm.GetStateUp(SteamVR_Input_Sources.Any);

        instance.m_Recorder.startBtn = result;
        return result;
    }

    public static bool GetPauseButton()
    {
        if (Hardware == HmdType.Recorder)
        {
            return instance.m_Recorder.pauseBtn;
        }

        bool result = Input.GetKeyDown(KeyCode.A);
        instance.m_Recorder.pauseBtn = result;
        return result;
    }

    public static bool GetResumeButton()
    {
        if (Hardware == HmdType.Recorder)
        {
            return instance.m_Recorder.resumeBtn;
        }

        bool result = Input.GetKeyDown("space");
        instance.m_Recorder.resumeBtn = result;
        return result;
    }
    public static bool GetQuitButton()
    {
        if (Hardware == HmdType.Recorder)
        {
            return instance.m_Recorder.quitBtn;
        }

        bool result = Input.GetKeyDown(KeyCode.Escape);
        instance.m_Recorder.quitBtn = result;
        return result;
    }

    //==== player's input ==== 
    public static bool GetDiscomfortConfirmButton()
    {
        if (Hardware == HmdType.Recorder)
        {
            return instance.m_Recorder.discomfortConfirmBtn;
        }

        bool result = Input.GetKeyDown(KeyCode.Q) || SteamVR_Actions._default.Confirm.GetStateUp(SteamVR_Input_Sources.Any);
        instance.m_Recorder.discomfortConfirmBtn = result;
        return result;
    }
    public static bool GetSpatialConfirmButton()
    {
        if (Hardware == HmdType.Recorder)
        {
            return instance.m_Recorder.spatialConfirmBtn;
        }

        // oculus
        bool result = false;
        if (Hardware == HmdType.Vive) result = SteamVR_Actions._default.Confirm.GetStateDown(SteamVR_Input_Sources.Any);

        instance.m_Recorder.spatialConfirmBtn = result;
        return result;
    }
    public static Vector2 GetMoveAxis()
    {
        if (Hardware == HmdType.Recorder)
        {
            return instance.m_Recorder.moveAxis;
        }

        Vector2 result = Vector2.zero;
        if (Hardware == HmdType.Vive) result = SteamVR_Actions._default.Move.GetAxis(SteamVR_Input_Sources.Any);

        instance.m_Recorder.moveAxis = result;
        return result;
    }
}
