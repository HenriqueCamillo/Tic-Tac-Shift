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
    private Marks[,] backupBoard = new Marks[4,4];
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
    private void Awake()
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
    private void InitializeSquares()
    {
        for (int i = 0; i < 16; i++)
            squares[i%4,i/4] = transform.GetChild(i).GetComponent<Square>();
    }

    /// <summary>
    /// Sets the board and all the squares to empty
    /// </summary>
    private void CleanBoard()
    {
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
            {
                board[i,j] = Marks.Empty;
                backupBoard[i,j] = Marks.Empty;
                squares[i,j].Set(Marks.Empty);
            }
    }

    /// <summary>
    /// Updates all squares according to the modifications made on the board
    /// </summary>
    private void UpdateSquares()
    {
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                squares[i,j].Set(board[i,j]);
    }

    private void UpdateBackup()
    {
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                backupBoard[i,j] = board[i,j];
    }

    /// <summary>
    /// Returns the sprite corresponding to the mark
    /// </summary>
    /// <param name="mark"></param>
    /// <returns></returns>
    public Sprite GetSprite(Marks mark)
    {
        if (mark == Marks.X)
            return XSprite;
        else if (mark == Marks.O)
            return OSprite;
        else if (mark == Marks.BrokenX)
            return BrokenXSprite;
        else if (mark == Marks.BrokenO)
            return BrokenOSprite;
        else
            return emptySprite;
    }

    /// <summary>
    /// Swithces the turn of the players and updates the backup board
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

        UpdateBackup();   
    }

    /// <summary>
    /// Marks a square if possible, given its coordinates, and switches turns
    /// </summary>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    public void Mark(int x, int y)
    {
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
            board[x, y] = turnMark;
            squares[x, y].Set(turnMark);

            SwitchTurn();
        }
    }

    public void ShiftAllColumns(bool positiveMovement)
    {
        for (int i = 0; i < 4; i++)
            ShiftColumn(i, positiveMovement);
    }

    public void ShiftAllRows(bool positiveMovement)
    {
        for (int i = 0; i < 4; i++)
            ShiftRow(i, positiveMovement);
    }

    public void ShiftColumn(int index, bool positiveMovement)
    {
        if (positiveMovement)
        {
            Marks temp = board[index, 3];
            for (int j = 3; j > 0; j--)
                board[index, j] = board[index, j-1];
            board[index, 0] = temp;
        }
        else
        {
            Marks temp = board[index, 0];
            for (int j = 0; j < 3; j++)
                board[index, j] = board[index, j+1];
            board[index, 3] = temp;
        }

        UpdateSquares();
    }

    public void ShiftRow(int index, bool positiveMovement)
    {
        if (positiveMovement)
        {
            Marks temp = board[3, index];
            for (int i = 3; i > 0; i--)
                board[i, index] = board[i-1, index];
            board[0, index] = temp;
        }
        else
        {
            Marks temp = board[0, index];
            for (int i = 0; i < 3; i++)
                board[i, index] = board[i+1, index];
            board[3, index] = temp;
        }

        UpdateSquares();
    }

}