using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class InputRecorder : BaseRecorder {

    public bool StartBtn { get { return startBtn; } set { startBtn = value; } }
    public bool PauseBtn { get { return pauseBtn; } set { pauseBtn = value; } }
    public bool ResumeBtn { get { return resumeBtn; } set { resumeBtn = value; } }
    public bool QuitBtn { get { return quitBtn; } set { quitBtn = value; } }

    public bool DiscomfortConfirmBtn { get { return discomfortConfirmBtn; } set { discomfortConfirmBtn = value; } }
    public bool SpatialConfirmBtn { get { return spatialConfirmBtn; } set { spatialConfirmBtn = value; } }
    public Vector2 MoveAxis { get { return moveAxis; } set { moveAxis = value; } }
    public Vector2 RotAxis { get { return rotAxis; } set { rotAxis = value; } }

    public bool MoveFoward { get { return moveFoward; } set { moveFoward = value; } }
    public bool MoveBackward { get { return moveBackward; } set { moveBackward = value; } }
    public bool TurnRight { get { return turnRight; } set { turnRight = value; } }
    public bool TurnLeft { get { return turnLeft; } set { turnLeft = value; } }
    
    private bool startBtn;
    private bool pauseBtn;
    private bool resumeBtn;
    private bool quitBtn;

    private bool discomfortConfirmBtn;
    private bool spatialConfirmBtn;
    private Vector2 moveAxis;
    private Vector2 rotAxis;

    public bool moveFoward;
    public bool moveBackward;
    public bool turnRight;
    public bool turnLeft;

    public override void Load(string fileName){
        m_ClipList = new List<BaseRecordData>();
        string filePath = Application.streamingAssetsPath + "/Behaviors/" + fileName + "_" + recordName + ".txt";

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

                    x = Convert.ToSingle(s[8]);
                    y = Convert.ToSingle(s[9]);
                    data.rotAxis = new Vector2(x, y);

                    data.moveFoward = bool.Parse(s[10]);
                    data.moveBackward = bool.Parse(s[11]);
                    data.turnRight = bool.Parse(s[12]);
                    data.turnLeft = bool.Parse(s[13]);                   

                    m_ClipList.Add(data);
                }
            }
        }

    }
	
	public override void Record(int currentClip){
        // record controller action state as string
        string s = string.Format("{14}#{15}#{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}/{9}/{10}/{11}/{12}/{13}",
            startBtn,
            pauseBtn,
            resumeBtn,
            quitBtn,
            discomfortConfirmBtn,
            spatialConfirmBtn,

            moveAxis.x,
            moveAxis.y,

            rotAxis.x,
            rotAxis.y,

            moveFoward,
            moveBackward,
            turnRight,
            turnLeft,

            currentClip,
            Time.time);

        m_StringList.Add(s);

        if (startBtn) startBtn = false;
        if (pauseBtn) pauseBtn = false;
        if (resumeBtn) resumeBtn = false;
        if (quitBtn) quitBtn = false;
        if (discomfortConfirmBtn) discomfortConfirmBtn = false;
        if (spatialConfirmBtn) spatialConfirmBtn = false;
        if (moveAxis != Vector2.zero) moveAxis = Vector2.zero;
        if (rotAxis != Vector2.zero) rotAxis = Vector2.zero;
	}
	
	public override void Play(int currentClip){
        // read from array

        if (currentClip >= m_ClipList.Count) return;
        InputRecordData data = m_ClipList[currentClip] as InputRecordData;

        startBtn = data.startBtn;
        pauseBtn = data.pauseBtn;
        resumeBtn = data.resumeBtn;
        quitBtn = data.quitBtn;
        discomfortConfirmBtn = data.discomfortConfirmBtn;
        spatialConfirmBtn = data.spatialConfirmBtn;
        moveAxis = data.moveAxis;
        rotAxis = data.rotAxis;

        moveFoward = data.moveFoward;
        moveBackward = data.moveBackward;
        turnLeft = data.turnLeft;
        turnRight = data.turnRight;
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
    public Vector2 rotAxis;

    public bool moveFoward = false;
    public bool moveBackward = false;
    public bool turnRight = false;
    public bool turnLeft = false;

    public InputRecordData(int idx, float time)
    {
        index = idx;
        timeStamp = time;

        discomfortConfirmBtn = false;
        spatialConfirmBtn = false;
        Vector2 moveAxis = Vector2.zero;
        Vector2 rotAxis = Vector2.zero;
    }
}