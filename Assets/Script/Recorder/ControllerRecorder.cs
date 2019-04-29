using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Threading;
using ThreadPriority = System.Threading.ThreadPriority;

public class ControllerRecorder : BaseRecorder {
	
	public override void Load(string fileName){
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName + "_" + name + ".txt");
        m_ClipList = new List<RecordData>();
    }
	
	public override void Record(int currentClip){
		// get transfom matrix and turn to string
		
		// record controller action state as string
	}
	
	public override void Play(int currentClip){
        // read from array

        
	}
}