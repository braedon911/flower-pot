using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMain : MonoBehaviour
{
    public void ReturnToSceneOne()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
