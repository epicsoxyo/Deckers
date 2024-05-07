using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Joker : Card
{

    public override void OnPlay()
    {
        CardsManager.Instance.DrawCard(team);
    }

    public override void OnDeckersTurnStart() {}

    public override void OnCheckersTurnStart() {}

    public override void OnTurnEnd() {}

    public override void OnGameEnd() {}

}