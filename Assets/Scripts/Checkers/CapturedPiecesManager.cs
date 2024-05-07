using System;
using System.Collections.Generic;

using UnityEngine;



public class CaptureManager : MonoBehaviour
{

    public static CaptureManager Instance { get; private set; }

    [SerializeField] private Transform upperCaptureDial;
    [SerializeField] private Transform lowerCaptureDial;

    private List<Transform> _upperCaptureSlots = new List<Transform>();
    private List<Transform> _lowerCaptureSlots = new List<Transform>();

    public int whiteCapturedPieces { get; private set; }
    public int redCapturedPieces  { get; private set; }

    public event EventHandler onWhiteWin;
    public event EventHandler onRedWin;



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of CapturedPiecesManager detected!");
            return;
        }

        Instance = this;

        foreach(Transform t in upperCaptureDial)
        {
            _upperCaptureSlots.Add(t);
        }
        foreach(Transform t in lowerCaptureDial)
        {
            _lowerCaptureSlots.Add(t);
        }

    }



    public void Capture(GamePiece capturedPiece)
    {

        List<Transform> captureSlots;
        bool flipDials = (OnlineGameManager.Instance.localTeam == Team.TEAM_RED);

        switch(capturedPiece.player)
        {
            case Team.TEAM_WHITE:
                captureSlots = flipDials ? _upperCaptureSlots : _lowerCaptureSlots;

                capturedPiece.transform.SetParent(captureSlots[whiteCapturedPieces]);
                whiteCapturedPieces++;

                if(whiteCapturedPieces >= captureSlots.Count)
                {
                    Debug.Log("Sufficient captures for a win.");
                    TriggerWin(Team.TEAM_RED);
                    return;
                }

                break;

            case Team.TEAM_RED:
                captureSlots = flipDials ? _lowerCaptureSlots : _upperCaptureSlots;
                Debug.Log($"Red captured pieces: {redCapturedPieces}; Capture slots: {captureSlots.Count}");

                capturedPiece.transform.SetParent(captureSlots[redCapturedPieces]);
                redCapturedPieces++;

                if(redCapturedPieces >= captureSlots.Count)
                {
                    Debug.Log("Sufficient captures for a win.");
                    TriggerWin(Team.TEAM_WHITE);
                    return;
                }

                break;

        }

    }



    private void TriggerWin(Team winningTeam)
    {

        switch(winningTeam)
        {
            case Team.TEAM_WHITE:
                Debug.Log("Invoking onWhiteWin");
                onWhiteWin?.Invoke(this, EventArgs.Empty);
                return;
            case Team.TEAM_RED:
                Debug.Log("Invoking onRedWin");
                onRedWin?.Invoke(this, EventArgs.Empty);
                return;
        }
    }


}