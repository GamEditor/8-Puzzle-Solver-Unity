using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzleCell : MonoBehaviour, IPointerClickHandler
{
    private Puzzle m_Parent;

    // Cell information
    public int m_Row, m_Column;
    public int m_CellNumber;
    public Text m_CellText;

    private void Awake()
    {
        m_Parent = GetComponentInParent<Puzzle>();
        m_CellText = GetComponent<Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_Parent != null)
            m_Parent.SwapCellValue(this);
    }
}