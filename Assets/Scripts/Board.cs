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

    // Boards and reference to the squares
    private Marks[,] board = new Marks[4,4];
    private Marks[,] backupBoard = new Marks[4,4];
    private Marks[,] lastTurnBoard = new Marks[4,4];
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


    [SerializeField] int emptySquares;
    private bool hasShifted = false;

    // Events
    public delegate void ArrowManager(bool row, int index, bool bigArrow);
    public event ArrowManager UpdateArrowsState; 
    public delegate void ArrowEnabler();
    public event ArrowEnabler EnableArrows;

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

        emptySquares = 16;
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

    /// <summary>
    /// Copies the current state of the board to the backup
    /// </summary>
    private void UpdateBackup()
    {
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                if (backupBoard[i,j] != board[i,j])
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

        hasShifted = false;
        EnableArrows?.Invoke();
        UpdateBackup();   
    }

    /// <summary>
    /// Marks a square if possible, given its coordinates, and switches the turn
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
        // If marking a empty square
        if (board[x,y] == Marks.Empty)
        {
            board[x, y] = turnMark;
            squares[x, y].Set(turnMark);

            if (!hasShifted && WonGame())
            {
                Debug.Log(turnMark + " won");
                CleanBoard();   
            }


            if (--emptySquares == 0)
                CleanBoard();
            else
                SwitchTurn();
        }
        // If restoring a broken mark
        else if ((board[x,y] == Marks.BrokenX && turnMark == Marks.X) || (board[x,y] == Marks.BrokenO && turnMark == Marks.O))
        {
            board[x, y] = turnMark;
            squares[x, y].Set(turnMark);

            if (WonGame())
            {
                Debug.Log(turnMark + " won");
                CleanBoard();   
            }

            SwitchTurn();
        }
    }

    public void ShiftAllColumns(bool positiveMovement)
    {
        for (int i = 0; i < 4; i++)
            ShiftColumn(i, positiveMovement, true);

        if (WonGame())
        {
            Debug.Log(turnMark + " won");
            CleanBoard();   
        }

        UpdateButtons(false, 0, true);
        UpdateSquares();
    }

    public void ShiftAllRows(bool positiveMovement)
    {
        for (int i = 0; i < 4; i++)
            ShiftRow(i, positiveMovement, true);

        if (WonGame())
        {
            Debug.Log(turnMark + " won");
            CleanBoard();   
        }

        UpdateButtons(true, 0, true);
        UpdateSquares();
    }

    public void ShiftColumn(int index, bool positiveMovement, bool bigArrow)
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

        if (!bigArrow)
        {
            if (WonGame())
            {
                Debug.Log(turnMark + " won");
                CleanBoard();   
            }

            UpdateButtons(false, index, bigArrow);
            UpdateSquares();
        }
    }

    public void ShiftRow(int index, bool positiveMovement, bool bigArrow)
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

        // Checks for win condition and updates squares only if it is not a big arrow.
        // If it is, checks outside this method
        if (!bigArrow)
        {
            if (WonGame())
            {
                Debug.Log(turnMark + " won");
                CleanBoard();   
            }

            UpdateButtons(true, index, bigArrow);
            UpdateSquares();
        }
    }

    private void UpdateButtons(bool row, int index, bool bigArrow)
    {
        if (CompareWith(backupBoard))
        {
            hasShifted = false;
            EnableArrows?.Invoke();
        }
        else
        {
            hasShifted = true;
            // if (CompareWith(lastTurnBoard))
                
            // else
                UpdateArrowsState?.Invoke(row, index, bigArrow);
        }

    }

    /// <summary>
    /// Compares the current state of the board with another version
    /// </summary>
    /// <returns>Returns true if they are equal</returns>
    private bool CompareWith(Marks[,] otherBoard)
    {
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                if (otherBoard[i,j] != board[i,j])
                    return false;
        return true;
    }

    /// <summary>
    /// Checks if the player whose turn is the current has won the game
    /// </summary>
    /// <returns>Returns ture if the game was won</returns>
    private bool WonGame()
    {
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                if (board[i,j] == turnMark)
                    if (HasVictoryCondition(i, j))
                        return true;

        return false;
    }

    /// <summary>
    /// Verifies if a index is valid or not
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>Returns true if the index is invalid</returns>
    private bool OutOfBounds(int x, int y)
    {
        if (x < 0 || x > 3 || y < 0 || y > 3)
            return true;
        else 
            return false;
    }

    /// <summary>
    /// Verifies if a square with coordinates [x,y] is part of a connection of 3 marks (victory condition)
    /// </summary>
    /// <param name="x">X position of the square</param>
    /// <param name="y">Y position of the square</param>
    /// <returns>Returns true if the victory condition is found</returns>
    private bool HasVictoryCondition(int x, int y)
    {
        // Possible connections
        Vector2[] connections = { Vector2.up, Vector2.down, Vector2.right, Vector2.left,
               new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1),new Vector2(-1, -1)};


        int newX, newY;
        // For each one of the possible connections
        foreach (var connection in connections)
        {
            // Verifies if there is connection using the array of possible connections
            newX = x + (int)connection.x;
            newY = y + (int)connection.y;

            // Go to next possible connection if it's out of bounds
            if (OutOfBounds(newX, newY))
                continue;

            // If there is a connection
            if (board[newX, newY] == turnMark)
            {
                // Verifies if [x,y] is the center of the connection of 3   
                newX = x - (int)connection.x;
                newY = y - (int)connection.y;
                
                if (!OutOfBounds(newX, newY))
                {
                    if (board[newX, newY] == turnMark)
                        return true;
                }
                // If it gets out of bounds, checks if it is the start
                else 
                {
                    newX = x + 2 * (int)connection.x;
                    newY = y + 2 * (int)connection.y;

                    if (!OutOfBounds(newX, newY))
                    {
                        if (board[newX, newY] == turnMark)
                            return true;
                    }
                }
            }
        }

        return false;
    }
}