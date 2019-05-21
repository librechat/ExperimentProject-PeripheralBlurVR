using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.IO;
using System.Threading;
using ThreadPriority = System.Threading.ThreadPriority;

public class GazeRecorder : BaseRecorder {

    public Vector3 localGazeDirection;
    public Vector3 gazeNormalLeft, gazeNormalRight;
    public Vector3 eyeCenterLeft, eyeCenterRight;
    public bool blink;

    public override void Load(string fileName){
        m_ClipList = new List<BaseRecordData>();
        string filePath = Path.Combine(Application.streamingAssetsPath, "/Behaviors/" + fileName + "_" + recordName + ".txt");

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

                    List<Vector3> vectorList = new List<Vector3>();
                    for (int i = 0; i < 15; i+=3)
                    {
                        float x = Convert.ToSingle(s[i]);
                        float y = Convert.ToSingle(s[i+1]);
                        float z = Convert.ToSingle(s[i+2]);

                       vectorList.Add(new Vector3(x, y, z));
                    }

                    bool blink = Convert.ToBoolean(s[15]);

                    m_ClipList.Add(new GazeRecorderData(index, timeStamp, vectorList, blink));
                }
            }
        }
    }
	
	public override void Record(int currentClip){
		// record controller action state as string
        localGazeDirection = GazeInputManager.s_Instance.localGazeDirection;
        gazeNormalLeft = GazeInputManager.s_Instance.gazeNormalLeft;
        gazeNormalRight = GazeInputManager.s_Instance.gazeNormalRight;
        eyeCenterLeft = GazeInputManager.s_Instance.eyeCenterLeft;
        eyeCenterRight = GazeInputManager.s_Instance.eyeCenterRight;

        blink = GazeInputManager.s_Instance.blink;

        string s = string.Format("{16}#{17}#{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}/{9}/{10}/{11}/{12}/{13}/{14}/{15}",
            localGazeDirection.x,
            localGazeDirection.y,
            localGazeDirection.z,

            gazeNormalLeft.x,
            gazeNormalLeft.y,
            gazeNormalLeft.z,
            
            gazeNormalRight.x,
            gazeNormalRight.y,
            gazeNormalRight.z,

            eyeCenterLeft.x,
            eyeCenterLeft.y,
            eyeCenterLeft.z,

            eyeCenterRight.x,
            eyeCenterRight.y,
            eyeCenterRight.z,

            blink,

            currentClip,
            Time.time);

        m_StringList.Add(s);
	}
	
	public override void Play(int currentClip){
        // read from array
        if (currentClip >= m_ClipList.Count) return;
        GazeRecorderData data = m_ClipList[currentClip] as GazeRecorderData;

        localGazeDirection = data.localGazeDirection;
	}
}

public class GazeRecorderData : BaseRecordData
{
    public Vector3 localGazeDirection;
    public Vector3 gazeNormalLeft, gazeNormalRight;
    public Vector3 eyeCenterLeft, eyeCenterRight;

    public bool blink;

    public GazeRecorderData(int idx, float time, List<Vector3> vectorList, bool blink)
    {
        index = idx;
        timeStamp = time;

        localGazeDirection = vectorList[0];
        gazeNormalLeft = vectorList[1];
        gazeNormalRight = vectorList[2];
        eyeCenterLeft = vectorList[3];
        eyeCenterRight = vectorList[4];

        this.blink = blink;
    }
}