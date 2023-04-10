using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDtkUnity.Editor;
using LDtkUnity;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class LevelScenePacker : LDtkPostprocessor
{
    protected override void OnPostprocessLevel(GameObject root, LdtkJson projectJson)
    {
        LDtkComponentLevel levelData = root.GetComponent<LDtkComponentLevel>();
        string name = root.name;
        string outputPath = "Assets/Scenes/"+name+".unity";

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        scene.name = name;
        GameObject.Instantiate(root, levelData.BorderRect.position, Quaternion.identity);
        EditorSceneManager.SaveScene(scene, outputPath);
        EditorSceneManager.CloseScene(scene, true);
    }
}
