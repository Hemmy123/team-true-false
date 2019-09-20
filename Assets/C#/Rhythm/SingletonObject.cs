using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Add this component to a gameobject to prevent multiple copies, provide global access and optionally prevent destruction

public class SingletonObject : MonoBehaviour
{
    //Static list of all singleton objects
    static Dictionary<string, GameObject> s_singletons;
    bool m_containedInDictionary = false;

    //Member to identify which object this is
    [Tooltip("A string identifying this object. Each different singleton should have a different string.")]
    [SerializeField] string m_identity = "CHANGE THIS VALUE";
    [SerializeField] bool m_dontDestroyOnLoad = false;

    //Update static list on creation of new singleton
    private void Awake()
    {
        if(s_singletons == null)
        {
            s_singletons = new Dictionary<string, GameObject>();
        }

        if(s_singletons.ContainsKey(m_identity))
        {
            Destroy(this);
        }
        else
        {
            s_singletons.Add(m_identity, gameObject);
            m_containedInDictionary = true;
            if(m_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    //Update static list on destruction
    private void OnDestroy()
    {
        if (m_containedInDictionary)
        {
            s_singletons.Remove(m_identity);
        }
    }

    //Access a singleton object
    public static GameObject GetSingleton(string key)
    {
        return s_singletons[key];
    }

}
