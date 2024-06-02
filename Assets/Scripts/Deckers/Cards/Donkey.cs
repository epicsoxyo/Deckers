using Deckers.Game;



public class Donkey : Card
{

    public override bool IsPlayable() { return false;}

    public override void OnDraw()
    {
        DeckersGameManager.Instance.PlayCard(this);
    }

    public override void OnPlay() {} // overrides DeckersGameManager.Instance.EndTurn()

    public override void OnDeckersTurnStart(Team team)
    {

        // PRIORITY: donkey script

        // if it is the first turn,

    }

}