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
    public bool transitioning
    {
        get { return !m_reachedDestination; }
    }

    //Parabola parameters
    [SerializeField] float m_gravity = 0f;
    float m_parabolaU = 0f;

    //1D Nonlinear parameters
    [System.Serializable]
    enum NonLinearType
    {
        NONE,
        SmoothStart,
        SmoothStop
    }
    [SerializeField] NonLinearType m_type;
    [SerializeField] float m_smoothStartParam = 1f;
    [SerializeField] float m_smoothStopParam = 1f;

    //On update, set position according to transition
    private void FixedUpdate()
    {
        if(m_remainingTime > 0f)
        {
            float lerpParameter = (m_duration - m_remainingTime) / m_duration;
            //if(m_smoothStopParam != 0f || m_smoothStartParam != 0f)
            lerpParameter = SetNonLinearParameter(lerpParameter); 
            SetPositionByParameter(lerpParameter);
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

    float SetNonLinearParameter(float parameter)
    {
        //Mathf.Pow(lerpParameter, m_smoothStartParam) * (1f - Mathf.Pow(1f - lerpParameter, m_smoothStopParam));
        switch(m_type)
        {
            case NonLinearType.SmoothStart:
                return Mathf.Pow(parameter, m_smoothStartParam);
            case NonLinearType.SmoothStop:
                return 1f - Mathf.Pow(1f - parameter, m_smoothStopParam);
            case NonLinearType.NONE:
            default:
                return parameter;
        }
    }

    void SetPositionByParameter(float lerpParameter)
    {
        float time = lerpParameter * m_duration;
        transform.position = Vector3.Lerp(m_originPos, m_destinationPos, lerpParameter) + Vector3.up * time * (m_parabolaU - 0.5f * time * m_gravity);
        transform.eulerAngles = Vector3.Lerp(m_originRot, m_destinationRot, lerpParameter);
        transform.localScale = Vector3.Lerp(m_originScale, m_destinationScale, lerpParameter);
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
