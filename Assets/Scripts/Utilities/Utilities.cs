using UnityEngine;

public static class Utilities
{
    public static readonly int s_PuzzleDimension = 3;

    public static int[,] s_GoalState;

    public static void SetFrom<T>(this T[,] target, T[,] source)
    {
        if (source.GetLength(0) != target.GetLength(0))
        {
            Debug.LogError("size of target[,] is not equal to the source[,]!");
            return;
        }
        
        for (int i = 0; i < source.GetLength(0); i++)
            for (int j = 0; j < source.GetLength(1); j++)
                target[i, j] = source[i, j];
    }

    public static void Swap<T>(ref T target, ref T source)
    {
        T temp = target;
        target = source;
        source = temp;
    }

    public static int Pow(int number, int power)
    {
        return (int)Mathf.Pow(number, power);
    }
}