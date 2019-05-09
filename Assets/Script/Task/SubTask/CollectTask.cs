using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectTask : BaseTask
{
    private bool catched = false;

    public long startTick;
    public long closedTick;

    private void Start()
    {
        startTick = DateTime.Now.Ticks;
    }

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
        closedTick = DateTime.Now.Ticks;

        bool result = CollectTaskManager.FinishTask(TaskIndex);
        if (result)
        {
            // show disappear effects
            // Destroy(gameObject);
            gameObject.SetActive(false);
            AudioPlayer.PlaySE(AudioPlayer.AudioName.Collected);
        }
    }
}
