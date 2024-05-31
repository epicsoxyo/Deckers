using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;



public class SelectionSlot : MonoBehaviour, IPointerEnterHandler
{

    public static event EventHandler onSlotPointerEnter;
    public int index;



    public List<Transform> GetSlotTransforms()
    {

        List<Transform> slotTransforms = new List<Transform>();

        foreach (Transform t in transform)
        {
            slotTransforms.Add(t);
        }

        return slotTransforms;

    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        onSlotPointerEnter?.Invoke(this, EventArgs.Empty);
    }

}