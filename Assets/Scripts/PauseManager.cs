using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    public UnityEvent pause;
    public UnityEvent unpause;
    // Start is called before the first frame update
    bool paused = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (paused) { unpause.Invoke(); paused = false; }
            else { pause.Invoke(); paused = true; }
        }  
    }
}
