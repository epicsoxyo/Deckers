using System;

using Unity.Netcode;

using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;



public enum GameScene
{
    MainMenu,
    Game,

    // add exact scene names here...
}



public class SceneLoader : MonoBehaviour
{

    public static SceneLoader Instance { get; private set;}



    private void Awake()
    {
        
        if(Instance != null)
        {
            Debug.LogWarning("Multiple SceneLoader instances detected!");
            return;
        }

        Instance = this;

    }



    public void LoadSceneLocally(string sceneName)
    {
        SceneManager.LoadScene(sceneName.ToString());
    }



    public void LoadSceneOverNetwork(string sceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

}