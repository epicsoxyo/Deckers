using System.Collections;

using UnityEngine;



[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class ScreenElement : MonoBehaviour
{

    [SerializeField] private UIScreen screen_;
    public UIScreen screen{ get{ return screen_; } }

    private CanvasGroup _canvasGroup;
    private float _defaultAlpha;

    private RectTransform _rectTransform;

    [SerializeField] private Vector2 offScreenAnchoredPosition;
    private Vector2 _onScreenAnchoredPosition;

    [SerializeField] private float transitionTime = 0.5f;

    private bool _isOnScreen;
    public bool isOnScreen
    {
        get {return _isOnScreen;}
        set
        {

            _canvasGroup.alpha = value ? _defaultAlpha : 0f;
            _canvasGroup.interactable = value;
            _canvasGroup.blocksRaycasts = value;

            _rectTransform.anchoredPosition = value ? _onScreenAnchoredPosition : offScreenAnchoredPosition;

            _isOnScreen = value;

        }
    }



    private void Awake()
    {

        _canvasGroup = GetComponent<CanvasGroup>();
        _defaultAlpha = _canvasGroup.alpha;

        _rectTransform = GetComponent<RectTransform>();
        _onScreenAnchoredPosition = _rectTransform.anchoredPosition;

    }



    public void ExitTransition()
    {

        if(!isOnScreen) return;

        StopAllCoroutines();
        StartCoroutine(Transition(false));

    }



    public void EnterTransition()
    {

        if(isOnScreen) return;

        StopAllCoroutines();
        StartCoroutine(Transition(true));

    }



    private IEnumerator Transition(bool isEntering)
    {

        float timeElapsed = 0f;

        float startAlpha = _canvasGroup.alpha;
        float endAlpha = isEntering ? _defaultAlpha : 0f;

        Vector2 startPos = _rectTransform.anchoredPosition;
        Vector2 endPos = isEntering ? _onScreenAnchoredPosition : offScreenAnchoredPosition;

        while(timeElapsed < transitionTime)
        {

            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / transitionTime);
            _rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, timeElapsed / transitionTime);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        isOnScreen = isEntering;

    }

}