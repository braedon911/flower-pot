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

	void Update()
	{
		stateMachine.Execute();
    }
	//only pans up down left or right
	async void CameraPan(Vector3 newPosition)
	{
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
				await Task.Delay(speedCoef);
			}
		}
		else if (originalPosition.y != newPosition.y)
		{
            int movement = Mathf.RoundToInt(newPosition.y - originalPosition.y);
            int speedCoef = Mathf.Abs(movement / scrollSpeed);
            int dir = Math.Sign(movement) * speedCoef;
            while (movement != 0)
            {
                transform.position += Vector3.up * dir;
                movement -= dir;
                await Task.Delay(speedCoef);
            }
        }
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
        Vector3 playerPosition = playerToFollow.transform.position;
        int mod_x = ((int)playerPosition.x - 64) / 128;
        int mod_y = ((int)playerPosition.y + 64) / 128;
        //the /128 *128 seems redundant but it's a rounding thing for the room positions
        //CameraPan(new Vector3(mod_x * 128, mod_y * 128, transform.position.z));
    }
}
