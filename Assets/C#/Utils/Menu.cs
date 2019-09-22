using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            ChangeText();
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    [SerializeField] GameObject m_initialText;
    [SerializeField] GameObject m_helpText;

    void ChangeText()
    {
        m_initialText.SetActive(!m_initialText.activeSelf);
        m_helpText.SetActive(!m_helpText.activeSelf);
    }
}
