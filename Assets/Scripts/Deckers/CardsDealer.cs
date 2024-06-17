using System.Collections.Generic;

using UnityEngine;



public enum CardsPool
{

    POOL_NORMAL,
    POOL_JOKER,

    // add card pool names here...

}



public class CardsDealer : MonoBehaviour
{

    public static CardsDealer Instance { get; private set; }

    [SerializeField] private List<GameObject> normalPool = new List<GameObject>();
    [SerializeField] private List<GameObject> jokerPool = new List<GameObject>();



    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of CardsDealer detected!");
            return;
        }
        Instance = this;
    }



    public int GetRandomCardIndex(CardsPool cardsPool = CardsPool.POOL_NORMAL)
    {

        int maxIndex;

        switch(cardsPool)
        {
            case CardsPool.POOL_NORMAL:
                maxIndex = normalPool.Count - 1;
                break;
            case CardsPool.POOL_JOKER:
                maxIndex = jokerPool.Count - 1;
                break;
            default:
                Debug.LogWarning(cardsPool + " does not have an assigned pool. Using normal pool...");
                maxIndex = normalPool.Count - 1;
                break;
        }

        return Random.Range(0, maxIndex);

    }



    public Card GetNewCardFromIndex(int index, Transform parentTransform, CardsPool cardsPool = CardsPool.POOL_NORMAL)
    {

        GameObject cardGameObject;

        switch(cardsPool)
        {
            case CardsPool.POOL_JOKER:
                cardGameObject = jokerPool[index];
                break;
            default:
                if(normalPool.Count == 0) { cardGameObject = jokerPool[index]; }
                else
                {
                    cardGameObject = normalPool[index];
                    normalPool.RemoveAt(index); // FOR NOW!!
                }
                break;
        }

        Card instancedCard = Instantiate(cardGameObject, parentTransform, false).GetComponent<Card>();

        // if(!instancedCard.CanHaveMultiple()) { normalPool.RemoveAt(index); }

        return instancedCard;

    }

}