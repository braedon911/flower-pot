using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ActorSolidSystem;
public class RefreshSystemOnSceneLoad : MonoBehaviour
{
    /// <summary>
    /// this script exists entirely to externally add a dependency between scenemanager and boxsystem
    /// </summary>
    private void Awake()
    {
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => { BoxSystem.CullNullBoxes(); };
    }
    
}
