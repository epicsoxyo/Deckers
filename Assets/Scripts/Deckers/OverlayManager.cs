using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public enum Overlay
{
    Connect4,
    Joker,

    // add overlays here
}



public class OverlayManager : MonoBehaviour
{

    public static OverlayManager Instance { get; private set; }

    [SerializeField] private float transitionTime;

    [SerializeField] private CanvasGroup connect4Overlay;
    [SerializeField] private CanvasGroup jokerOverlay;



    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of OverlayManager detected!");
            return;
        }
        Instance = this;
    }



    public CanvasGroup GetOverlay(Overlay overlay)
    {
        switch(overlay)
        {
            case Overlay.Connect4:
                return connect4Overlay;
            case Overlay.Joker:
                return jokerOverlay;
            default:
            Debug.LogWarning(overlay + " was not assigned a canvas group and will return null.");
                return null;
        }
    }



    public CanvasGroup ToggleOverlayOn(Overlay overlay, bool isOn = true)
    {

        CanvasGroup canvasGroup = GetOverlay(overlay);
        if(canvasGroup == null) { return null; }

        StartCoroutine(OverlayTransition(canvasGroup, isOn));

        return canvasGroup;

    }



    private IEnumerator OverlayTransition(CanvasGroup overlayCanvasGroup, bool isOn)
    {
    
        ScreenManager.Instance.SwitchToScreen(UIScreen.SCREEN_EMPTY);

        float a = overlayCanvasGroup.alpha;
        float timeElapsed = (isOn ? a : 1 - a) * transitionTime;
        float startValue = a;
        float endValue = isOn ? 1f : 0f;

        overlayCanvasGroup.alpha = endValue;
        overlayCanvasGroup.blocksRaycasts = isOn;
        overlayCanvasGroup.interactable = isOn;

        while(timeElapsed < transitionTime)
        {
            overlayCanvasGroup.alpha = Mathf.Lerp(startValue, endValue, timeElapsed / transitionTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        overlayCanvasGroup.alpha = endValue;

    }

}