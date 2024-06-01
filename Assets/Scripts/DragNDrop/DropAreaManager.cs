using System.Collections.Generic;

using UnityEngine;

using Deckers.Game;
using Deckers.Network;



public class DropAreaManager : MonoBehaviour
{

    public static DropAreaManager Instance { get; private set;}
    public static Dictionary<int, DropArea> indexToArea { get; private set; }



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of DropAreaManager detected!");
            return;
        }

        Instance = this;

        indexToArea = new Dictionary<int, DropArea>();

    }



    private void Start()
    {

        DropArea[] dropAreas = FindObjectsOfType<DropArea>();

        foreach(DropArea dropArea in dropAreas)
        {
            indexToArea[dropArea.areaId] = dropArea;
        }

    }



    public void Swap(DropArea slot1, DropArea slot2)
    {

        if(DeckersNetworkManager.isOnline)
        {
            OnlineGameManager.Instance.Deckers_SwapSlots(slot1.areaId, slot2.areaId);
            return;
        }

        LocalSwap(slot1.areaId, slot2.areaId);

    }



    public void LocalSwap(int slot1, int slot2)
    {

        DropArea dropArea1 = indexToArea[slot1];
        DropArea dropArea2 = indexToArea[slot2];

        if(dropArea1.transform.childCount > 0)
        {
            DraggableElement element1 = dropArea1.transform.GetChild(0).GetComponent<DraggableElement>();
            element1.SetMostRecentSlot(dropArea2.transform);
        }

        if(dropArea2.transform.childCount > 0)
        {
            DraggableElement element2 = dropArea2.transform.GetChild(0).GetComponent<DraggableElement>();
            element2.SetMostRecentSlot(dropArea1.transform); 
        }

    }

}