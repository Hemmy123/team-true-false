using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class is intended to wrap a music track and provide bpm information

[CreateAssetMenu(fileName = "NewMusicWrapper", menuName = "ScriptableObjects/MusicWrapper")]
public class MusicWrapper : ScriptableObject
{
    //
    [SerializeField] AudioClip m_musicTrack;
    public AudioClip musicTrack
    {
        get { return m_musicTrack; }
    }

    //
    [System.Serializable]
    public class BPMSection
    {
        public float startTime;
        public float endTime;
        public float bpm;
    }

    //
    [SerializeField] BPMSection[] m_sections;
    public BPMSection[] sections
    {
        get { return m_sections; }
    }
}
