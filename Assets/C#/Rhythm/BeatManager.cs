using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    //Reference to main music source (for timing)
    AudioSource m_audioSource;

    //Current music track
    [SerializeField] MusicWrapper m_music;

    //Events that are invoked on the beat
    UnityEvent m_beatEvent, m_offBeatEvent;
    public UnityEvent beatEvent
    {
        get { return m_beatEvent; }
    }
    public UnityEvent offBeatEvent
    {
        get { return m_offBeatEvent; }
    }

    //Initialize references and objects, start playing default track
    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_beatEvent = new UnityEvent();
        m_offBeatEvent = new UnityEvent();

        if(m_music != null)
        {
            PlayMusic(m_music);
        }
    }

    //Fires off beat events
    int m_lastBeat = 0;
    float m_currentBeatProgress = 0f;
    bool m_offbeat = false;

    float m_lastBeatTime = 0f;
    private void FixedUpdate()
    {

        //Fire off beat events if appropriate
        if(m_music != null)
        {
            var section = GetCurrentSection();
            if(section != null)
            {
                m_lastBeatTime = m_currentBeatProgress;
                m_currentBeatProgress = GetCurrentBeat(section);

                if (Mathf.FloorToInt(m_currentBeatProgress) != m_lastBeat)
                {
                    m_beatEvent.Invoke();
                    m_lastBeat = Mathf.FloorToInt(m_currentBeatProgress);
                    m_offbeat = true;
                }
                else if (Mathf.FloorToInt(m_currentBeatProgress + 0.5f) != m_lastBeat && m_offbeat)
                {
                    m_offBeatEvent.Invoke();
                    m_offbeat = false;
                }
            }
            else //No bpm defined for this section
            {
                m_currentBeatProgress = float.NaN;
            }
        }
        else //No music
        {
            m_currentBeatProgress = float.NaN;
        }
    }

    /// <summary>
    /// Returns a float which represents the current beat. Will be a whole number when exactly on the beat, or will end in .5 when exactly on an off-beat.
    /// </summary>
    /// <returns></returns>
    float GetCurrentBeat(MusicWrapper.BPMSection section)
    {
        float beatDuration = 60f / section.bpm;
        return (m_audioSource.time - section.startTime) / beatDuration;
    }
    float GetCurrentBeat()
    {
        return GetCurrentBeat(GetCurrentSection());
    }

    MusicWrapper.BPMSection GetCurrentSection()
    {
        float musicTime = m_audioSource.time;
        MusicWrapper.BPMSection[] sections = m_music.sections;

        MusicWrapper.BPMSection section = null;

        for (int i = 0; i < sections.Length; ++i)
        {
            if (musicTime >= sections[i].startTime && musicTime <= sections[i].endTime)
            {
                //Correct section
                section = sections[i];
                break;
            }
        }

        return section;
    }


    //Public methods

    public float TimeToNextBeat()
    {
        return Mathf.Ceil(m_currentBeatProgress) - m_currentBeatProgress;
    }
    public float TimeFromPreviousBeat()
    {
        return m_currentBeatProgress - Mathf.Floor(m_currentBeatProgress);
    }
    public float TimeFromClosestBeat()
    {
        return Mathf.Min(TimeToNextBeat(), TimeFromPreviousBeat());
    }
    public bool SongIsOver()
    {
        return !m_audioSource.isPlaying;
    }
    public float currentBeatProgress
    {
        get { return m_currentBeatProgress; }
    }
    public float currentBPM
    {
        get {
            var section = GetCurrentSection();
            if(section != null)
            {
                return GetCurrentSection().bpm;
            }
            else
            {
                return float.NaN;
            }
        }
    }
    public float BeatsToTime(float beats)
    {
        return beats * 60f / currentBPM;
    }
    public float TimeToBeats(float time)
    {
        return time * currentBPM / 60f;
    }

    //Returns true if the passed in beat was this frame
    public bool IsOnBeat(float period, float beat)
    {
        float beatFromBarStart = m_currentBeatProgress % period;
        float lastBeatFromBarStart = m_lastBeatTime % period;

        if(SongIsOver())
        {
            return false;
        }
        else if(beatFromBarStart > beat && lastBeatFromBarStart <= beat)  //Slightly dodgy 0.3f window but should work for our purposes so long as there are always at least 2 waypoints
        {
            return true;
        }
        else if (beatFromBarStart > beat && lastBeatFromBarStart > beatFromBarStart)
        {
            return true;
        }
        else
        {
            Debug.Log(beatFromBarStart + "," + lastBeatFromBarStart);
            return false;
        }
    }
    //
    public bool IsOnBeat(float period, float beat, float tolerance)
    {
        float beatFromBarStart = m_currentBeatProgress % period;
        if (Mathf.Abs(beatFromBarStart - beat) <= tolerance || Mathf.Abs(beatFromBarStart - period - beat) <= tolerance)
        {
            return true;
        }
        else
            return false;
    }

    //
    public void PlayMusic(MusicWrapper music)
    {
        m_music = music;
        m_audioSource.Stop();
        m_audioSource.clip = music.musicTrack;
        m_audioSource.Play();
    }
}
