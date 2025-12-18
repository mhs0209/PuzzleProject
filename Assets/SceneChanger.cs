using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    //public SceneAsset sceneAsset;

    public void SceneChange(SceneAsset sceneAsset)
    {
        SceneManager.LoadScene(sceneAsset.name);
    }
}
