using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class CardDropArea : DropArea
{

    [SerializeField] private TextMeshProUGUI titleBox;
    [SerializeField] private TextMeshProUGUI descriptionBox;



    public override void SwapForThis(DraggableElement draggableElement)
    {

        base.SwapForThis(draggableElement);

        Card card = draggableElement.GetComponent<Card>();
        titleBox.SetText(card.GetName());
        descriptionBox.SetText(card.GetDescription());

    }



    public void ClearDescription()
    {
        titleBox.SetText("");
        descriptionBox.SetText("");
    }

}