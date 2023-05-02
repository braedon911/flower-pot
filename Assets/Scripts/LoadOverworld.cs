using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOverworld : MonoBehaviour
{
    public void RequestLoadOveworld()
    {
        StartCoroutine(Delayed());
    }
    IEnumerator Delayed()
    {
        yield return new WaitForSecondsRealtime(1);
        GameObject.FindObjectOfType<RoomManager>().LoadOverworld();
    }
}
