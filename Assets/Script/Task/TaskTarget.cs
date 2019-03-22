using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTarget : MonoBehaviour {

    //[SerializeField]
    public int TargetIndex;

    private bool catched = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider) {

        
        if (collider.gameObject.tag == "Hand" && !catched && gameObject.tag != "Hand")
        {
            OnTargetCollected();
        }
    }

    void OnTargetCollected()
    {
        catched = true;

        bool result = TaskManager.EliminateTarget(TargetIndex);
        if (result)
        {
            
            // disappear effects
            Destroy(gameObject);
        }
    }
}
