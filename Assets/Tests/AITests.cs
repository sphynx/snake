using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class AITests
{
    [Test]
    public void Test_2x2()
    {
        List<Cell> snake = new List<Cell>() { new Cell(0, 0) };
        T(2, 2, 1, 1, snake);
        T(2, 2, 0, 1, snake);
        T(2, 2, 1, 0, snake);
    }

    [Test]
    public void Test_3x3_1snake()
    {
        List<Cell> snake = new List<Cell>() { new Cell(0, 0) };
        T(3, 3, 0, 2, snake);
        T(3, 3, 2, 0, snake);
        T(3, 3, 1, 0, snake);
        T(3, 3, 0, 1, snake);
        T(3, 3, 1, 1, snake);
        T(3, 3, 2, 1, snake);
        T(3, 3, 1, 2, snake);
        T(3, 3, 2, 2, snake);
    }

    [Test]
    public void Test_3x3_2snake()
    {
        List<Cell> snake = new List<Cell>() { new Cell(0, 1), new Cell(0, 0) };
        T(3, 3, 0, 2, snake);
        T(3, 3, 1, 0, snake);
        T(3, 3, 1, 1, snake);
        T(3, 3, 1, 2, snake);
        T(3, 3, 2, 0, snake);
        T(3, 3, 2, 1, snake);
        T(3, 3, 2, 2, snake);
    }

    [Test]
    public void Test_3x2_1snake()
    {
        List<Cell> snake = new List<Cell>() { new Cell(0, 0) };
        T(3, 2, 1, 2, snake);
    }

    void T2(int w, int h, int goalRow, int goalCol, List<Cell> snake)
    {
        State state = new State(snake);
        AStar search = new AStar(state, w, h, new Cell(goalRow, goalCol));
        var moves = search.Search();
        Assert.NotNull(moves);
    }

    void T(int w, int h, int goalRow, int goalCol, List<Cell> snake)
    {
        State state = new State(snake);
        AStar search = new AStar(state, w, h, new Cell(goalRow, goalCol));
        var moves = search.Search();

        var snakeStr = string.Join(" <- ", snake.Select(c => $"{c.Row},{c.Col}"));

        if (moves == null)
        {
            Debug.Log($"{w}x{h}, goal: ({goalRow}, {goalCol}), {snakeStr}: No solution found");
        }
        else
        {
            var strategy = string.Join(',', moves.Select(m => m.ToString()));
            Debug.Log($"{w}x{h}, ({goalRow}, {goalCol}), {snakeStr}: Found moves: {strategy}");
        }

        Assert.NotNull(moves);
    }

}
