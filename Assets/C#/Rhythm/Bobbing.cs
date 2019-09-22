using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    //
    [SerializeField] float m_radianOffset = 0;
    [SerializeField] float m_period = 2f;
    [SerializeField] float m_xChange = 0.5f;
    [SerializeField] float m_yChange = 0.5f;

    //
    BeatManager m_beatManager;

    private void Start()
    {
        m_beatManager = SingletonObject.GetSingleton("BeatManager").GetComponent<BeatManager>();
    }

    void FixedUpdate()
    {
        float phase = (m_radianOffset + m_beatManager.currentBeatProgress / m_period * 2f) * Mathf.PI;
        float xScale = 1f + m_xChange * Mathf.Sin(phase);
        float yScale = 1f - m_yChange * Mathf.Sin(phase);

        transform.localScale = new Vector3(xScale, yScale, xScale);
    }
}
