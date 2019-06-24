using System.Collections.Generic;
using UnityEngine;

// Note 1: Arrays are reference types, methods can change the value of the elements.
// Note 2: List<T> are...

public class Node : MonoBehaviour
{
    // current puzzle of each node
    public int[,] m_Puzzle = new int[Utilities.s_PuzzleDimension, Utilities.s_PuzzleDimension];

    // graph connections
    public Node m_Parent;           // reference of parent node
    public List<Node> m_Children;   // list of children of current/each node
    
    public void ExpandNode()
    {
        int row = 0, column = 0;    // these two will pass to move methods

        m_Children = new List<Node>();

        for (int i = 0; i < Utilities.s_PuzzleDimension; i++)
            for (int j = 0; j < Utilities.s_PuzzleDimension; j++)
                if (m_Puzzle[i, j] == 0)
                {
                    row = i;
                    column = j;
                }
        
        MoveToRight(m_Puzzle, row, column);
        MoveToLeft(m_Puzzle, row, column);
        MoveToUp(m_Puzzle, row, column);
        MoveToDown(m_Puzzle, row, column);
    }

    private void MoveToRight(int[,] puzzle, int row, int column)
    {
        if (column >= 0 && column < Utilities.s_PuzzleDimension - 1)
        {
            int[,] possiblePuzzle = new int[Utilities.s_PuzzleDimension, Utilities.s_PuzzleDimension];
            possiblePuzzle.SetFrom(puzzle);

            Utilities.Swap(ref possiblePuzzle[row, column], ref possiblePuzzle[row, column + 1]);

            CreateChildNode("move_Right", transform, possiblePuzzle);
        }
    }

    private void MoveToLeft(int[,] puzzle, int row, int column)
    {
        if (column > 0 && column < Utilities.s_PuzzleDimension)
        {
            int[,] possiblePuzzle = new int[Utilities.s_PuzzleDimension, Utilities.s_PuzzleDimension];
            possiblePuzzle.SetFrom(puzzle);

            Utilities.Swap(ref possiblePuzzle[row, column], ref possiblePuzzle[row, column - 1]);

            CreateChildNode("move_Left", transform, possiblePuzzle);
        }
    }

    private void MoveToUp(int[,] puzzle, int row, int column)
    {
        if (row > 0 && row < Utilities.s_PuzzleDimension)
        {
            int[,] possiblePuzzle = new int[Utilities.s_PuzzleDimension, Utilities.s_PuzzleDimension];
            possiblePuzzle.SetFrom(puzzle);

            Utilities.Swap(ref possiblePuzzle[row, column], ref possiblePuzzle[row - 1, column]);

            CreateChildNode("move_Up", transform, possiblePuzzle);
        }
    }

    private void MoveToDown(int[,] puzzle, int row, int column)
    {
        if (row >= 0 && row < Utilities.s_PuzzleDimension - 1)
        {
            int[,] possiblePuzzle = new int[Utilities.s_PuzzleDimension, Utilities.s_PuzzleDimension];
            possiblePuzzle.SetFrom(puzzle);

            Utilities.Swap(ref possiblePuzzle[row, column], ref possiblePuzzle[row + 1, column]);

            CreateChildNode("move_Down", transform, possiblePuzzle);
        }
    }

    // comparing a puzzle with m_Puzzle
    public bool IsSamePuzzle(int[,] puzzle)
    {
        for (int i = 0; i < Utilities.s_PuzzleDimension; i++)
            for (int j = 0; j < Utilities.s_PuzzleDimension; j++)
                if (m_Puzzle[i, j] != puzzle[i, j])
                    return false;
        
        return true;
    }

    public bool IsGoalState()
    {
        for (int i = 0; i < Utilities.s_PuzzleDimension; i++)
            for (int j = 0; j < Utilities.s_PuzzleDimension; j++)
                if (m_Puzzle[i, j] != Utilities.s_GoalState[i, j])
                    return false;

        return true;
    }
    
    private void CreateChildNode(string name, Transform parent, int[,] puzzle)
    {
        GameObject obj = new GameObject(name, typeof(Node));
        obj.transform.parent = parent;

        Node child = obj.GetComponent<Node>();
        child.m_Parent = this;
        child.m_Puzzle.SetFrom(puzzle);

        m_Children.Add(child);
    }
}