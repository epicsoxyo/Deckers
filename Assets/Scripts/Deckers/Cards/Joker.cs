using System.Collections;

using UnityEngine;



public class Joker : Card
{

    public override void OnPlay()
    {
        StartCoroutine("PlayJoker");
    }

    private IEnumerator PlayJoker()
    {

        // TODO: make new card invisible for the other player.
        // TODO: allow player to select from three new cards instead of just getting a random one.
        // TODO: hovering over each card opens its definition.

        CardsManager.Instance.DrawCard(team);

        yield return new WaitForSeconds(1f);

        DeckersGameManager.Instance.EndTurn();

    }

}