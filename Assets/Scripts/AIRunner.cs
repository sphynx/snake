using System;
using System.Collections.Generic;

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

        ICollection<Direction> moves = searchResult.Path;

        return moves;
    }
}
