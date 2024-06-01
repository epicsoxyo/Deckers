using System.Collections.Generic;

using UnityEngine;



public class CardsDealer : MonoBehaviour
{

    public static CardsDealer Instance { get; private set; }

    [SerializeField] private List<GameObject> cardsPool = new List<GameObject>();
    [SerializeField] private List<GameObject> jokerPool = new List<GameObject>();

    [SerializeField] private Transform _spawnArea;



    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of CardsDealer detected!");
            return;
        }
        Instance = this;
    }



    public int GetRandomCardIndex()
    {
        int maxIndex = cardsPool.Count > 0 ? cardsPool.Count : jokerPool.Count;
        return Random.Range(0, maxIndex);
    }



    public Card GetNewCardFromIndex(int index, bool forceUseJokerPool = false)
    {

        bool useJokerPool = forceUseJokerPool || (cardsPool.Count == 0);

        GameObject cardGameObject = useJokerPool ? jokerPool[index] : cardsPool[index];
        Card instancedCard = Instantiate(cardGameObject, _spawnArea, false).GetComponent<Card>();

        if(!useJokerPool) cardsPool.RemoveAt(index);

        return instancedCard;

    }

}