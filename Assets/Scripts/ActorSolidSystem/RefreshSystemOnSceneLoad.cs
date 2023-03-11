using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ActorSolidSystem;
public class RefreshSystemOnSceneLoad : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => { BoxSystem.CullNullBoxes(); };
    }
    
}
