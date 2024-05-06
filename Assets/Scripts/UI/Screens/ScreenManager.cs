using UnityEngine;



public enum UIScreen
{
    SCREEN_EMPTY, // no UI on screen
    SCREEN_MAIN_MENU, // main menu
    SCREEN_CHECKERS, // checkers UI
    SCREEN_DECKERS, // Deckers cards UI
    SCREEN_GAME_OVER, // game over screen

    // add additional screens here...
}



public class ScreenManager : MonoBehaviour
{

    public static ScreenManager Instance;

    ScreenElement[] screenElements_;

    [SerializeField] private UIScreen startingScreen;
    public UIScreen currentScreen { get; private set; }



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of ScreenManager detected!");
            return;
        }

        Instance = this;

    }



    private void Start()
    {

        screenElements_ = FindObjectsOfType<ScreenElement>();

        QuickSwitchToScreen(startingScreen);

    }



    public void QuickSwitchToScreen(UIScreen screen)
    {

        foreach(ScreenElement element in screenElements_)
        {
            element.isOnScreen = (element.screen == screen);
        }

        currentScreen = screen;

    }



    public void SwitchToScreen(UIScreen screen)
    {

        if(currentScreen == screen) return;

        foreach(ScreenElement element in screenElements_)
        {
            if(element.screen == screen)
                element.EnterTransition();
            else
                element.ExitTransition();
        }

        currentScreen = screen;

    }

}