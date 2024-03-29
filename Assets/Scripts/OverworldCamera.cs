using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldCamera : MonoBehaviour
{
    [SerializeField] RoomGridTracker playerToFollow;
    int scrollSpeed = 1000;
    [SerializeField] RoomGridTracker roomGridTracker;
    Vector3 cameraOffset = 64 * (Vector3.up + Vector3.right) + (10*Vector3.back);
    RoomManager roomManager;

    public void AssignRoomManager(RoomManager roomManager)
    {
        this.roomManager = roomManager;
        roomManager.roomChangeBegin.AddListener(CallCameraPan);
    }
    private void Start()
    {
        scrollSpeed = roomManager.TransitionTimer;
    }

    bool cameraPanIsRunning = false;
    void CallCameraPan()
    {
        if (!cameraPanIsRunning)
        {
            StartCoroutine("CameraPan");
            cameraPanIsRunning = true;
        }
    }
    IEnumerator CameraPan()
    {
        Vector3 start = transform.position;
        Vector3 destination = RoomManager.RoomToWorldCoords(playerToFollow.Position) + cameraOffset;

        for (float i = 1; i <= scrollSpeed; i++)
        {
            transform.position = Vector3.Lerp(start, destination, i/scrollSpeed);
            yield return null;
        }
        cameraPanIsRunning = false;
    }
}
