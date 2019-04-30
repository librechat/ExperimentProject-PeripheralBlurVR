using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading;
using ThreadPriority = System.Threading.ThreadPriority;


public class TransformRecorder : BaseRecorder {

    public override void Load(string fileName)
    {
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

                    Matrix4x4 m = Matrix4x4.zero;
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            m[i, j] = Convert.ToSingle(s[i * 4 + j]);
                        }
                    }

                    m_ClipList.Add(new TransformRecordData(index, timeStamp, m));
                }
            }
        }
    }

	public override void Record(int currentClip){
		// get transfom matrix and turn to string
        var m = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        string s = string.Format("{16}#{17}#{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}/{9}/{10}/{11}/{12}/{13}/{14}/{15}",
            m[0, 0], m[0, 1], m[0, 2], m[0, 3],
            m[1, 0], m[1, 1], m[1, 2], m[1, 3],
            m[2, 0], m[2, 1], m[2, 2], m[2, 3],
            m[3, 0], m[3, 1], m[3, 2], m[3, 3],
            currentClip,
            Time.time);

        m_StringList.Add(s);
    }
	
	public override void Play(int currentClip){
        TransformRecordData data = m_ClipList[currentClip] as TransformRecordData;

        transform.position = data.position;
        transform.rotation = data.rotation;
        // transform.lossyScale = data.scale; // scale wont change during playing
    }
}

public class TransformRecordData: BaseRecordData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public TransformRecordData(int idx, float time, Matrix4x4 m)
    {
        index = idx;
        timeStamp = time;
        
        position = m.MultiplyPoint3x4(Vector3.zero);
        rotation = m.rotation;
        scale = m.lossyScale;
    }
}