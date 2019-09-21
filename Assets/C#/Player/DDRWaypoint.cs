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
    [SerializeField] float m_duration = 1f;
    [SerializeField] bool m_hasFollowingWaypoint = true;
    [SerializeField] float m_hitTime = 0f;
    [SerializeField] float m_hitTolerance = 0.2f;
    [SerializeField] Direction m_direction = Direction.UP;

    //
    BeatManager m_beatManager;

    private void Start()
    {
        m_beatManager = SingletonObject.GetSingleton("BeatManager").GetComponent<BeatManager>();
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
        if(Mathf.Abs(beatProgress - m_hitTime) <= m_hitTolerance || Mathf.Abs(beatProgress - 1f - m_hitTime) <= m_hitTolerance)
        {
            move.ApplyTransition(m_nextWaypoint, m_duration, m_hasFollowingWaypoint);
            move.ddrCurrentDuration = m_beatManager.BeatsToTime(m_duration + m_hitTolerance);
            return true;
        }
        return false;
    }
}
