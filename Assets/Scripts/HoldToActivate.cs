using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HoldToActivate : MonoBehaviour
{
    [SerializeField] string button = "Fire1";
    [SerializeField] int countUntil = 30;
    public int Progress { get; private set; }
    public float ProgressFraction { get { return (float)Progress / countUntil; } }
    public UnityEvent finished;
    public UnityEvent cancelled;
    public UnityEvent started;

    private void Update()
    {
        if (Input.GetButtonDown(button)) started.Invoke();
        if (Input.GetButton(button))
        {
            Progress++;
            if(Progress >= countUntil) { finished.Invoke(); }
        }
        if (Input.GetButtonUp(button))
        {
            Progress = 0;
            cancelled.Invoke();
        }
    }
    private void OnDisable()
    {
        if (Progress > 0) cancelled.Invoke();
        Progress = 0;
    }
}
