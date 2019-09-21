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
        TRANSITIONING,
        FALLING
    }
    PlayerState m_state = PlayerState.GROUNDED;

    //
    Rigidbody2D m_rb;
    BeatManager m_beatManager;
    Transition m_transitioner;
    Collider2D m_collider2D;
    ParticleSystem m_particles;
    [SerializeField] Animator m_animator;
    AudioSource m_audioSource;

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
    DDRWaypoint m_currentWaypoint = null;
    float m_ddrCurrentDuration = 0f;
    public float ddrCurrentDuration
    {
        get { return m_ddrCurrentDuration; }
        set { m_ddrCurrentDuration = value; }
    }

    //
    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_beatManager = SingletonObject.GetSingleton("BeatManager").GetComponent<BeatManager>();
        m_transitioner = GetComponent<Transition>();
        m_collider2D = GetComponent<Collider2D>();
        m_particles = GetComponent<ParticleSystem>();
        m_audioSource = GetComponent<AudioSource>();

        m_gravity = m_rb.gravityScale;

    }

    // Update is called once per frame
    Vector3 m_previousPosition = Vector3.zero;
    void FixedUpdate()
    {
        if (m_state == PlayerState.TRANSITIONING && !m_transitioner.transitioning)
        {
            m_state = PlayerState.AIRBORNE;
            EnablePhysics();
        }
        else if(m_state == PlayerState.DDR)
        {
            if(m_ddrCurrentDuration <= 0f)
            {
                m_state = PlayerState.FALLING;
                EnablePhysics();
            }
            else
            {
                m_ddrCurrentDuration -= Time.fixedDeltaTime;
            }
        }

        if(m_state == PlayerState.AIRBORNE || m_state == PlayerState.GROUNDED || m_state == PlayerState.FALLING)
            CheckIfLanded();

        switch (m_state)
        {
            case PlayerState.GROUNDED:
                GroundedMovement();
                if (m_rb.velocity.x != 0f)
                {
                    SetRunning();
                }
                else
                {
                    SetIdle();
                }
                break;
            case PlayerState.AIRBORNE:
                AirborneMovement();
                if(m_rb.velocity.y > 0f)
                {
                    SetJumping();
                }
                else
                {
                    SetFalling();
                }
                break;
            case PlayerState.DDR:
                DDRMovement();
                SetDancing();
                break;
            case PlayerState.FALLING:
                SetFalling();
                break;
            case PlayerState.TRANSITIONING:
                SetFalling();
                break;
            default:
                break;
        }

        if(Input.GetKeyDown(m_rightKey))//m_previousPosition.x < transform.position.x)
        {
            FaceRight();
        }
        else if(Input.GetKeyDown(m_leftKey))//m_previousPosition.x > transform.position.x)
        {
            FaceLeft();
        }
        m_previousPosition = transform.position;
    }
    void FaceLeft()
    {
        Vector3 scale = m_animator.transform.localScale;
        m_animator.transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, -Mathf.Abs(scale.z));
    }
    void FaceRight()
    {
        Vector3 scale = m_animator.transform.localScale;
        m_animator.transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, Mathf.Abs(scale.z));
    }
    void SetFalling()
    {
        m_animator.SetBool("Falling", true);
        m_animator.SetBool("Jumping", false);
        m_animator.SetBool("Running", false);
        m_animator.SetBool("Dance", false);
    }
    void SetRunning()
    {
        m_animator.SetBool("Falling", false);
        m_animator.SetBool("Jumping", false);
        m_animator.SetBool("Running", true);
        m_animator.SetBool("Dance", false);
    }
    void SetJumping()
    {
        m_animator.SetBool("Falling", false);
        m_animator.SetBool("Jumping", true);
        m_animator.SetBool("Running", false);
        m_animator.SetBool("Dance", false);
        m_animator.transform.eulerAngles = new Vector3(0f, 90f, 0f);
    }
    void SetIdle()
    {
        m_animator.SetBool("Falling", false);
        m_animator.SetBool("Jumping", false);
        m_animator.SetBool("Running", false);
        m_animator.SetBool("Dance", false);
        m_animator.transform.eulerAngles = new Vector3(0f, 90f, 0f);
    }
    void SetDancing()
    {
        m_animator.SetBool("Falling", false);
        m_animator.SetBool("Jumping", false);
        m_animator.SetBool("Running", false);
        m_animator.SetBool("Dance", true);
        m_animator.transform.eulerAngles = new Vector3(0f, 180f, 0f);
    }

    void EnablePhysics()
    {
        m_rb.bodyType = RigidbodyType2D.Dynamic;
        m_rb.gravityScale = m_gravity;
    }
    void DisablePhysics()
    {
        m_rb.bodyType = RigidbodyType2D.Kinematic;
        m_rb.gravityScale = 0f;
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
            m_rb.velocity = new Vector2(m_rb.velocity.x - Mathf.Sign(m_rb.velocity.x) * m_dragGrounded * modifier * Time.fixedDeltaTime, m_rb.velocity.y);
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
        if(EnterDDR())
        {
            m_particles.Play();
            return;
        }

        HorizontalMovement();

        //Jump stuff
        if(Input.GetKey(m_upKey))
        {
            m_state = PlayerState.AIRBORNE;

            if (m_currentZone != null && m_currentZone.CorrectBeat())
            {
                m_currentZone.ApplyJump(m_rb, this);
                m_particles.Play();
            }
            else
            {
                m_rb.velocity = new Vector2(m_rb.velocity.x, m_baseJumpSpeed);
            }
        }
    }

    void AirborneMovement()
    {
        if (EnterDDR())
        {
            m_particles.Play();
            return;
        }

        HorizontalMovement();

        //if(Input.GetKey(m_upKey) && m_rb.velocity.y > 0f)
        //{
        //    m_rb.gravityScale = m_gravity * m_jumpAssistGravityReduction;
        //}
        //else
        //{
        //    m_rb.gravityScale = m_gravity;
        //}
    }
    void CheckIfLanded()
    {
        if ((m_rb.velocity.y <= 0f || m_state == PlayerState.GROUNDED))
        {
            if (Physics2D.Raycast(transform.position + Vector3.right * 0.4f, Vector2.down, .75f, m_groundLayer) || Physics2D.Raycast(transform.position - Vector3.right * 0.4f, Vector2.down, .75f, m_groundLayer))
            {
                m_state = PlayerState.GROUNDED;
            }
            else if (m_state == PlayerState.GROUNDED)
            {
                m_state = PlayerState.AIRBORNE;
            }
        }
    }

    bool EnterDDR()
    {
        if(m_currentWaypoint != null)
        {
            bool success = false;
            if(Input.GetKeyDown(m_upKey))
                success = m_currentWaypoint.JumpUp(this);
            if(Input.GetKeyDown(m_downKey))
                success = m_currentWaypoint.JumpDown(this);
            if (Input.GetKeyDown(m_leftKey))
                success = m_currentWaypoint.JumpLeft(this);
            if (Input.GetKeyDown(m_rightKey))
                success = m_currentWaypoint.JumpRight(this);

            if (success)
                m_audioSource.Play();
            return success;
        }
        return false;
    }

    //
    void DDRMovement()
    {
        if(EnterDDR())
            m_particles.Play(); ;
    }

    public void ApplyTransition(Transform waypoint, float beatDuration, bool ddr = false)
    {
        m_transitioner.StartTransition(waypoint, m_beatManager.BeatsToTime(beatDuration), false);
        if(ddr)
        {
            m_state = PlayerState.DDR;
        }
        else
        {
            m_state = PlayerState.TRANSITIONING;
        }
        DisablePhysics();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var v = collision.GetComponent<SuperJumpZone>();
        if(v != null)
        {
            m_currentZone = v;
        }
        var v2 = collision.GetComponent<DDRWaypoint>();
        if(v2 != null)
        {
            m_currentWaypoint = v2;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var v = collision.GetComponent<SuperJumpZone>();
        if(v == m_currentZone)
        {
            m_currentZone = null;
        }
        var v2 = collision.GetComponent<DDRWaypoint>();
        if (v2 == m_currentWaypoint)
        {
            m_currentWaypoint = null;
        }

        if (collision.tag == "Boundary" && m_state != PlayerState.TRANSITIONING)
        {
            Utils.RestartLevel();
        }
    }
}
