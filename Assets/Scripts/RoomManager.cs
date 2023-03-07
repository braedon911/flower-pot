using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    //TODO keep track of a grid of rooms and load one room at a time
    [Header("Room transition")]
    public UnityEvent roomChangeBegin;
    public UnityEvent roomChangeEnd;
    [Range(0,90)]
    int transition_timer;

    //transition from one room to another
    IEnumerator Transition()
    {
        for (int i = 0; i < transition_timer; i++)
        {

            yield return null;
        }
    }
    void Start()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Persistent":
                SceneManager.LoadScene("Start");
                break;
        }
    }
}
