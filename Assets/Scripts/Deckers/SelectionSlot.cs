using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class SelectionSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public static event EventHandler OnSlotPointerEnter;
    public int Index;

    private Image _image;
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Sprite _deselectedSprite;



    private void Awake()
    {
        _image = GetComponent<Image>();
    }



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
        _image.sprite = _selectedSprite;
        OnSlotPointerEnter?.Invoke(this, EventArgs.Empty);
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        _image.sprite = _deselectedSprite;
    }

}