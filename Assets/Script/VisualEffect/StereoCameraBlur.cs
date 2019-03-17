using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StereoCameraBlur : MonoBehaviour {

    [System.Serializable]
    struct BlurredCamera
    {
        public CameraBlurPost camera;
        public Material material;
    };
    
    [SerializeField]
    bool BlurEnabled = true;
    [SerializeField]
    BlurredCamera[] bCameras;
    
    [Header ("Shader Coefficients")]
    [SerializeField]
    bool indicated = true;
    [SerializeField]
    Color indicatedColor = new Color(1, 0, 0, 0);
    private Color hiddenColor = new Color(0, 0, 0, 0);
    [SerializeField]
    int KernelRadius = 5;
    [SerializeField]
    int MaskFactor = 450;
    [SerializeField]
    float CoeffStep = 0.32f;
    [SerializeField]
    float InnerRadius = 0.15f;
    [SerializeField]
    float OuterRadius = 0.35f;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < bCameras.Length; i++)
        {
            bCameras[i].camera.enabled = BlurEnabled;
            bCameras[i].material.SetFloat("_CoeffStep", CoeffStep);
            bCameras[i].material.SetInt("_MaskFactor", MaskFactor);
            bCameras[i].material.SetInt("_KernelRadius", KernelRadius);
            bCameras[i].material.SetFloat("_InnerRadius", InnerRadius);
            bCameras[i].material.SetFloat("_OuterRadius", OuterRadius);

            if (!indicated) bCameras[i].material.SetColor("_IndicatorColor", hiddenColor);
            else bCameras[i].material.SetColor("_IndicatorColor", indicatedColor);
        }
        if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetButtonDown("Oculus_GearVR_X"))
        {
            indicated = !indicated;
        }
        if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetButtonDown("Oculus_GearVR_Y"))
        {
            BlurEnabled = !BlurEnabled;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            RandomArray(18);
        } 
	}

    void RandomArray(int count) {
        int[] array = new int[count];
        for (int i = 0; i < count; i++) {
            array[i] = i+1;
        }
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(i, count);
            int tmp = array[index];
            array[index] = array[i];
            array[i] = tmp;
        }
        string str = "";
        for (int i = 0; i < count; i++)
        {
            str += array[i] + "/";
        }
        Debug.Log("count: "+count+"==== result: "+str);
        return;
    }
}
