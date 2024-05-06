using System.Collections.Generic;

using UnityEngine;



public class CardsManager : MonoBehaviour
{

    public static CardsManager Instance { get; private set; }

    [SerializeField] private Transform upperCards;
    [SerializeField] private Transform lowerCards;

    private List<Transform> _upperCardSlots = new List<Transform>();
    private List<Transform> _lowerCardSlots = new List<Transform>();

    public List<Card> whiteCards { get; private set; }
    public List<Card> redCards  { get; private set; }



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of CapturedPiecesManager detected!");
            return;
        }

        Instance = this;

        foreach(Transform t in upperCards)
        {
            _upperCardSlots.Add(t);
        }
        foreach(Transform t in lowerCards)
        {
            _lowerCardSlots.Add(t);
        }

    }

}