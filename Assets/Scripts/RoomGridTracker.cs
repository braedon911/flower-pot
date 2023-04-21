using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomGridTracker : MonoBehaviour
{
    Vector2Int coords;
    public Vector2Int Position { get => coords; }
    public int X { get => coords.x; }
    public int Y { get => coords.y; }

    [HideInInspector] public UnityEvent roomChange = new UnityEvent();
    private void Update()
    {
        Vector2Int previous = coords;
        coords = RoomManager.WorldToRoomCoords(transform.position);
        if(coords!=previous)
        {
            roomChange.Invoke();
        }
    }
}
