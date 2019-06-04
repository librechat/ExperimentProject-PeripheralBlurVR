using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour {

    [SerializeField]
    bool TranslationControllable;
    [SerializeField]
    bool RotationControllable;

    public float MoveSpeed = 1.0f;
    public float RotationAmount = 1.5f;
    private float RotationScaleMultiplier = 1.0f;
    private float SimulationRate = 60f;

    Vector3 prevPos;

    [SerializeField]
    Transform hmd;

    void FixedUpdate () {
        prevPos = transform.position;
        //UpdateRotation();
        if(ExperimentManager.State == ExperimentManager.ExperimentState.Performing)
        {
            if (TranslationControllable) UpdateTransform();
            if (RotationControllable) UpdateRotation();
        }
	}

    void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.tag == "Wall")
        {
            transform.position = prevPos;
        }
    }

    void UpdateRotation()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        float rotateInfluence = SimulationRate * Time.deltaTime * RotationAmount * RotationScaleMultiplier;
;
        float rotationAmount = InputManager.GetRotAxis().x;
        euler.y += rotationAmount * rotateInfluence;

        transform.rotation = Quaternion.Euler(euler);
    }

    void UpdateTransform()
    {
        Vector2 secondaryAxis = InputManager.GetMoveAxis();
        Vector3 direction = new Vector3(hmd.forward.x, 0, hmd.forward.z);

        RaycastHit hit;
        if (Physics.Raycast(hmd.position, hmd.forward, out hit, 0.3f))
        {
            if (hit.collider.tag == "Wall")
            {
                return;
            }
        }

        transform.position += direction * secondaryAxis.magnitude * MoveSpeed;
    }
}
