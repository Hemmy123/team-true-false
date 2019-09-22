﻿using System.Collections;
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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(1);
        }
        //else if(Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    SceneManager.LoadScene(2);
        //}
        //else if(Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    SceneManager.LoadScene(3);
        //}
        //else if(Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    SceneManager.LoadScene(4);
        //}
        //else if(Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    SceneManager.LoadScene(5);
        //}
        //else if(Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    SceneManager.LoadScene(6);
        //}
        //else if(Input.GetKeyDown(KeyCode.Alpha7))
        //{
        //    SceneManager.LoadScene(7);
        //}
    }
}
