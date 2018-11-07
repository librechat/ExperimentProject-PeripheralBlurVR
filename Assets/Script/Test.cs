using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    
    [System.Serializable]
    public struct TrialList {
        public GameObject model;
    }

    [SerializeField]
    List<Test.TrialList> model_list;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
