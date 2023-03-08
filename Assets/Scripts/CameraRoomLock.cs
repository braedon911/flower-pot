using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraRoomLock : MonoBehaviour
{
    public GameObject playerToFollow;
    void Update()
    {
     	Vector3 playerPosition = playerToFollow.transform.position;
	int mod_x = ((int)playerPosition.x - 64) / (int)128;
	int mod_y = ((int)playerPosition.y + 64) / (int)128;

	transform.position = new Vector3(mod_x * 128, mod_y * 128, transform.position.z); 
    }
	void Awake(){
		SceneManager.sceneLoaded += FindPlayer;
	}
	void FindPlayer(Scene scene, LoadSceneMode mode){
		playerToFollow = GameObject.Find("Tree Guy") ?? null;
	}
}
