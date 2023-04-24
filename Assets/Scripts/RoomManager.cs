using LDtkUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
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
    public int TransitionTimer { get { return transition_timer; } set { transition_timer = value; } }
    //life is pain. i hate.
    const string anchorRoom = "_0_0";
    const string firstRoom = "_0_1";
    const string overworld = "Overworld";
    const string startScreen = "Start";

    static int anchor_x = 0;
    static int anchor_y = 128;
    
    GameObject player;
    private RoomGridTracker playerGridTracker;

    public static Vector2Int WorldToRoomCoords(Vector3 worldCoords)
    {        
        return new Vector2Int((Mathf.FloorToInt(worldCoords.x)-anchor_x)/128, Mathf.Abs((Mathf.FloorToInt(worldCoords.y) - anchor_y - 128) /128));
    }
    public static Vector3 RoomToWorldCoords(Vector2Int roomCoords)
    {
        return new Vector3((roomCoords.x * 128) + anchor_x, (roomCoords.y * -128) + anchor_y);
    }
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
        AsyncOperation sceneLoader = SceneManager.LoadSceneAsync(anchorRoom, LoadSceneMode.Additive);
        sceneLoader.completed += EstablishAnchor;
    }
    public void RoomChangeRoutine()
    {
        LoadRoom(playerGridTracker.X, playerGridTracker.Y);
        Transition();
    }
    void EstablishAnchor(AsyncOperation operation)
    {
        Scene anchorRoomScene = SceneManager.GetSceneByName(anchorRoom);
        SceneManager.SetActiveScene(anchorRoomScene);
        LDtkComponentLevel level = GameObject.FindObjectOfType<LDtkComponentLevel>();
        SceneManager.UnloadSceneAsync(anchorRoomScene);
        Vector2Int anchorRoomCoords = new Vector2Int((int)level.BorderRect.position.x, (int)level.BorderRect.position.y);
        anchor_x = anchorRoomCoords.x;
        anchor_y = anchorRoomCoords.y;
        SceneManager.LoadScene(startScreen);
        Debug.Log($"Anchor established at {anchor_x}, {anchor_y}");
    }
    void Awake()
    {
        SceneManager.sceneLoaded += SceneChecker;
    }
    void SceneChecker(Scene scene, LoadSceneMode mode)
    {
        if (FindPlayer(scene, mode))
        {
            playerGridTracker = player.GetComponent<RoomGridTracker>();
            playerGridTracker.roomChange.AddListener(RoomChangeRoutine);
        }
        switch (scene.name)
        {
            case startScreen:
                GameObject.Find("Tree Guy Prop")?.GetComponent<CutePlayerStartAnimation>().finishedAnimation.AddListener(LoadOverworld);
                break;
            case overworld:
                PlayerController playerController = player.GetComponent<PlayerController>();
                roomChangeBegin.AddListener(playerController.Suspend);
                roomChangeEnd.AddListener(playerController.Unsuspend);

                GameObject.FindObjectOfType<OverworldCamera>().AssignRoomManager(this); 
                break;
        }
    }
    bool FindPlayer(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        return player != null;
    }
    public void LoadOverworld()
    {
        Debug.Log("Overworld loading");

        SceneManager.LoadScene(firstRoom, LoadSceneMode.Single);
        SceneManager.LoadScene(overworld, LoadSceneMode.Additive);
    }
    string GetNameFromCoord(int x, int y)
    {
        return $"_{x}_{y}";
    }
    void LoadRoom(int x, int y)
    {
        string roomToLoad = GetNameFromCoord(x,y);
        if (!SceneManager.GetSceneByName(roomToLoad).isLoaded)
        {
            SceneManager.LoadScene(roomToLoad, LoadSceneMode.Additive);
        }
    }
}
