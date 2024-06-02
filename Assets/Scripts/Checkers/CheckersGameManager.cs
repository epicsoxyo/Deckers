using System;

using UnityEngine;
using UnityEngine.UI;

using Deckers.Network;



namespace Deckers.Game
{

    public class CheckersGameManager : MonoBehaviour
    {

        public static CheckersGameManager Instance { get; private set; }

        [SerializeField] private Button _skipTurnButton;

        public Team CurrentPlayer { get; private set; }

        public event Action OnEndTurn;



        private void Awake()
        {
            if(Instance != null)
            {
                Debug.LogWarning("Multiple instances of CheckersGameManager detected!");
                return;
            }
            Instance = this;
        }



        private void Start()
        {
            _skipTurnButton.onClick.AddListener(EndTurn);
            LocalGameManager.Instance.OnCheckersStart += BeginTurn;
        }



        public void BeginTurn(Team player)
        {

            CurrentPlayer = player;

            PieceSelectionManager.Instance.UpdateActivePieces(CurrentPlayer);

            if(DeckersNetworkManager.isOnline
            && player != OnlineGameManager.Instance.localTeam)
            {
                ScreenManager.Instance.SwitchToScreen(UIScreen.SCREEN_EMPTY);
                return;
            }

            ScreenManager.Instance.SwitchToScreen(UIScreen.SCREEN_CHECKERS);
            GridSquare.onGridClick += OnGridClick;
            GamePiece.onClick += OnPieceClick;

        }

        private void OnGridClick(object sender, EventArgs e)
        {
            GridSquare clickedSquare = sender as GridSquare;
            if(clickedSquare != null){ TakeTurn(clickedSquare); }
        }

        private void OnPieceClick(object sender, EventArgs e)
        {
            GamePiece clickedPiece = sender as GamePiece;
            PieceSelectionManager.Instance.DisplayAvailableMoves(clickedPiece);
        }



        public void TakeTurn(GridSquare destination)
        {

            bool isOnline = DeckersNetworkManager.isOnline;
            OnlineGameManager networkedGame = OnlineGameManager.Instance;

            GamePiece selectedPiece = PieceSelectionManager.Instance.SelectedPiece;

            // movement

            if(isOnline)
            {
                networkedGame.Checkers_MovePiece(selectedPiece.Id, destination.x, destination.y);
            }
            else
            {
                LocalMovePiece(selectedPiece.Id, destination.x, destination.y);
            }

            // promotion

            if((CurrentPlayer == Team.TEAM_WHITE && destination.y == 8)
            || (CurrentPlayer == Team.TEAM_RED && destination.y == 1))
            {
                if(isOnline)
                {
                    networkedGame.Checkers_PromotePiece(selectedPiece.Id);
                }
                else
                {
                    LocalPromotePiece(selectedPiece.Id);
                }
            }

            // capture

            GamePiece capturedPiece = PieceSelectionManager.Instance.AvailableMoves[destination];
            if(capturedPiece == null)
            {
                EndTurn();
                return;
            }

            if(isOnline)
            {
                networkedGame.Checkers_CapturePiece(capturedPiece.Id);
            }
            else
            {
                LocalCapturePiece(capturedPiece.Id);
            }

            // bonus turn

            if(selectedPiece.PieceType == GamePieceType.PIECE_BISHOP)
            {
                EndTurn();
                return;
            }

            PieceSelectionManager.Instance.UpdateActivePiece(selectedPiece);
            
            if(!PieceSelectionManager.Instance.DisplayAvailableMoves(selectedPiece, isCapturing: true))
            {
                EndTurn();
            }

        }

        public void LocalMovePiece(int pieceId, int destinationX, int destinationY)
        {
            GamePiece pieceToMove = Board.FindPiece(pieceId);
            GridSquare destination = Board.grid[(destinationX, destinationY)];
            pieceToMove.transform.SetParent(destination.transform);
        }

        public void LocalPromotePiece(int pieceId)
        {
            GamePiece pieceToPromote = Board.FindPiece(pieceId);
            pieceToPromote.Promote();
        }

        public void LocalCapturePiece(int pieceId)
        {
            GamePiece capturedPiece = Board.FindPiece(pieceId);
            capturedPiece.Capture();
        }



        public void EndTurn()
        {

            if(DeckersNetworkManager.isOnline)
            {
                OnlineGameManager.Instance.Checkers_EndTurn();
            }
            else
            {
                LocalEndTurn();
            }

            PieceSelectionManager.Instance.ClearCurrentSelection();

            GridSquare.onGridClick -= OnGridClick;
            GamePiece.onClick -= OnPieceClick;

        }

        public void LocalEndTurn()
        {
            OnEndTurn?.Invoke();
        }

    }

}