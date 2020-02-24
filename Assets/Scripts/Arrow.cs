using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    [SerializeField] bool row;
    [SerializeField] bool column;
    [SerializeField] bool positiveMovement;
    [SerializeField] int index;
    [SerializeField] bool bigArrow;
    private Button button;

    void Start()
    {
        if (row && column)
            Debug.LogWarning(name + "is column and row at the same time");
        else if (!row && !column)
            Debug.LogWarning(name + "is neither column nor row");
        
        button = GetComponent<Button>();
        Board.instance.UpdateArrowsState += UpdateState;
        Board.instance.EnableArrows += Enable;
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
                Board.instance.ShiftRow(index, positiveMovement, bigArrow);
            else if (column)
                Board.instance.ShiftColumn(index, positiveMovement,bigArrow);
        }
    }

    private void UpdateState(bool row, int index, bool bigArrow)
    {
        if (this.row != row || this.index != index || this.bigArrow != bigArrow)
            button.interactable = false;
    }

    private void Enable()
    {
        button.interactable = true;
    }
}
