using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public enum Overlay
{
    Connect4,

    // add overlays here
}



public class OverlayManager : MonoBehaviour
{

    public static OverlayManager Instance { get; private set; }

    [SerializeField] private float transitionTime;

    [SerializeField] private CanvasGroup connect4Overlay;



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
            default:
                return null;
        }
    }



    public void ToggleOverlayOn(Overlay overlay, bool isOn = true)
    {
        StartCoroutine(OverlayTransition(overlay, isOn));
    }



    private IEnumerator OverlayTransition(Overlay overlay, bool isOn)
    {

        CanvasGroup canvasGroup = GetOverlay(overlay);
        if(canvasGroup == null) yield break;

        ScreenManager.Instance.SwitchToScreen(UIScreen.SCREEN_EMPTY);

        float a = canvasGroup.alpha;
        float timeElapsed = (isOn ? a : 1 - a) * transitionTime;
        float startValue = a;
        float endValue = isOn ? 1f : 0f;

        while(timeElapsed < transitionTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startValue, endValue, timeElapsed / transitionTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endValue;
        canvasGroup.blocksRaycasts = isOn;
        canvasGroup.interactable = isOn;

    }

}