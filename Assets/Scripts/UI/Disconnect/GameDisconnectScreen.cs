using UnityEngine;
using UnityEngine.UI;



public class GameDisconnectScreen : MonoBehaviour
{

    public static GameDisconnectScreen Instance { get; private set;}

    private CanvasGroup _canvasGroup;
    private Button _exitButton;



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of GameDisconnectScreen detected!");
            return;
        }

        Instance = this;

        _canvasGroup = GetComponent<CanvasGroup>();
        ActivateDisconnectScreen(false);

        _exitButton = GetComponentInChildren<Button>();
        _exitButton.onClick.AddListener(OnExit);

    }



    private void Start()
    {

        if(DeckersNetworkManager.Instance == null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        DeckersNetworkManager.Instance.OnClientDisconnectCallback += OnClientDisconnectCallback;

    }




    private void OnClientDisconnectCallback(ulong obj)
    {
        ActivateDisconnectScreen();
    }



    public void ActivateDisconnectScreen(bool isActive = true)
    {
        _canvasGroup.alpha = isActive ? 1f : 0f;
        _canvasGroup.interactable = isActive;
        _canvasGroup.blocksRaycasts = isActive;
    }



    private void OnExit()
    {
        SceneLoader.Instance.LoadSceneLocally(GameScene.MainMenu);
        _exitButton.onClick.RemoveListener(OnExit);
        DeckersNetworkManager.Instance.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    }

}