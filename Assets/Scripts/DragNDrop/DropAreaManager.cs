using System.Collections.Generic;
using UnityEngine;



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

        Debug.Log("Starting the swap");

        if(DeckersNetworkManager.isOnline)
        {
            Debug.Log("Is online");
            OnlineGameManager.Instance.Deckers_SwapSlots(slot1.areaId, slot2.areaId);
            return;
        }

        Debug.Log("Is not online");

        LocalSwap(slot1.areaId, slot2.areaId);

    }



    public void LocalSwap(int slot1, int slot2)
    {

        Debug.Log("Starting the swap locally");

        DropArea dropArea1 = indexToArea[slot1];
        DropArea dropArea2 = indexToArea[slot2];

        Debug.Log($"Drop area 1: {dropArea1.name}; Drop area 2: {dropArea2.name}");

        if(dropArea1.transform.childCount > 0)
        {
            Debug.Log("Detected a child in " + dropArea1.name);
            DraggableElement element1 = dropArea1.transform.GetChild(0).GetComponent<DraggableElement>();
            element1.SetMostRecentSlot(dropArea2.transform);
        }

        if(dropArea2.transform.childCount > 0)
        {
            Debug.Log("Detected a child in " + dropArea2.name);
            DraggableElement element2 = dropArea2.transform.GetChild(0).GetComponent<DraggableElement>();
            element2.SetMostRecentSlot(dropArea1.transform); 
        }

    }

}