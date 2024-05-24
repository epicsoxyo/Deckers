using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Joker : Card
{

    public override void OnPlay()
    {
        StartCoroutine("PlayJoker");
    }

    private IEnumerator PlayJoker()
    {

        // ScreenManager.Instance.SwitchToScreen(UIScreen.SCREEN_EMPTY);

        CardsManager.Instance.DrawCard(team);

        yield return new WaitForSeconds(1f);

        DeckersGameManager.Instance.EndTurn();

    }



    public override void OnDeckersTurnStart() {}

    public override void OnCheckersTurnStart() {}

    public override void OnTurnEnd() {}

    public override void OnGameEnd() {}

}