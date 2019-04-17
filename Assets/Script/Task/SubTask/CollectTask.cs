using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectTask : BaseTask
{
    private bool catched = false;

    public void update(float timestep)
    {
        
    }

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

        bool result = CollectTaskManager.FinishTask(TaskIndex);
        if (result)
        {
            // show disappear effects
            // Destroy(gameObject);
            gameObject.SetActive(false);
            AudioPlayer.Play(AudioPlayer.AudioName.Collected);
        }
    }
}
