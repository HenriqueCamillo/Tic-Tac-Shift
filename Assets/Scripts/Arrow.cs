using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] bool row;
    [SerializeField] bool column;
    [SerializeField] bool positiveMovement;
    [SerializeField] int index;
    [SerializeField] bool bigArrow;

    void Start()
    {
        if (row && column)
            Debug.LogWarning(name + "is column and row at the same time");
        else if (!row && !column)
            Debug.LogWarning(name + "is neither column nor row");
    }

    /// <summary>
    /// Sends message to board to shift a column or a row, if possible
    /// </summary>
    public void Shift()
    {
        if (bigArrow)
        {
            if (row)
                Board.instance.ShiftAllRows(positiveMovement);
            else if (column)
                Board.instance.ShiftAllColumns(positiveMovement);
        }
        else
        {
            if (row)
                Board.instance.ShiftRow(index, positiveMovement);
            else if (column)
                Board.instance.ShiftColumn(index, positiveMovement);
        }
    }
}
