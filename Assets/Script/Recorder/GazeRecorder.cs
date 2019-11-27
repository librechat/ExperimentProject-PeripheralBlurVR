using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.IO;
using System.Threading;
using ThreadPriority = System.Threading.ThreadPriority;

public class GazeRecorder : BaseRecorder {

    [SerializeField]
    GazeInputManager gazeInputManager;

    void Start()
    {
        /*string filePath = Application.streamingAssetsPath + "/Behaviors/Gaze/";
        DirectoryInfo dir = new DirectoryInfo(filePath);
        FileInfo[] info = dir.GetFiles("*.txt");

        foreach (FileInfo file in info)
        {
            string fileName = Path.GetFileNameWithoutExtension(file.ToString());
            Debug.Log(fileName);
            ConvertToScreen(fileName);
        }  */      
    }

    public Vector3 localGazeDirection;
    public Vector3 gazeNormalLeft, gazeNormalRight;
    public Vector3 eyeCenterLeft, eyeCenterRight;
    public bool blink;

    public override void Load(string fileName){
        m_ClipList = new List<BaseRecordData>();
        //string filePath = Application.streamingAssetsPath + "/Behaviors/" + fileName + "_" + recordName + ".txt";
        string filePath = Application.streamingAssetsPath + "/Behaviors/Gaze/" + fileName + ".txt";

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
        localGazeDirection = gazeInputManager.localGazeDirection;
        gazeNormalLeft = gazeInputManager.gazeNormalLeft;
        gazeNormalRight = gazeInputManager.gazeNormalRight;
        eyeCenterLeft = gazeInputManager.eyeCenterLeft;
        eyeCenterRight = gazeInputManager.eyeCenterRight;

        blink = gazeInputManager.blink;

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
        gazeNormalLeft = data.gazeNormalLeft;
        gazeNormalRight = data.gazeNormalRight;
        eyeCenterLeft = data.eyeCenterLeft;
        eyeCenterRight = data.eyeCenterRight;

        blink = data.blink;
    }

    void ConvertToScreen(string fileName)
    {
        Load(fileName);
        
        Transform cameraTransform = gazeInputManager.cameraTransform;
        Camera camera = cameraTransform.GetComponent<Camera>();

        List<string> s_List = new List<string>();
        s_List.Add("Index,Time,CenterX,CenterY,LeftX,LeftY,RightX,RightY,Angle,AngleX,AngleY");

        for (int i = 0; i < m_ClipList.Count; i++)
        {
            GazeRecorderData data = m_ClipList[i] as GazeRecorderData;

            // center
            Vector3 origin = cameraTransform.position;
            Vector3 direction = cameraTransform.TransformDirection(data.localGazeDirection);
            Vector2 pointCenter = camera.WorldToScreenPoint(origin + direction);
            pointCenter.x /= camera.pixelWidth;
            pointCenter.y /= camera.pixelHeight;

            // center angle
            float angle = Vector3.Angle(Vector3.forward, data.localGazeDirection); // serve as amplitude
            //Quaternion q = Quaternion.FromToRotation(Vector3.forward, data.localGazeDirection);
            //Vector3 v3Euler = q.eulerAngles;
            float angleX = Vector3.SignedAngle(Vector3.forward, new Vector3(data.localGazeDirection.x, 0, data.localGazeDirection.z), Vector3.up);
            float angleY = Vector3.SignedAngle(Vector3.forward, new Vector3(0, data.localGazeDirection.y, data.localGazeDirection.z), -Vector3.right);

            // left
            Vector3 pos = cameraTransform.TransformPoint(data.eyeCenterLeft);
            direction = cameraTransform.TransformDirection(data.gazeNormalLeft);
            Vector2 pointLeft = camera.WorldToScreenPoint(pos + direction, Camera.MonoOrStereoscopicEye.Left);
            pointLeft.x /= camera.pixelWidth;
            pointLeft.y /= camera.pixelHeight;

            // right
            pos = cameraTransform.TransformPoint(data.eyeCenterRight);
            direction = cameraTransform.TransformDirection(data.gazeNormalRight);
            Vector2 pointRight = camera.WorldToScreenPoint(pos + direction, Camera.MonoOrStereoscopicEye.Right);
            pointRight.x /= camera.pixelWidth;
            pointRight.y /= camera.pixelHeight;

            string s = string.Format("{9},{10},{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                pointCenter.x,
                pointCenter.y,
                pointLeft.x,
                pointLeft.y,
                pointRight.x,
                pointRight.y,
                angle,
                angleX,
                angleY,

                data.index,
                data.timeStamp);
            s_List.Add(s);
        }
        string filePath = Application.streamingAssetsPath + "/Behaviors/GazeScreen/" + fileName + ".csv";
        using (StreamWriter outputFile = new StreamWriter(filePath))
        {
            for (int i = 0; i < s_List.Count; i++) outputFile.WriteLine(s_List[i]);
        }
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