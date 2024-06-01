using UnityEngine;

using TMPro;

using Deckers.Game;



public class TurnsCounter : MonoBehaviour
{

    private TextMeshProUGUI _counterText;



    private void Awake()
    {
        _counterText = GetComponent<TextMeshProUGUI>();
        _counterText.SetText("Current turn: 1");
    }



    private void Start()
    {
        LocalGameManager.Instance.OnTurnEnd += OnTurnEnd;
    }



    private void OnTurnEnd(int turnNumber)
    {
        _counterText.SetText("Current turn: " + turnNumber.ToString());
    }

}