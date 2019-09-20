using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SingletonObject.GetSingleton("BeatManager").GetComponent<BeatManager>().beatEvent.AddListener(Tick);
        //SingletonObject.GetSingleton("BeatManager").GetComponent<BeatManager>().offBeatEvent.AddListener(OffBeatTick);
    }

    void Tick()
    {
        transform.position = -transform.position;
    }
    void OffBeatTick()
    {
        transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
    }
}
