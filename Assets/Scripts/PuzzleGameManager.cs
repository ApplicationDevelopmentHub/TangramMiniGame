using UnityEngine;
using UnityEngine.UI;

public class PuzzleGameManager : MonoBehaviour
{
    [Header("All Pieces")]
    public PieceSnapper[] pieces;

    [Header("Completion UI")]
    public GameObject completionPanel; //UI to show the completion message

    private bool gameCompleted = false;

    void Start()
    {
        completionPanel.SetActive(false);
    }

    void Update()
    {
        if (gameCompleted)
            return;

        if (CheckCompletion())
        {
            OnGameCompleted();
        }
    }

    bool CheckCompletion()
    {
        foreach (PieceSnapper piece in pieces)
        {
            if (!piece.isSnapped)
                return false;
        }

        return true;
    }

    void OnGameCompleted()
    {
        gameCompleted = true;

        Debug.Log("Puzzle Completed!");

        completionPanel.SetActive(true);
    }

    
    //Restart Game
    public void RestartGame()
    {
        foreach (PieceSnapper piece in pieces)
        {
            ResetPiece(piece);
        }

        completionPanel.SetActive(false);
        gameCompleted = false;
    }

    void ResetPiece(PieceSnapper piece)
    {
        PieceInteraction interaction =
            piece.GetComponent<PieceInteraction>();

        // Reset snap state
        piece.isSnapped = false;

        // Reset slot occupancy
        foreach (var snap in piece.allowedTargets)
        {
            if (snap.target == null) continue;

            SnapSlot slot =
                snap.target.GetComponent<SnapSlot>();

            if (slot != null)
                slot.isOccupied = false;

            Renderer r =
                snap.target.GetComponent<Renderer>();

            if (r != null)
                r.enabled = true;
        }

        // Reset transform
        interaction.SendMessage(
            "ReturnToCluster",
            SendMessageOptions.DontRequireReceiver);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
