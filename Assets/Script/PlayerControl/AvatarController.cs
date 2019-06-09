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
            Vector2 secondaryAxis = InputManager.GetMoveAxis();
            Debug.Log(secondaryAxis);
            if (TranslationControllable)
            {
                if(secondaryAxis.x < 0.2f && secondaryAxis.x > -0.2f) UpdateTransform();
            }
            if (RotationControllable) {
                if (secondaryAxis.y < 0.2f && secondaryAxis.y > -0.2f) UpdateRotation();
            }
        }
	}

    void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.tag == "Wall")
        {
            transform.position = prevPos;
        }
    }

    bool UpdateRotation()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        float rotateInfluence = SimulationRate * Time.deltaTime * RotationAmount * RotationScaleMultiplier;
;
        /*float rotationAmount = InputManager.GetRotAxis().x;
        euler.y += rotationAmount * rotateInfluence;
        transform.rotation = Quaternion.Euler(euler);*/

        bool turnRight = InputManager.GetTurnRight();
        bool turnLeft = InputManager.GetTurnLeft();

        if (turnLeft && turnRight) return false;
        else if (turnLeft)
        {
            //euler.y -= rotateInfluence;
            //transform.rotation = Quaternion.Euler(euler);
            transform.RotateAround(hmd.position, transform.up, -rotateInfluence);
            return true;
        }
        else if (turnRight)
        {
            //euler.y += rotateInfluence;
            //transform.rotation = Quaternion.Euler(euler);
            transform.RotateAround(hmd.position, transform.up, rotateInfluence);
            return true;
        }
        else return false;
    }

    bool UpdateTransform()
    {
        Vector2 secondaryAxis = InputManager.GetMoveAxis();
        Vector3 direction = new Vector3(hmd.forward.x, 0, hmd.forward.z);

        RaycastHit hit;
        if (Physics.Raycast(hmd.position, hmd.forward, out hit, 0.3f))
        {
            if (hit.collider.tag == "Wall")
            {
                return false;
            }
        }

        bool moveForward = InputManager.GetMoveFoward();
        bool moveBackward = InputManager.GetMoveBackward();
        if (moveForward && moveBackward) return false;
        else if (moveForward)
        {
            transform.position += direction * MoveSpeed;
            return true;
        }
        else if (moveBackward)
        {
            transform.position -= direction * MoveSpeed;
            return true;
        }
        else return false;

        //transform.position += direction * secondaryAxis.magnitude * MoveSpeed;
    }
}
