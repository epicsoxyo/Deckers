using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public enum GamePieceOverlay
{

    Null,
    GreetingsCard,

    // add game piece overlays here...

}



public class GamePieceOverlayManager : MonoBehaviour
{

    private Image _image;
    
    [SerializeField] private Sprite _greetingsCardOverlay;



    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.enabled = false;
    }


    
    public void ToggleOverlayOn(GamePieceOverlay overlay)
    {

        _image.enabled = true;

        switch(overlay)
        {
            case GamePieceOverlay.GreetingsCard:
                _image.sprite = _greetingsCardOverlay;
                return;
        }

        Debug.LogWarning($"Overlay \"{overlay}\" does not have an image!");

    }

    public void ToggleOverlayOff()
    {
        _image.enabled = false;
    }

}
