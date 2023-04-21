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
    public static Vector2Int anchorRoomCoords = Vector2Int.zero;
    protected override void OnPostprocessLevel(GameObject root, LdtkJson projectJson)
    {
        LDtkComponentLevel levelData = root.GetComponent<LDtkComponentLevel>();
        string name = root.name;
        string outputPath = "Assets/Scenes/" + name + ".unity";
        //scene already exists

        Scene scene = SceneManager.GetSceneByPath(outputPath);
        if (scene.IsValid())
        {
            if (!scene.isLoaded)
            {
                EditorSceneManager.OpenScene(outputPath);
            }
            //this is madness
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SetActiveScene(scene);
            GameObject.DestroyImmediate(GameObject.Find(name+"(Clone)"));
            GameObject.Instantiate(root, levelData.BorderRect.position, Quaternion.identity);
            EditorSceneManager.SaveScene(scene, outputPath);
            EditorSceneManager.CloseScene(scene, true);
        }
        //scene is new
        else
        {
            scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            EditorSceneManager.SetActiveScene(scene);
            scene.name = name;
            GameObject.Instantiate(root, levelData.BorderRect.position, Quaternion.identity);
            EditorSceneManager.SaveScene(scene, outputPath);
            EditorSceneManager.CloseScene(scene, true);
        }
    }
}
