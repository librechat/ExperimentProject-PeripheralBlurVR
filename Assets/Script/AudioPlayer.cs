using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {

    public enum AudioName
    {
        Discomfort = 0,
        Spatial,
        Done,
        Collected
    }

    [SerializeField]
    List<AudioClip> audioClipList;
    AudioSource audioSource;

    List<int> pendingList;

    public static AudioPlayer s_Instance;

    void Awake()
    {
        if (s_Instance != null) Destroy(gameObject);
        s_Instance = this;

        audioSource = GetComponent<AudioSource>();
        pendingList = new List<int>();
    }

    private void Update()
    {
        if(pendingList.Count > 0 && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(audioClipList[pendingList[0]]);
            pendingList.RemoveAt(0);
        }
    }

    public static void Play(AudioName audio)
    {
        int index = (int)audio;

        if (!s_Instance.audioSource.isPlaying)
        {
            s_Instance.audioSource.PlayOneShot(s_Instance.audioClipList[index]);
        }
        else
        {
            s_Instance.pendingList.Add(index);
        }
    }
}
