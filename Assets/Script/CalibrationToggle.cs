using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PupilLabs;

public class CalibrationToggle : MonoBehaviour
{
    public PupilLabs.CalibrationController calibrationController;

    [Header("Reference")]
    [SerializeField]
    Camera camera;
    [SerializeField]
    VisaulEffectManager visaulEffectManager;
    [SerializeField]
    GameObject playGround;

    private void Awake()
    {
        camera.clearFlags = CameraClearFlags.SolidColor;
        playGround.SetActive(false);
    }

    void OnEnable()
    {
        calibrationController.OnCalibrationSucceeded += AfterCalibration;
    }

    void OnDisable()
    {
        calibrationController.OnCalibrationSucceeded -= AfterCalibration;
    }

    void AfterCalibration()
    {
        visaulEffectManager.Init();
        camera.clearFlags = CameraClearFlags.Skybox;
        playGround.SetActive(true);

        ExperimentManager.StartExperiment();
    }
}
