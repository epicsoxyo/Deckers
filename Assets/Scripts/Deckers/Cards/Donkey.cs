using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Donkey : Card
{

    protected override void Awake()
    {
        base.Awake();
        _canBeUsed = false;
    }

    public override void OnPlay() {}

    public override void OnDeckersTurnStart() {}

    public override void OnCheckersTurnStart() {}

    public override void OnTurnEnd() {}

    public override void OnGameEnd() {}

}