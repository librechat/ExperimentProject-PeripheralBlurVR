using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Threading;
using ThreadPriority = System.Threading.ThreadPriority;

public class BaseRecorder : MonoBehaviour {
	
	[SerializeField]
	public string name;
	
	protected List<BaseRecordData> m_ClipList;
	// public ArrayList trackerArray = new ArrayList(100000);
	
	public virtual void Load(string fileName){
		
	}
	
	public virtual void Record(int currentClip){
		
	}
	
	public virtual void Play(int currentClip){
		
	}
}

public class BaseRecordData
{
    public int index;
    public float timeStamp;
}