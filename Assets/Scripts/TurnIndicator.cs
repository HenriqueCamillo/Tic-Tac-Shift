using UnityEngine;
using UnityEngine.UI;

public class TurnIndicator : MonoBehaviour
{
    [SerializeField] Image player1, player2;

    /// <summary>
    /// Gets the sprites from the board, and subscribe to turn change event, to keep track of it.
    /// Shows the sprite whose turn is the current brighter than the other.
    /// </summary>
    private void Start()
    {
        player1.sprite = Board.instance.XSprite;
        player2.sprite = Board.instance.OSprite;

        if (Board.instance.TurnMark == Board.Marks.X)
        {
            player1.color = Color.white;
            player2.color = Color.grey;
        }
        else
        {
            player1.color = Color.grey;
            player2.color = Color.white;
        }
        

        Board.instance.OnTurnChange += ChangeTurn;
        Board.instance.OnEndGame += Reset;
    }

    private void Reset()
    {
        player1.sprite = Board.instance.XSprite;
        player2.sprite = Board.instance.OSprite;

        if (Board.instance.TurnMark == Board.Marks.X)
        {
            player1.color = Color.white;
            player2.color = Color.grey;
        }
        else
        {
            player1.color = Color.grey;
            player2.color = Color.white;
        }

    }

    /// <summary>
    /// Inverts the brightness of the sprites
    /// </summary>
    private void ChangeTurn()
    {
        if (player1.color == Color.grey)
        {
            player1.color = Color.white;
            player2.color = Color.grey;
        }
        else
        {
            player2.color = Color.white;
            player1.color = Color.grey;
        } 
    }
}