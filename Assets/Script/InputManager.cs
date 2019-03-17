using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager {

    public static bool GetStartButton(){
        return Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("Oculus_GearVR_A");
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
        return Input.GetAxis("Oculus_GearVR_LThumbstickX") < -0.5f;
    }

    public static bool GetLStickToRight() {
        return Input.GetAxis("Oculus_GearVR_LThumbstickX") > 0.5f;
    }

    public static bool GetLStickConfirm(){
        return Input.GetButtonUp("Oculus_GearVR_LThumbStick");
    }
}
