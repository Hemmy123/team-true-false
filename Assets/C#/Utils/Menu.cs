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
    }

    [SerializeField] GameObject m_initialText;
    [SerializeField] GameObject m_helpText;

    void ChangeText()
    {
        m_initialText.SetActive(false);
        m_helpText.SetActive(true);
    }
}
