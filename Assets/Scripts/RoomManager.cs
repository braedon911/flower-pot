using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    //TODO keep track of a grid of rooms and load one room at a time
    public UnityEvent roomChangeBegin;
    public UnityEvent roomChangeEnd;

    int timer;

    //transition from one room to another
    IEnumerator Transition()
    {
        for (int i = 0; i < timer; i++)
        {

            yield return null;
        }
    }
    private void Update()
    {
        
    }
}
