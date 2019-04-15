using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectTask : BaseTask
{
    private bool catched = false;

    void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.tag == "Hand" && !catched && gameObject.tag != "Hand")
        {
            OnTargetCollected();
        }
    }

    void OnTargetCollected()
    {
        catched = true;

        bool result = CollectTaskManager.EliminateTarget(TaskIndex);
        if (result)
        {

            // disappear effects
            Destroy(gameObject);
        }
    }
}
