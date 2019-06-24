using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Search : MonoBehaviour
{
    public enum SearchAlgorithm { BreadthFirstSearch, DepthFirstSearch, IterativeDeepeningSearch, AStar }

    [Header("UI Management")]
    public TMP_Dropdown m_AlgorithmOptions;
    public Button m_SearchButton;
    public TextMeshProUGUI m_MessageText;

    // for UI activation control
    public GameObject m_StartPanel;
    public GameObject m_SolutionPanel;
    public GameObject m_ExitPanel;

    public Transform m_SolutionPathParent;
    
    [Header("Timing")]
    [Range(0.000000001f, 0.25f)]
    [Tooltip("A delay for tree expandation")]
    public float m_SearchDelayTime = 0.000000001f;  // this delay is for better presentation to audiences

    [Header("Puzzle")]
    public Puzzle m_StartState;
    public Puzzle m_GoalState;

    [Header("Tree")]
    [Tooltip("It must be existed in current scene")]
    public Node m_RootNode;
    [Tooltip("The latest node is the goal state")]
    public List<Node> m_PathToSolution;

    #region UI Management methods
    private void Awake()
    {
        List<string> options = new List<string>(Enum.GetNames(typeof(SearchAlgorithm)));

        // making space between camel-case words for displaying better result in dropdown menu using Regexp
        for (int i = 0; i < options.Count; i++)
            options[i] = Regex.Replace(options[i], "(\\B[A-Z])", " $1");

        m_ExitPanel.SetActive(false);

        // inserting SearchAlgorithm enum values into dropdown menu
        m_AlgorithmOptions.ClearOptions();
        m_AlgorithmOptions.AddOptions(options);

        // clearing message text for reuse
        ClearLog();

        SetUIActivity(true);
    }

    /*
     * this method will be called from a UI.Button, so it accessibility modifier must be public.
     * but it's cool if it called in another way, because it doesn't depends on ui actions.
     */
    public void StartSearching()
    {
        SetUIActivity(false);
        Log("Searching...", Color.white);

        // destroying previous children of root node
        DestroyChilrenNodes();

        Init();

        SearchAlgorithm algorithm = (SearchAlgorithm)m_AlgorithmOptions.value;

        switch (algorithm)
        {
            case SearchAlgorithm.BreadthFirstSearch:
                StartCoroutine(BreadthFirstSearch(m_RootNode));
                break;
            case SearchAlgorithm.DepthFirstSearch:
                StartCoroutine(DepthFirstSearch(m_RootNode));
                break;

            default:
                SetUIActivity(true);
                Log($"[{Regex.Replace(algorithm.ToString(), "(\\B[A-Z])", " $1")}] is not implemented yet!", Color.red);
                break;
        }
    }

    private void DestroyChilrenNodes()
    {
        m_RootNode.m_Children.Clear();
        m_PathToSolution.Clear();

        for (int i = 0; i < m_RootNode.transform.childCount; i++)
            Destroy(m_RootNode.transform.GetChild(i).gameObject);
    }

    private void SetUIActivity(bool active)
    {
        m_AlgorithmOptions.interactable = active;
        m_SearchButton.interactable = active;

        if(active)
            m_StartPanel.transform.Find("Lock").SetAsFirstSibling();
        else
            m_StartPanel.transform.Find("Lock").SetAsLastSibling();

    }

    private void Log(string message, Color logColor)
    {
        m_MessageText.color = logColor;
        m_MessageText.text = message;
    }

    private void ClearLog()
    {
        m_MessageText.text = "";
    }
    #endregion

    // start and goal state coordination
    private void Init()
    {
        // setting start state for root node
        m_RootNode.m_Puzzle.SetFrom(m_StartState.m_Puzzle);
        
        // setting goal state for Utilities.s_GoalState[,]
        Utilities.s_GoalState = new int[Utilities.s_PuzzleDimension, Utilities.s_PuzzleDimension];
        Utilities.s_GoalState.SetFrom(m_GoalState.m_Puzzle);
    }

    #region Tree Searching Algorithms
    // The searching algorithms are made by IEnumerator because of rendering order/queue problems

    #region BFS
    public IEnumerator BreadthFirstSearch(Node root)
    {
        if(root.IsGoalState())
        {
            Log("Goal Found!", Color.green);

            AudioManager.Instance.PlaySound(AudioManager.Instance.m_FoundAudio);
            SetUIActivity(true);
            PathTrace(root, m_PathToSolution);  // filling m_PathSolution with root node

            yield break;
        }

        List<Node> openList = new List<Node>();     // not searched
        List<Node> closedList = new List<Node>();   // searched

        openList.Add(root);
        bool isGoalFound = false;

        while (openList.Count > 0 && !isGoalFound)
        {
            Node currentNode = openList[0];
            closedList.Add(currentNode);
            openList.RemoveAt(0);

            currentNode.ExpandNode();

            // checking children of currentNode to finding goal state
            for (int i = 0; i < currentNode.m_Children.Count; i++)
            {
                Node currentChild = currentNode.m_Children[i];

                if (currentChild.IsGoalState())
                {
                    AudioManager.Instance.PlaySound(AudioManager.Instance.m_FoundAudio);
                    Log("Goal Found!", Color.green);

                    isGoalFound = true;

                    PathTrace(currentChild, m_PathToSolution); // trace path to root node
                }

                if (!Contains(openList, currentChild) && !Contains(closedList, currentChild))
                    openList.Add(currentChild);
            }

            yield return new WaitForSeconds(m_SearchDelayTime);
        }

        SetUIActivity(true);
    }
    #endregion

    #region DFS
    public IEnumerator DepthFirstSearch(Node root)
    {
        if (root.IsGoalState())
        {
            Log("Goal Found!", Color.green);

            AudioManager.Instance.PlaySound(AudioManager.Instance.m_FoundAudio);
            SetUIActivity(true);
            PathTrace(root, m_PathToSolution);  // filling m_PathSolution with root node

            yield break;
        }

        List<Node> openList = new List<Node>();     // not searched
        
        openList.Add(root);
        bool isGoalFound = false;
        
        while (openList.Count > 0 && !isGoalFound)
        {
            Node currentNode = openList[0];
            openList.RemoveAt(0);
            currentNode.ExpandNode();

            // checking the left children of currentNode to finding goal state
            Node currentChild = null;

            for (int i = 0; i < currentNode.m_Children.Count; i++)
            {
                if (currentNode.m_Parent != null)
                {
                    if (!currentNode.m_Children[i].IsSamePuzzle(currentNode.m_Parent.m_Puzzle))
                    {
                        currentChild = currentNode.m_Children[i];
                        break;
                    }
                }
                else
                {
                    currentChild = currentNode.m_Children[0];
                    break;
                }
            }

            if (currentChild.IsGoalState())
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.m_FoundAudio);
                Log("Goal Found!", Color.green);

                isGoalFound = true;

                PathTrace(currentChild, m_PathToSolution); // trace path to root node
            }

            //if (!Contains(openList, currentChild) && !Contains(closedList, currentChild))
            openList.Add(currentChild);

            yield return new WaitForSeconds(m_SearchDelayTime);
        }

        SetUIActivity(true);
    }
    #endregion
    #endregion

    /// <summary>
    /// tracing path from a child node to root node (start state)
    /// </summary>
    /// <param name="pathToRootNode">An empty list for finding path</param>
    /// <param name="node">The current node for path finding from parents</param>
    private void PathTrace(Node node, List<Node> pathToRootNode)
    {
        Node current = node;

        pathToRootNode.Add(current);

        // root node has no parent
        while (current.m_Parent != null)
        {
            current = current.m_Parent;
            pathToRootNode.Add(current);
        }

        pathToRootNode.Reverse(); // reversing the order of path from root node to child nodes
    }

    private static bool Contains(List<Node> list, Node node)
    {
        bool contains = false;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].IsSamePuzzle(node.m_Puzzle))
                contains = true;
        }

        return contains;
    }
}