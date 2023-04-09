using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StateMachines;
public class CameraRoomLock : MonoBehaviour
{
	GameObject playerToFollow;
	StateMachine stateMachine;

	void Update()
	{
		stateMachine.Execute();
    }
	void CameraPan(Vector3 newPosition)
	{

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
        CameraPan(new Vector3(mod_x * 128, mod_y * 128, transform.position.z));
    }
}
