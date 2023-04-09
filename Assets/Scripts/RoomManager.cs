using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    //TODO keep track of a grid of rooms and load one room at a time
    [Header("Room transition events")]
    public UnityEvent roomChangeBegin = new UnityEvent();
    public UnityEvent roomChangeEnd = new UnityEvent();
    [Range(0, 90), SerializeField]
    int transition_timer;
    //life is pain. i hate.

    const string firstRoom = "0,0";
    const string overworld = "Overworld";
    const string startScreen = "Start";

    string Room_current { get { return GetNameFromCoord(room_coord_x, room_coord_y); } } 
    int room_coord_x = 1;
    int room_coord_y = 0;

    GameObject player;
    GameObject cam;
    //transition from one room to another
    async void Transition()
    {
        roomChangeBegin.Invoke();
        for (int i = 0; i < transition_timer; i++)
        {
            await Task.Yield();
            
        }
        roomChangeEnd.Invoke();
    }
    void Start()
    {
        SceneManager.LoadScene(startScreen);
    }
    void Awake()
    {
        SceneManager.sceneLoaded += SceneChecker;
    }
    void SceneChecker(Scene scene, LoadSceneMode mode)
    {
        FindCamera(scene, mode);
        FindPlayer(scene, mode);
        switch (scene.name)
        {
            case startScreen:
                GameObject.Find("Tree Guy Prop")?.GetComponent<CutePlayerStartAnimation>().finishedAnimation.AddListener(LoadOverworld);
                break;
            case overworld:
                PlayerController playerController = player.GetComponent<PlayerController>();
                roomChangeBegin.AddListener(playerController.Suspend);
                roomChangeEnd.AddListener(playerController.Unsuspend);
                playerController.Suspend();
                break;
        }
    }
    void FindPlayer(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void FindCamera(Scene scene, LoadSceneMode mode)
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
    }
    public void LoadOverworld()
    {
        Debug.Log("Overworld loading");

        SceneManager.LoadScene(firstRoom, LoadSceneMode.Single);
        SceneManager.LoadScene(overworld, LoadSceneMode.Additive);
    }
    void Update()
    {
        if (player != null)
        {
            int room_coord_x_previous = room_coord_x;
            int room_coord_y_previous = room_coord_y;
            room_coord_x = ((int)player.transform.position.x - 64) / (int)128;
            room_coord_y = ((int)player.transform.position.y - 64) / (int)128;

            int delta_x = room_coord_x - room_coord_x_previous;
            int delta_y = room_coord_y - room_coord_y_previous;

            if (delta_x != 0 || delta_y != 0)
            {
                LoadRoom(room_coord_x, room_coord_y);
                Transition();
            }
        }
    }
    string GetNameFromCoord(int x, int y)
    {
        return $"{x},{y}";
    }
    void LoadRoom(int x, int y)
    {
        string previous = Room_current;
        string roomToLoad = GetNameFromCoord(x,y);
        SceneManager.LoadScene(roomToLoad, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(previous);
    }
}
