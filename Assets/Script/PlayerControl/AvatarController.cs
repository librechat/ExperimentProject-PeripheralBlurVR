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

    public Vector3 preHmdPosition;
    public float totalTravelDistance = 0.0f;

    public Quaternion preHmdRotation;
    public float totalRotation = 0.0f;

    private void Start()
    {
        preHmdPosition = hmd.position;
        preHmdRotation = hmd.rotation;
    }

    void FixedUpdate() {
        prevPos = transform.position;
        if (ExperimentManager.State == ExperimentManager.ExperimentState.Performing)
        {
            UpdateMovement2();

            totalTravelDistance += (hmd.position - preHmdPosition).magnitude;
            totalRotation += Mathf.Abs(Quaternion.Angle(preHmdRotation, hmd.rotation))/360.0f;

            preHmdPosition = hmd.position;
            preHmdRotation = hmd.rotation;
        }
    }

    void OnTriggerEnter(Collider collider)
    {

        /*if (collider.gameObject.tag == "Wall")
        {
            transform.position = prevPos;
        }*/
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
        float rotateInfluence = SimulationRate * Time.fixedDeltaTime * RotationAmount * RotationScaleMultiplier;

        if (RotationControllable)
        {
            //transform.RotateAround(hmd.position, Vector3.up, rotateInfluence * secondaryAxis.x);

            //transform.Rotate(0, rotateInfluence * secondaryAxis.x, 0);

            //Quaternion delta = Quaternion.Euler(0, rotateInfluence * secondaryAxis.x, 0);
            //transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * delta, 0.95f);

            euler.y += rotateInfluence * secondaryAxis.x;
            transform.rotation = Quaternion.Euler(euler);
        }
        if (TranslationControllable)
        {
            float speed = secondaryAxis.y;
            Vector3 forward = hmd.forward;
            if (speed < 0) forward = -hmd.forward;

            RaycastHit hit;
            if (Physics.Raycast(hmd.position, forward, out hit, 0.3f))
            {
                if (hit.collider.tag == "Wall")
                {
                    return;
                }
            }
            transform.position += direction * MoveSpeed * secondaryAxis.y;
            //Vector3 targetPosition = transform.position + direction * MoveSpeed * secondaryAxis.y;
            //transform.position = Vector3.Lerp(transform.position, targetPosition, 10 * Time.deltaTime);
        }
    }

    // deprecated
    bool UpdateRotation(Vector2 secondaryAxis)
    {
        Vector3 euler = transform.rotation.eulerAngles;
        float rotateInfluence = SimulationRate * Time.fixedDeltaTime * RotationAmount * RotationScaleMultiplier;
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
