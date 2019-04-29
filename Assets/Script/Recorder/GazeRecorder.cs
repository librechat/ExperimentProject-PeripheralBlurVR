using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Threading;
using ThreadPriority = System.Threading.ThreadPriority;

public class GazeRecorder : BaseRecorder {
	
	public override void Load(string fileName){
        m_ClipList = new List<BaseRecordData>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName + "_" + name + ".txt");

        if (new FileInfo(filePath).Exists == false) return;

        using (FileStream fs = File.OpenRead(filePath))
        {
            using (StreamReader sr = new StreamReader(fs))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {

                    string[] elements = line.Trim().Split('#');
                    int index = Int.Parse(elements[0]);
                    float timeStamp = Convert.ToSingle(elements[1]);

                    // elements[2]

                    m_ClipList.Add(new GazeRecorderData(index, timeStamp));
                }
            }
        }
    }
	
	public override void Record(int currentClip){
		// record controller action state as string
        string s = string.Format("{16}#{17}#{0}",
            currentClip,
            Time.time);

        m_StringList.Add(s);
	}
	
	public override void Play(int currentClip){
        // read from array
        GazeRecorderData data = m_ClipList[currentClip] as GazeRecorderData;
        
	}
}

public class GazeRecorderData : BaseRecordData
{
    public GazeRecorderData(int idx, float time)
    {
        index = idx;
        timeStamp = time;
    }
}