using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour {


    public float RotationAmount = 1.5f;
    private float RotationScaleMultiplier = 1.0f;
    private float SimulationRate = 60f;


    void FixedUpdate () {
        UpdateRotation();
	}

    void UpdateRotation()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        float rotateInfluence = SimulationRate * Time.deltaTime * RotationAmount * RotationScaleMultiplier;

        Vector2 secondaryAxis = InputManager.GetMoveAxis();
        euler.y += secondaryAxis.x * rotateInfluence;

        transform.rotation = Quaternion.Euler(euler);
    }
}
