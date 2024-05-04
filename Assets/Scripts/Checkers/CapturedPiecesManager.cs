using System.Collections.Generic;

using UnityEngine;



public class CapturedPiecesManager : MonoBehaviour
{

    public static CapturedPiecesManager Instance { get; private set; }

    [SerializeField] private Transform whiteCapturedPiecesTransform;
    [SerializeField] private Transform redCapturedPiecesTransform;

    private List<Transform> _whiteCaptureSlots = new List<Transform>();
    private List<Transform> _redCaptureSlots = new List<Transform>();

    public int whiteCapturedPieces { get; private set; }
    public int redCapturedPieces  { get; private set; }



    private void Awake()
    {

        if(Instance != null)
        {
            Debug.LogWarning("Multiple instances of CapturedPiecesManager detected!");
            return;
        }

        Instance = this;

        foreach(Transform t in whiteCapturedPiecesTransform)
        {
            _whiteCaptureSlots.Add(t);
        }
        foreach(Transform t in redCapturedPiecesTransform)
        {
            _redCaptureSlots.Add(t);
        }

    }



    public void CapturePiece(GamePiece capturedPiece)
    {

        switch(capturedPiece.player)
        {

            case Team.TEAM_WHITE:
                if(whiteCapturedPieces >= _whiteCaptureSlots.Count) return;
                capturedPiece.transform.SetParent(_whiteCaptureSlots[whiteCapturedPieces]);
                whiteCapturedPieces++;
                break;
            case Team.TEAM_RED:
                if(redCapturedPieces >= _redCaptureSlots.Count) return;
                capturedPiece.transform.SetParent(_redCaptureSlots[redCapturedPieces]);
                redCapturedPieces++;
                break;

        }


    }

}