using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
    public Board.Marks content;
    private Image image;

    [Tooltip("index[0] = x position, index[1] = y position")]
    [SerializeField] int[] index = new int[2];

    /// <summary>
    /// Gets image script reference
    /// </summary>
    void Start()
    {
        image = GetComponent<Image>();
    }

    /// <summary>
    /// Sets the content of the square
    /// </summary>
    public void Set(Board.Marks mark, Sprite newImage)
    {
        content = mark;
        image.sprite = newImage;
    }

    /// <summary>
    /// On click, calls a board function to mark this square, if possible
    /// </summary>
    public void Mark()
    {
        Board.instance.Mark(index[0], index[1]);
    }
}