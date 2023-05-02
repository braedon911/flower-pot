using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOverworld : MonoBehaviour
{
    public void RequestLoadOverworld()
    {
        GameObject.FindObjectOfType<RoomManager>().LoadOverworld();
    }
}
