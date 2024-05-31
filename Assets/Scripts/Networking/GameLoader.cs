using Unity.Netcode;

using UnityEngine;
using UnityEngine.SceneManagement;



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
        Instance = this;
    }



    public void LoadSceneLocally(GameScene sceneName)
    {
        SceneManager.LoadScene(sceneName.ToString());
    }



    public void LoadSceneOverNetwork(GameScene sceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName.ToString(), LoadSceneMode.Single);
    }

}