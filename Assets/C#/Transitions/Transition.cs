using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    //
    Vector3 m_destinationPos, m_destinationRot, m_destinationScale;
    Vector3 m_originPos, m_originRot, m_originScale;
    float m_remainingTime = 0f;
    float m_duration = 1f;
    bool m_reachedDestination = true;

    //
    [SerializeField] float m_gravity = 0f;
    float m_parabolaU = 0f;

    //On update, set position according to transition
    private void FixedUpdate()
    {
        if(m_remainingTime > 0f)
        {
            float time = m_duration - m_remainingTime;
            float lerpParameter = time / m_duration;
            transform.position = Vector3.Lerp(m_originPos, m_destinationPos, lerpParameter) + Vector3.up * time * (m_parabolaU - 0.5f * time * m_gravity);
            transform.eulerAngles = Vector3.Lerp(m_originRot, m_destinationRot, lerpParameter);
            transform.localScale = Vector3.Lerp(m_originScale, m_destinationScale, lerpParameter);
            m_remainingTime -= Time.fixedDeltaTime;
        }
        else if (!m_reachedDestination)
        {
            TransitionFinished();
            transform.position = m_destinationPos;
            transform.eulerAngles = m_destinationRot;
            transform.localScale = m_destinationScale;
        }
    }

    void TransitionFinished()
    {
        m_reachedDestination = true;
    }

    public void StartTransition(Transform destination, float duration)
    {
        m_reachedDestination = false;

        m_originPos = transform.position;
        m_originRot = transform.eulerAngles;
        m_originScale = transform.localScale;

        m_destinationPos = destination.position;
        m_destinationRot = destination.eulerAngles;
        m_destinationScale = destination.localScale;

        m_remainingTime = duration;
        m_duration = duration;

        m_parabolaU = m_gravity * m_duration * 0.5f;
    }

}
