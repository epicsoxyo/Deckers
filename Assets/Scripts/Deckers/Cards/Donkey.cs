using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Deckers.Game;



public class Donkey : Card
{

    public override bool IsPlayable() { return false;}

    public override void OnDraw()
    {
        DeckersGameManager.Instance.PlayCard(this);
    }

    public override void OnPlay() {} // overrides DeckersGameManager.Instance.EndTurn()

    public override void OnDeckersTurnStart()
    {

        // if it is the first turn,

    }

}