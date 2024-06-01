using UnityEngine;

using TMPro;

using Deckers.Game;



public class RoundsCounter : MonoBehaviour
{

    private TextMeshProUGUI _counterText;



    private void Awake()
    {
        _counterText = GetComponent<TextMeshProUGUI>();
        _counterText.SetText("Current round: 1");
    }



    private void Start()
    {
        LocalGameManager.Instance.OnRoundEnd += OnRoundEnd;
    }



    private void OnRoundEnd(int roundNumber)
    {
        _counterText.SetText("Current turn: " + roundNumber.ToString());
    }

}
