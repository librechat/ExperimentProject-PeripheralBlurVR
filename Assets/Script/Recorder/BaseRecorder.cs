using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Threading;
using ThreadPriority = System.Threading.ThreadPriority;

public class BaseRecorder : MonoBehaviour {
	
	[SerializeField]
	public string recordName;
	
	protected List<BaseRecordData> m_ClipList;
    protected List<string> m_StringList;
	// public ArrayList trackerArray = new ArrayList(100000);
	
    public void InitRecord()
    {
        m_StringList = new List<string>();
    }


    public virtual void Load(string fileName){
		
	}

    public void Save()
    {
        string dateformat = "yyyyMMdd-HHmm";
        string filename = System.DateTime.Now.ToString(dateformat);

        string condition = ExperimentManager.ConditionName;

        string filePath = Application.streamingAssetsPath + "/Behaviors/" + filename + "_" + condition + "_" + recordName + ".txt";

        using (StreamWriter outputFile = new StreamWriter(filePath))
        {
            for (int i = 0; i < m_StringList.Count; i++) outputFile.WriteLine(m_StringList[i]);
        }
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