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

    void FixedUpdate() {
        prevPos = transform.position;
        if (ExperimentManager.State == ExperimentManager.ExperimentState.Performing)
        {
            UpdateMovement2();
        }
    }

    void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.tag == "Wall")
        {
            transform.position = prevPos;
        }
    }

    void UpdateMovement()
    {
        Vector2 secondaryAxis = InputManager.GetMoveAxis();
        if (secondaryAxis.magnitude < 0.5f) return;

        Vector3 direction = new Vector3(hmd.forward.x, 0, hmd.forward.z);
        Vector3 euler = transform.rotation.eulerAngles;
        float rotateInfluence = SimulationRate * Time.deltaTime * RotationAmount * RotationScaleMultiplier;

        if (TranslationControllable && InputManager.GetMoveFoward())
        {
            RaycastHit hit;
            if (Physics.Raycast(hmd.position, hmd.forward, out hit, 0.3f))
            {
                if (hit.collider.tag == "Wall")
                {
                    return;
                }
            }

            transform.position += direction * MoveSpeed;
            return;
        }
        else if (RotationControllable && InputManager.GetTurnLeft())
        {
            transform.RotateAround(hmd.position, transform.up, -rotateInfluence);
            return;
        }
        else if (RotationControllable && InputManager.GetTurnRight())
        {
            transform.RotateAround(hmd.position, transform.up, rotateInfluence);
            return;
        }
        else if(TranslationControllable && InputManager.GetMoveBackward())
        {
            RaycastHit hit;
            if (Physics.Raycast(hmd.position, -hmd.forward, out hit, 0.3f))
            {
                if (hit.collider.tag == "Wall")
                {
                    return;
                }
            }

            transform.position -= direction * MoveSpeed;
            return;
        }
    }

    void UpdateMovement2()
    {
        Vector2 secondaryAxis = InputManager.GetMoveAxis();

        Vector3 direction = new Vector3(hmd.forward.x, 0, hmd.forward.z);
        Vector3 euler = transform.rotation.eulerAngles;
        float rotateInfluence = SimulationRate * Time.deltaTime * RotationAmount * RotationScaleMultiplier;

        transform.RotateAround(hmd.position, transform.up, rotateInfluence * secondaryAxis.x);
        transform.position += direction * MoveSpeed * secondaryAxis.y;
    }

    // deprecated
    bool UpdateRotation(Vector2 secondaryAxis)
    {
        Vector3 euler = transform.rotation.eulerAngles;
        float rotateInfluence = SimulationRate * Time.deltaTime * RotationAmount * RotationScaleMultiplier;
;
        /*float rotationAmount = InputManager.GetRotAxis().x;
        euler.y += rotationAmount * rotateInfluence;
        transform.rotation = Quaternion.Euler(euler);*/

        if (secondaryAxis.magnitude > 0.5f)
        {
            if (InputManager.GetTurnLeft())
            {
                transform.RotateAround(hmd.position, transform.up, -rotateInfluence);
                return true;
            }
            if (InputManager.GetTurnRight())
            {
                transform.RotateAround(hmd.position, transform.up, rotateInfluence);
                return true;
            }
        }

        return false;
    }
    // deprecated
    bool UpdateTransform(Vector2 secondaryAxis)
    {
        Vector3 direction = new Vector3(hmd.forward.x, 0, hmd.forward.z);

        RaycastHit hit;
        if (Physics.Raycast(hmd.position, hmd.forward, out hit, 0.3f))
        {
            if (hit.collider.tag == "Wall")
            {
                return false;
            }
        }

        if (secondaryAxis.magnitude > 0.5f)
        {
            if (InputManager.GetMoveFoward())
            {
                transform.position += direction * MoveSpeed;
                return true;
            }
            if (InputManager.GetMoveBackward())
            {
                transform.position -= direction * MoveSpeed;
                return true;
            }
        }

        return false;
    }
}
