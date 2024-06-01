using System.Collections.Generic;
using UnityEngine;



namespace Deckers.Game
{

    public class Player
    {

        public readonly Team team;

        public List<Card> hand;

        public bool HasDonkey { get; private set; }
        public bool HasSun { get; private set; }

        public int CapturedPieces { get; private set; }

        public Player(Team team)
        {
            this.team = team;

            hand = new List<Card>();

            HasDonkey = false;
            HasSun = false;

            CapturedPieces = 0;

        }

    }



    public class PlayerManager : MonoBehaviour
    {

        public static PlayerManager Instance { get; private set;}

        [field:SerializeField] public Player WhitePlayer { get; private set; }

        [field:SerializeField] public Player RedPlayer { get; private set; }



        private void Awake()
        {

            if(Instance != null)
            {
                Debug.LogWarning("Multiple instances of PlayerManager detected!");
                return;
            }

            Instance = this;

            WhitePlayer = new Player(Team.TEAM_WHITE);
            RedPlayer = new Player(Team.TEAM_RED);

        }

    }

}