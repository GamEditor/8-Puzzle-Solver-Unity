using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleCell : MonoBehaviour, IPointerClickHandler
{
    private Puzzle m_Parent;

    // Cell information
    public int m_Row, m_Column;
    public int m_CellNumber;
    public TextMeshProUGUI m_CellText;

    private void Awake()
    {
        m_Parent = GetComponentInParent<Puzzle>();
        m_CellText = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_Parent != null)
            m_Parent.SwapCellValue(this);
    }
}