using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScheduler : MonoBehaviour
{
    //Reference to beatmanager and transition component
    BeatManager m_beatManager;
    Transition m_transition;

    //Struct to provide intermediate states
    [System.Serializable]
    struct Waypoint
    {
        public Transform transform;
        public float startBeat;
        public float endBeat;
    }
    
    //Properties
    [SerializeField] float m_period;
    [SerializeField] Waypoint[] m_waypoints;

    void Start()
    {
        m_beatManager = SingletonObject.GetSingleton("BeatManager").GetComponent<BeatManager>();
        m_transition = GetComponent<Transition>();
    }

    //
    int m_nextWaypoint = 0;
    private void FixedUpdate()
    {
        //Begin transitions at the appropriate time
        Waypoint w = m_waypoints[m_nextWaypoint];
        if (m_beatManager.IsOnBeat(m_period, w.startBeat))
        {
            m_transition.StartTransition(w.transform, m_beatManager.BeatsToTime(w.endBeat - w.startBeat));
            m_nextWaypoint = (m_nextWaypoint + 1) % m_waypoints.Length;
        }
    }
}
