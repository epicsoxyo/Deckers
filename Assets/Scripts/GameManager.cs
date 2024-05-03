using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum Player
{
    PLAYER_WHITE,
    PLAYER_RED,
}



public enum GameState
{
    STATE_WAITING_TO_START,
    STATE_PLAYING,
    STATE_END_OF_GAME,
}



public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set;}

    private int _currentTurn = 1;




    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of GameManager detected!");
            return;
        }

        Instance = this;

    }



    private void TakeTurn()
    {

        // TODO: Deckers turns

        // start of turn card effects in order they were played

        // player 1 play a card

        // player 2 play a card

        // middle of turn card effects in order they were played

        // player 1 play checkers

        // player 2 play checkers

        // end of turn card effects

        // check for a win

        // if someone has won, do on win card effects

        // if neither player won yet, increment turn

        _currentTurn ++;

    }

}