using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class InputRecorder : BaseRecorder {

    public bool startBtn;
    public bool pauseBtn;
    public bool resumeBtn;
    public bool quitBtn;

    public bool discomfortConfirmBtn;
    public bool spatialConfirmBtn;
    public Vector2 moveAxis;

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

                    InputRecordData data = new InputRecordData(index, timeStamp);

                    string[] s = elements[2].Trim().Split('/');
                    data.startBtn = bool.Parse(s[0]);
                    data.pauseBtn = bool.Parse(s[1]);
                    data.resumeBtn = bool.Parse(s[2]);
                    data.quitBtn = bool.Parse(s[3]);

                    data.discomfortConfirmBtn = bool.Parse(s[4]);
                    data.spatialConfirmBtn = bool.Parse(s[5]);

                    float x = Convert.ToSingle(s[6]);
                    float y = Convert.ToSingle(s[7]);
                    data.moveAxis = new Vector2(x,y);

                    m_ClipList.Add(data);
                }
            }
        }
    }
	
	public override void Record(int currentClip){
        // record controller action state as string
        string s = string.Format("{8}#{9}#{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}",
            startBtn,
            pauseBtn,
            resumeBtn,
            quitBtn,
            discomfortConfirmBtn,
            spatialConfirmBtn,

            moveAxis.x,
            moveAxis.y,

            currentClip,
            Time.time);

        m_StringList.Add(s);
	}
	
	public override void Play(int currentClip){
        // read from array
        InputRecordData data = m_ClipList[currentClip] as InputRecordData;
        
	}
}

public class InputRecordData : BaseRecordData
{
    public bool startBtn;
    public bool pauseBtn;
    public bool resumeBtn;
    public bool quitBtn;

    public bool discomfortConfirmBtn;
    public bool spatialConfirmBtn;
    public Vector2 moveAxis;

    public InputRecordData(int idx, float time)
    {
        index = idx;
        timeStamp = time;

        discomfortConfirmBtn = false;
        spatialConfirmBtn = false;
        Vector2 moveAxis = Vector2.zero;        
    }
}