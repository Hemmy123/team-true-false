using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanging : MonoBehaviour
{
    //
    [SerializeField] string[] m_levels;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (m_levels.Length > 0)
            {
                SceneManager.LoadScene(m_levels[0]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (m_levels.Length > 1)
            {
                SceneManager.LoadScene(m_levels[1]);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (m_levels.Length > 2)
            {
                SceneManager.LoadScene(m_levels[2]);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (m_levels.Length > 3)
            {
                SceneManager.LoadScene(m_levels[3]);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (m_levels.Length > 4)
            {
                SceneManager.LoadScene(m_levels[4]);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (m_levels.Length > 5)
            {
                SceneManager.LoadScene(m_levels[5]);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (m_levels.Length > 6)
            {
                SceneManager.LoadScene(m_levels[6]);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (m_levels.Length > 7)
            {
                SceneManager.LoadScene(m_levels[7]);
            }
        }
    }
}
