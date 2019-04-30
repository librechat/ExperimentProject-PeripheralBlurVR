using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.IO;
using System.Threading;
using ThreadPriority = System.Threading.ThreadPriority;

public class GazeRecorder : BaseRecorder {

    public Vector2 RawGazePointLeft { get { return rawGazePointLeft; } }
    public Vector2 RawGazePointRight { get { return rawGazePointRight; } }
    public Vector2 RawGazePointCenter { get { return rawGazePointCenter; } }

    private Vector2 rawGazePointLeft;
    private Vector2 rawGazePointRight;
    private Vector2 rawGazePointCenter;

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
                    int index = int.Parse(elements[0]);
                    float timeStamp = Convert.ToSingle(elements[1]);

                    string[] s = elements[2].Trim().Split('/');
                    
                    float x = Convert.ToSingle(s[0]);
                    float y = Convert.ToSingle(s[1]);
                    Vector2 left = new Vector2(x, y);

                    x = Convert.ToSingle(s[2]);
                    y = Convert.ToSingle(s[3]);
                    Vector2 right = new Vector2(x, y);

                    x = Convert.ToSingle(s[4]);
                    y = Convert.ToSingle(s[5]);
                    Vector2 center = new Vector2(x, y);

                    m_ClipList.Add(new GazeRecorderData(index, timeStamp, left, right, center));
                }
            }
        }
    }
	
	public override void Record(int currentClip){
		// record controller action state as string
        Vector2 left = GazeInputManager.GazePointLeft;
        Vector2 right = GazeInputManager.GazePointRight;
        Vector2 center = GazeInputManager.GazePointCenter;

        string s = string.Format("{16}#{17}#{0}/{1}/{2}/{3}/{4}/{5}",
            left.x,
            left.y,
            right.x,
            right.y,
            center.x,
            center.y,
            currentClip,
            Time.time);

        m_StringList.Add(s);
	}
	
	public override void Play(int currentClip){
        // read from array
        GazeRecorderData data = m_ClipList[currentClip] as GazeRecorderData;

        rawGazePointCenter = data.rawGazePointCenter;
        rawGazePointLeft = data.rawGazePointLeft;
        rawGazePointRight = data.rawGazePointRight;
	}
}

public class GazeRecorderData : BaseRecordData
{
    public Vector2 rawGazePointLeft;
    public Vector2 rawGazePointRight;
    public Vector2 rawGazePointCenter;
    
    public GazeRecorderData(int idx, float time, Vector2 left, Vector2 right, Vector2 center)
    {
        index = idx;
        timeStamp = time;

        rawGazePointCenter = center;
        rawGazePointLeft = left;
        rawGazePointRight = right;
    }
}