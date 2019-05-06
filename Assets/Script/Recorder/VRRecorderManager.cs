using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRRecorderManager: MonoBehaviour
{
	
	public enum RecorderMode{
		None,
		IsRecording,
		IsPlaying		
	}    
    
    [Header("Recording Parameter")]
    [SerializeField]
	private RecorderMode mode = RecorderMode.None;
    public RecorderMode Mode
    {
        get { return mode; }
        set { mode = value; }
    }

    [SerializeField]
	private int fps = 60;
    public int FPS
    {
        get { return fps; }
        set { clipInterval = 1.0f / (float)value; fps = value; }
    }
    private float clipInterval;
	
	[SerializeField]
	private string loadFileName;
	
	[Header("Tracked Object Reference")]
	[SerializeField]
	private List<BaseRecorder> m_RecorderList;
	
	private float timer = 0.0f;
	private float globalTimer = 0.0f;
	private int currentClip = 0;

    private void Awake()
    {
        timer = 0.0f;
        globalTimer = 0.0f;
        currentClip = 0;

        if (mode == RecorderMode.IsPlaying)
        {
            // XRSettings.enabled = false;
            // UnityEngine.XR.InputTracking.disablePositionalTracking = true;

            InputManager.Hardware = InputManager.HmdType.Recorder;
            // load record file
            for (int i = 0; i < m_RecorderList.Count; i++) m_RecorderList[i].Load(loadFileName);
        }
        else if (mode == RecorderMode.IsRecording)
        {
            XRSettings.enabled = true;
            for (int i = 0; i < m_RecorderList.Count; i++) m_RecorderList[i].InitRecord();
        }
    }

    public void Stop()
    {
        if (mode == RecorderMode.IsRecording) {
            for (int i = 0; i < m_RecorderList.Count; i++)
            {
                m_RecorderList[i].Record(currentClip);
                m_RecorderList[i].Save();
            }
        }

        mode = RecorderMode.None;
    }
	
	public void _Update(float timeStep){

        if (mode == RecorderMode.None) return;

		timer += timeStep;
        globalTimer += timeStep;
		
		if(timer > clipInterval){
			if(mode == RecorderMode.IsRecording) Record();
			else if(mode == RecorderMode.IsPlaying) Play();
			
			timer = 0.0f;
			currentClip++;
		}		
	}
	
	void Record(){
		for(int i=0; i<m_RecorderList.Count; i++) m_RecorderList[i].Record(currentClip);
	}
	
	void Play(){
		for(int i=0; i<m_RecorderList.Count; i++) m_RecorderList[i].Play(currentClip);
	}
}