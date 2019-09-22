using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform m_playerTransform;
    [SerializeField] Vector3 m_offset;
    [SerializeField] float m_baseSpeed = 5f;
    [SerializeField] float m_exponent = 3f;

    private void FixedUpdate()
    {
        Vector3 targetPosition = m_playerTransform.position + m_offset;
        float distance = (transform.position - targetPosition).magnitude;
        if (distance != 0)
        {
            transform.position += Time.fixedDeltaTime * (targetPosition - transform.position) * m_baseSpeed;
        }
    }
}
