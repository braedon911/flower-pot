using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartScreen : MonoBehaviour
{
    public UnityEvent startActivated;
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            startActivated.Invoke();
        }
    }
}
