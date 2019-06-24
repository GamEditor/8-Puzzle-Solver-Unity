using UnityEngine;

public class Puzzle : MonoBehaviour
{
    private PuzzleCell m_CurrentCell;

    public int[,] m_Puzzle = new int[Utilities.s_PuzzleDimension, Utilities.s_PuzzleDimension];

    private void Start()
    {
        int childCount = transform.childCount;
        int index = 0;

        for (int i = 0; i < Utilities.s_PuzzleDimension; i++)
            for (int j = 0; j < Utilities.s_PuzzleDimension; j++)
            {
                // filling m_Puzzle cells information
                transform.GetChild(index).gameObject.AddComponent<PuzzleCell>();
                PuzzleCell cell = transform.GetChild(index).gameObject.GetComponent<PuzzleCell>();

                cell.m_Row = i;
                cell.m_Column = j;

                if (index < Utilities.Pow(Utilities.s_PuzzleDimension, 2) - 1)
                {
                    cell.m_CellNumber = index + 1;
                    cell.m_CellText.text = (index + 1).ToString();
                }
                else
                {
                    cell.m_CellNumber = 0;
                    cell.m_CellText.text = "";
                }

                index++;
            }

        UpdatePuzzle();
    }

    public void SwapCellValue(PuzzleCell cell)
    {
        if (m_CurrentCell == null)
            m_CurrentCell = cell;
        else
        {
            // if user clicked on a cell twice, doesn't need to proceed
            if(m_CurrentCell == cell)
            {
                m_CurrentCell = null;
                return;
            }

            string temp = m_CurrentCell.m_CellText.text;
            m_CurrentCell.m_CellText.text = cell.m_CellText.text;
            cell.m_CellText.text = temp;
            
            Utilities.Swap(ref m_CurrentCell.m_CellNumber, ref cell.m_CellNumber);

            m_CurrentCell = null;

            UpdatePuzzle();
        }
    }

    // m_Puzzle will be set here
    public void UpdatePuzzle()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            PuzzleCell cell = transform.GetChild(i).gameObject.GetComponent<PuzzleCell>();
            m_Puzzle[cell.m_Row, cell.m_Column] = cell.m_CellNumber;
        }
    }
}