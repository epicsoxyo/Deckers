using Deckers.Game;



public class Donkey : Card
{

    public override bool IsPlayable() { return false;}

    public override bool CanHaveMultiple() { return false; }

    public override void OnDraw()
    {
        // add Donkey's card-switching mechanic to the active abilities list on draw
        DeckersGameManager.Instance.PlayCard(this);
    }

    public override void OnPlay() {} // overrides DeckersGameManager.Instance.EndTurn()

    public override void OnDeckersTurnStart(Team team)
    {

        // PRIORITY: donkey script



    }

}