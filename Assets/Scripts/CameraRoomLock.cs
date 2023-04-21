using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StateMachines;
using System;
using System.Threading.Tasks;

public class CameraRoomLock : MonoBehaviour
{
	GameObject playerToFollow;
	StateMachine stateMachine;
	[SerializeField, Range(0,2000)] int scrollSpeed = 1000;
	[SerializeField] RoomGridTracker roomGridTracker;
	void Update()
	{
		stateMachine.Execute();
    }
	//only pans up down left or right
	bool CameraPanIsRunning = false;
	Vector3 cameraOffset = 64*(Vector3.up + Vector3.right);
	IEnumerator CameraPan(Vector3 newPosition)
	{
		CameraPanIsRunning = true;
        Vector3 originalPosition = transform.position;
		
		if (originalPosition.x != newPosition.x)
		{
			int movement = Mathf.RoundToInt(newPosition.x - originalPosition.x);
			int speedCoef = Mathf.Abs(movement / scrollSpeed);
			int dir = Math.Sign(movement)*speedCoef;
			while(movement != 0)
			{
				transform.position += Vector3.right * dir;
				movement -= dir;
				yield return new WaitForSeconds(speedCoef);
			}
		}
		//this section is just a rote revision of the horizontal movement code
		else if (originalPosition.y != newPosition.y)
		{
            int movement = Mathf.RoundToInt(newPosition.y - originalPosition.y);
            int speedCoef = Mathf.Abs(movement / scrollSpeed);
            int dir = Math.Sign(movement) * speedCoef;
            while (movement != 0)
            {
                transform.position += Vector3.up * dir;
                movement -= dir;
                yield return new WaitForSeconds(speedCoef);
            }
        }
        CameraPanIsRunning = false;
    }
	void Awake(){
		SceneManager.sceneLoaded += FindPlayer;
		stateMachine = new StateMachine(PlayerNull, PlayerExists);
    }
	void FindPlayer(Scene scene, LoadSceneMode mode){
		playerToFollow = GameObject.Find("Tree Guy") ?? null;
		if (playerToFollow != null) stateMachine.ChangeState(PlayerExists);
		else stateMachine.ChangeState(PlayerNull);
	}
	void PlayerNull()
	{

    }
	void PlayerExists()
	{
		Vector2Int roomPosition = roomGridTracker.Position;
		Vector2Int roomPositionPlayer = playerToFollow.GetComponent<RoomGridTracker>().Position;
		if (roomPosition != roomPositionPlayer && !CameraPanIsRunning)
		{
			StartCoroutine(CameraPan(cameraOffset + (Vector3)(Vector2)(roomPosition*128)));
		}
    }
}
