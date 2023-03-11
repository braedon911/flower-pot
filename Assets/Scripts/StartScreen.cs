using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartScreen : MonoBehaviour
{
    bool ready = true;
    public UnityEvent startActivated;
    void Update()
    {
        if(ready && Input.GetButtonDown("Fire1"))
        {
            ready = false;
            startActivated.Invoke();
        }
    }
}
