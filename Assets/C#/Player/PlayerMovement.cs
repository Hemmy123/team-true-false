using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //
    enum PlayerState
    {
        GROUNDED,
        AIRBORNE,
        DDR,
        TRANSITIONING
    }
    PlayerState m_state = PlayerState.GROUNDED;

    //
    Rigidbody2D m_rb;
    BeatManager m_beatManager;
    Transition m_transitioner;
    Collider2D m_collider2D;

    //
    [SerializeField] LayerMask m_groundLayer;

    //
    [SerializeField] KeyCode m_leftKey = KeyCode.A;
    [SerializeField] KeyCode m_rightKey = KeyCode.D;
    [SerializeField] KeyCode m_upKey = KeyCode.W;
    [SerializeField] KeyCode m_downKey = KeyCode.S;

    [SerializeField] float m_acceleration = 5f; //scaled by (bpm / 120f)
    [SerializeField] float m_speed = 5f;        //scaled by (bpm / 120f)
    [SerializeField] float m_dragGrounded = 10f;//scaled by (bpm / 120f)
    [SerializeField] float m_baseJumpSpeed = 5f;
    [SerializeField] float m_jumpAssistGravityReduction = 0.25f;
    float m_gravity = 0f;

    //
    SuperJumpZone m_currentZone = null;

    //
    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_beatManager = SingletonObject.GetSingleton("BeatManager").GetComponent<BeatManager>();
        m_transitioner = GetComponent<Transition>();
        m_collider2D = GetComponent<Collider2D>();

        m_gravity = m_rb.gravityScale;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_state == PlayerState.TRANSITIONING && !m_transitioner.transitioning)
        {
            m_state = PlayerState.GROUNDED;
            m_rb.gravityScale = m_gravity;
            m_collider2D.enabled = true;
        }
        CheckIfLanded();

        switch (m_state)
        {
            case PlayerState.GROUNDED:
                GroundedMovement();
                break;
            case PlayerState.AIRBORNE:
                AirborneMovement();
                break;
            case PlayerState.DDR:
                DDRMovement();
                break;
            default:
                break;
        }
    }

    void HorizontalMovement()
    {
        float modifier = m_beatManager.currentBPM / 120f;
        if (float.IsNaN(modifier))
            return;

        float speed = 0f;
        if (Input.GetKey(m_leftKey))
        {
            speed -= m_speed;
        }
        if (Input.GetKey(m_rightKey))
        {
            speed += m_speed;
        }
        speed *= modifier;

        if (Mathf.Abs(m_rb.velocity.x) > m_speed * m_beatManager.currentBPM / 120f)
        {
            //If above max speed, slow down
            m_rb.velocity = new Vector2(m_rb.velocity.x - Mathf.Sign(m_rb.velocity.x) * m_dragGrounded * modifier, m_rb.velocity.y);
        }
        else
        {
            //Otherwise, set speed
            m_rb.velocity = new Vector2(speed, m_rb.velocity.y);
        }
    }

    //
    void GroundedMovement()
    {
        HorizontalMovement();

        //Jump stuff
        if(Input.GetKey(m_upKey))
        {
            m_state = PlayerState.AIRBORNE;

            if (m_currentZone != null && m_currentZone.CorrectBeat())
            {
                m_currentZone.ApplyJump(m_rb, this);
            }
            else
            {
                m_rb.velocity = new Vector2(m_rb.velocity.x, m_baseJumpSpeed);
            }
        }
    }

    void AirborneMovement()
    {
        HorizontalMovement();

        if(Input.GetKey(m_upKey) && m_rb.velocity.y > 0f)
        {
            m_rb.gravityScale = m_gravity * m_jumpAssistGravityReduction;
        }
        else
        {
            m_rb.gravityScale = m_gravity;
        }
    }
    void CheckIfLanded()
    {
        if ((m_rb.velocity.y <= 0f || m_state == PlayerState.GROUNDED) && m_state != PlayerState.DDR && m_state != PlayerState.TRANSITIONING)
        {
            if (Physics2D.Raycast(transform.position + Vector3.right * 0.5f, Vector2.down, .75f, m_groundLayer) || Physics2D.Raycast(transform.position - Vector3.right * 0.5f, Vector2.down, .75f, m_groundLayer))
            {
                m_state = PlayerState.GROUNDED;
                m_rb.gravityScale = m_gravity;
            }
            else
            {
                m_state = PlayerState.AIRBORNE;
            }
        }
    }

    //
    void DDRMovement()
    {

    }

    public void ApplyTransition(Transform waypoint, float beatDuration)
    {
        m_transitioner.StartTransition(waypoint, m_beatManager.BeatsToTime(beatDuration));
        m_state = PlayerState.TRANSITIONING;
        m_rb.gravityScale = 0f;
        m_collider2D.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var v = collision.GetComponent<SuperJumpZone>();
        if(v != null)
        {
            m_currentZone = v;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var v = collision.GetComponent<SuperJumpZone>();
        if(v == m_currentZone)
        {
            m_currentZone = null;
        }
    }
}
