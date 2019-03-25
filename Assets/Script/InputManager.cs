using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

public static class InputManager{

    public enum HmdType
    {
        Vive,
        Oculus,

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
    }
    private static HmdType hardware = HmdType.None;

    public static bool GetStartButton(){

        if (Input.GetKeyDown(KeyCode.A)) return true;

        if (Hardware == HmdType.Oculus) return Input.GetButtonDown("Oculus_GearVR_A");
        else if (Hardware == HmdType.Vive) return SteamVR_Actions._default.Confirm.GetStateUp(SteamVR_Input_Sources.Any);
        else return false;
    }

    public static bool GetPauseButton()
    {
        return Input.GetKeyDown(KeyCode.A);
    }

    public static bool GetResumeButton()
    {
        return Input.GetKeyDown("space");
    }

    public static bool GetQuitButton()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }

    public static bool GetLStickToLeft() {
        if (Hardware == HmdType.Oculus) return Input.GetAxis("Oculus_GearVR_LThumbstickX") < -0.5f;
        else if (Hardware == HmdType.Vive) return SteamVR_Actions._default.SliderDecrease.GetStateUp(SteamVR_Input_Sources.Any);
        else return false;
    }

    public static bool GetLStickToRight() {
        if (Hardware == HmdType.Oculus) return Input.GetAxis("Oculus_GearVR_LThumbstickX") > 0.5f;
        else if (Hardware == HmdType.Vive) return SteamVR_Actions._default.SliderIncrease.GetStateUp(SteamVR_Input_Sources.Any);
        else return false;
    }

    public static bool GetLStickConfirm(){
        if (Hardware == HmdType.Oculus) return Input.GetButtonUp("Oculus_GearVR_LThumbStick");
        else if (Hardware == HmdType.Vive) return SteamVR_Actions._default.Confirm.GetStateDown(SteamVR_Input_Sources.Any);
        else return false;
    }

    public static Vector2 GetMoveAxis()
    {
        if (Hardware == HmdType.Vive) return SteamVR_Actions._default.Move.GetAxis(SteamVR_Input_Sources.Any);
        else return Vector2.zero;
    }
}
