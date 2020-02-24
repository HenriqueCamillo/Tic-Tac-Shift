using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    // Singleton reference
    public static Board instance;

    /// <summary>
    /// Indicates the possible marks on a square
    /// </summary>
    public enum Marks
    {
        Empty,
        X,
        O,
        BrokenX,
        BrokenO
    };

    // Board and reference to the squares
    private Marks[,] board = new Marks[4,4];
    private Square[,] squares = new Square[4,4];

    // Current turn variables
    private Marks turnMark;
    private Sprite turnSprite;

    // Sprites
    [SerializeField] Sprite XSprite;
    [SerializeField] Sprite OSprite;
    [SerializeField] Sprite BrokenXSprite;
    [SerializeField] Sprite BrokenOSprite;
    [SerializeField] Sprite emptySprite;



    /// <summary>
    /// Prepares the game
    /// </summary>
    void Awake()
    {
        // Verifies singleton condition
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

        // Creates a brand new board
        InitializeSquares();
        CleanBoard();

        // Initializes the variables for the first turn
        turnMark = Marks.X;
        turnSprite = XSprite;
    }

    /// <summary>
    /// Gets the references of all squares
    /// </summary>
    void InitializeSquares()
    {
        for (int i = 0; i < 16; i++)
            squares[i%4,i/4] = transform.GetChild(i).GetComponent<Square>();
    }

    /// <summary>
    /// Sets the board and all the squares to empty
    /// </summary>
    void CleanBoard()
    {
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
            {
                board[i,j] = Marks.Empty;
                squares[i,j].Set(Marks.Empty, emptySprite);
            }
    }

    /// <summary>
    /// Swithces the turn of the players
    /// </summary>
    private void SwitchTurn()
    {
        if (turnMark == Marks.X)
        {
            turnMark = Marks.O;
            turnSprite = OSprite;
        }
        else if (turnMark == Marks.O)
        {
            turnMark = Marks.X;
            turnSprite = XSprite;
        }            
    }

    /// <summary>
    /// Marks a square if possible, given its coordinates, and switches turns
    /// </summary>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    public void Mark(int x, int y)
    {
        Debug.Log(x + "," + y);

        // Possible moves:
        /*
            Mark an empty square
            Fix a broken mark
        */
        if (
            board[x,y] == Marks.Empty ||
            (board[x,y] == Marks.BrokenX && turnMark == Marks.X) ||
            (board[x,y] == Marks.BrokenO && turnMark == Marks.O)
        )
        {
            board[x,y] = turnMark;
            squares[x,y].Set(turnMark, turnSprite);

            SwitchTurn();
        }
    }
}