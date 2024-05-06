using UnityEngine;



[CreateAssetMenu(fileName = "Card Information", menuName = "Card Information", order = 0)]
public class CardInfo : ScriptableObject
{

    public string cardName;

    [TextArea(5, 15)]
    public string cardDescription;

}