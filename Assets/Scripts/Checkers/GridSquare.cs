using System;

using UnityEngine;
using UnityEngine.UI;



[RequireComponent(typeof(Button))]
public class GridSquare : MonoBehaviour
{

    // TODO: implement gridsquare properties (e.g. is burning)

    private static int _currentId = 0;
    public int id { get; private set; }

    public int x;
    public int y;

    private Image _glow;
    private Button _gridButton;

    public static event EventHandler onGridClick;



    private void Awake()
    {

        id = _currentId++;

        _glow = GetComponent<Image>();
        _glow.enabled = false;

        _gridButton = GetComponent<Button>();
        _gridButton.onClick.AddListener(OnGridSquareClick);
        _gridButton.enabled = false;

    }



    private void Start()
    {
        RectTransform rectTransform = transform as RectTransform;
        rectTransform.localScale = new Vector3(1, 1, 1);
    }



    public void DisplayAsAvailableMove(bool isAvailable = true)
    {
        _glow.enabled = isAvailable;
        _gridButton.enabled = isAvailable;
    }



    private void OnGridSquareClick()
    {
        onGridClick?.Invoke(this, EventArgs.Empty);
    }

}