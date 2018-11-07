using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviour {

    [SerializeField]
    List<Test.TrialList> model_list;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        OVRInput.FixedUpdate();
	}
    void Update() {
        OVRInput.Update();
    }
}
