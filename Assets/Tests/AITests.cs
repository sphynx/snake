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
        HasPath(2, 2, 1, 1, snake);
        HasPath(2, 2, 0, 1, snake);
        HasPath(2, 2, 1, 0, snake);
    }

    [Test]
    public void Test_2x2_2snake()
    {
        List<Cell> snake = new List<Cell>() { new Cell(0, 0), new Cell(1, 0) };
        HasPath(2, 2, 1, 1, snake);
        HasPath(2, 2, 0, 1, snake);
    }

    [Test]
    public void Test_3x3_1snake()
    {
        List<Cell> snake = new List<Cell>() { new Cell(0, 0) };
        HasPath(3, 3, 0, 2, snake);
        HasPath(3, 3, 2, 0, snake);
        HasPath(3, 3, 1, 0, snake);
        HasPath(3, 3, 0, 1, snake);
        HasPath(3, 3, 1, 1, snake);
        HasPath(3, 3, 2, 1, snake);
        HasPath(3, 3, 1, 2, snake);
        HasPath(3, 3, 2, 2, snake);
    }

    [Test]
    public void Test_3x3_2snake()
    {
        List<Cell> snake = new List<Cell>() { new Cell(0, 1), new Cell(0, 0) };
        HasPath(3, 3, 0, 2, snake);
        HasPath(3, 3, 1, 0, snake);
        HasPath(3, 3, 1, 1, snake);
        HasPath(3, 3, 1, 2, snake);
        HasPath(3, 3, 2, 0, snake);
        HasPath(3, 3, 2, 1, snake);
        HasPath(3, 3, 2, 2, snake);
    }

    [Test]
    public void Test_3x3_3snake_turns()
    {
        List<Cell> snake = new List<Cell>() { new Cell(0, 2), new Cell(1, 2), new Cell(1, 1) };
        var path = HasPath(3, 3, 2, 2, snake);
        Assert.AreEqual(4, path.Count);

        HasPath(3, 3, 2, 1, snake);
        HasPath(3, 3, 2, 0, snake);
        HasPath(3, 3, 1, 0, snake);
        HasPath(3, 3, 0, 0, snake);
        HasPath(3, 3, 0, 1, snake);
    }

    [Test]
    public void Test_3x2_1snake()
    {
        List<Cell> snake = new List<Cell>() { new Cell(0, 0) };
        HasPath(3, 2, 1, 2, snake);
    }

    [Test]
    public void Test_LongDistance()
    {
        List<Cell> snake = new List<Cell>() { new Cell(0, 0) };
        var path = HasPath(1000, 1000, 999, 999, snake);
        Assert.AreEqual(2 * 999, path.Count);
    }

    /*
    3x3 board with a situation like this (H=head, A=apple, *=body, .=empty_space).
    The bot shouldn't get the apple immediately, because it will be dead next turn.

     . . .

     * *-H
     | |
     *-* A

     */
    [Test]
    public void Test_TemptationOfStAnthony()
    {
        List<Cell> snake = new List<Cell>() { new Cell(1, 2), new Cell(1, 1), new Cell(0, 1), new Cell(0, 0), new Cell(1, 0) };
        var path = HasPath(3, 3, 0, 2, snake);
        Assert.That(path.Count > 1);
    }

    /*
     . A .

     . *-*
       | |
     . * H

     */
    [Test]
    public void Test_SeesExit()
    {
        List<Cell> snake = new List<Cell>() { new Cell(0, 2), new Cell(1, 2), new Cell(1, 1), new Cell(0, 1) };
        var path = HasPath(3, 3, 2, 1, snake);
        Assert.AreEqual(3, path.Count);
    }

    ICollection<Direction> HasPath(int w, int h, int goalRow, int goalCol, List<Cell> snake)
    {
        State state = new State(snake);
        AStar search = new AStar(state, w, h, new Cell(goalRow, goalCol));
        var moves = search.Search().Path;

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

        return moves;
    }
}
