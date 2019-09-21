using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDRWaypoint : MonoBehaviour
{
    //
    [System.Serializable]
    enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
    [SerializeField] Transform m_nextWaypoint;
    [SerializeField] float m_travelDuration = 1f;
    [SerializeField] bool m_hasFollowingWaypoint = true;
    [SerializeField] float m_period = 1f;
    [SerializeField] float m_hitTime = 0f;
    [SerializeField] float m_hitTolerance = 0.2f;
    [SerializeField] Direction m_direction = Direction.UP;

    //
    BeatManager m_beatManager;
    [SerializeField] ParticleSystem m_particles;

    private void Start()
    {
        m_beatManager = SingletonObject.GetSingleton("BeatManager").GetComponent<BeatManager>();
    }

    private void FixedUpdate()
    {
        if(m_beatManager.IsOnBeat(m_period, m_hitTime))
        {
            m_particles.Play();
        }
    }

    public bool JumpLeft(PlayerMovement move)
    {
        if (m_direction == Direction.LEFT)
        {
            return Jump(move);
        }
        return false;
    }
    public bool JumpRight(PlayerMovement move)
    {
        if (m_direction == Direction.RIGHT)
        {
            return Jump(move);
        }
        return false;
    }
    public bool JumpUp(PlayerMovement move)
    {
        if (m_direction == Direction.UP)
        {
            return Jump(move);
        }
        return false;
    }
    public bool JumpDown(PlayerMovement move)
    {
        if (m_direction == Direction.DOWN)
        {
            return Jump(move);
        }
        return false;
    }

    public bool Jump(PlayerMovement move)
    {
        float beatProgress = m_beatManager.currentBeatProgress % 1f;
        if(m_beatManager.IsOnBeat(m_period, m_hitTime, m_hitTolerance))
        {
            move.ApplyTransition(m_nextWaypoint, m_travelDuration, m_hasFollowingWaypoint);
            move.ddrCurrentDuration = m_beatManager.BeatsToTime(m_travelDuration + m_hitTolerance);
            return true;
        }
        return false;
    }
}
