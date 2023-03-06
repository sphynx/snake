using System;
using System.Collections.Generic;
using UnityEngine;

public static class AIRunner
{
    public static ICollection<Direction> GetMoves(int gridWidth, int gridHeight, ICollection<Cell> snake, Cell apple)
    {
        Cell adjustedApple = new Cell(apple.Row - 1, apple.Col - 1);

        List<Cell> adjustedSnake = new List<Cell>(snake.Count);
        foreach (Cell cell in snake)
        {
            adjustedSnake.Add(new Cell(cell.Row - 1, cell.Col - 1));
        }

        State state = new State(adjustedSnake);
        AStar search = new AStar(state, gridWidth - 2 , gridHeight - 2, adjustedApple);
        SearchResult searchResult = search.Search();
        AIStatistics.AddResult(searchResult);

        ICollection<Direction> moves = searchResult.Path;
        return moves;
    }
}

public static class AIStatistics
{
    static readonly List<SearchResult> results = new List<SearchResult>(100);

    public static void AddResult(SearchResult result)
    {
        results.Add(result);
    }

    public static void Clear()
    {
        results.Clear();
    }

    public static void PrintStats()
    {
        int numberOfResults = results.Count;

        float totalElapsed = 0f;
        float totalExplored = 0f;
        int maxExplored = -1;
        float avgPosPerMs = 0f;

        foreach (SearchResult res in results)
        {
            float ms = (float) res.ElapsedMicroSeconds / 1000f;
            totalElapsed += (float) ms;
            totalExplored += (float) res.ExploredStates;
            if (res.ExploredStates > maxExplored)
                maxExplored = res.ExploredStates;
            avgPosPerMs += (float) res.ExploredStates / ms;
        }

        Debug.Log($"avg positions/ms: {totalExplored / totalElapsed}\n" +
            $"another avg positions/ms: {avgPosPerMs / numberOfResults}\n" +
            $"max positions per problem: {maxExplored}\n" +
            $"avg positions/problem: {totalExplored / numberOfResults}\n" +
            $"# results: {numberOfResults}\n"
        );
    }
}
