using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperJumpZone : MonoBehaviour
{
    //
    public enum Type
    {
        Transition,
        Force
    }
    [SerializeField] Type m_type = Type.Transition;
    public Type type
    {
        get { return m_type; }
    }

    //
    [SerializeField] float m_period;
    [SerializeField] float m_beat;
    [SerializeField] float m_tolerance;
    [SerializeField] Transform m_waypoint;
    [SerializeField] float m_transitionBeatDuration;
    [SerializeField] Vector2 m_force;
    Transform waypoint
    {
        get { return m_waypoint; }
    }
    Vector2 force
    {
        get { return m_force; }
    }

    //
    BeatManager m_beatManager;

    private void Start()
    {
        m_beatManager = SingletonObject.GetSingleton("BeatManager").GetComponent<BeatManager>();
    }

    public bool CorrectBeat()
    {
        return m_beatManager.IsOnBeat(m_period, m_beat, m_tolerance);
    }

    public void ApplyJump(Rigidbody2D rb, PlayerMovement move)
    {
        switch (m_type)
        {
            case Type.Transition:
                move.ApplyTransition(m_waypoint, m_transitionBeatDuration);
                break;
            case Type.Force:
                rb.velocity += m_force;
                break;
        }
    }
}
