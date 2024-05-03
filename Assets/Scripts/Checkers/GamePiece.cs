using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{

    private static _currentId;
    public int id { get; private set; }



    private void Awake()
    {

        if(_currentId == null) _currentId = 0;

        id = _currentId;
        _currentId++;

    }



    private void OnDestroy()
    {
        // increment amount of captured pieces
    }


}