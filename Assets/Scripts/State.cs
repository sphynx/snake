using System;
using System.Collections.Generic;
using System.Linq;

public class State : IEquatable<State>
{
    public static int width, height;
    public static Cell goal;

    private readonly LinkedList<Cell> snake;

    public State(ICollection<Cell> snake)
    {
        if (snake == null || snake.Count == 0)
        {
            throw new ArgumentException("Snake shouldn't be empty");
        }

        this.snake = new LinkedList<Cell>();
        foreach (Cell cell in snake)
        {
            this.snake.AddLast(cell);
        }
    }

    public override bool Equals(object obj)
    {
        if (!(obj is State)) return false;
        return Equals((State) obj);
    }

    public bool Equals(State other)
    {
        return snake.SequenceEqual(other.snake);
    }

    public override int GetHashCode()
    {
        unchecked // to disable overflow error and just truncate the result.
        {
            int hash = 19;
            foreach (var cell in snake)
            {
                hash = hash * 31 + cell.GetHashCode();
            }
            return hash;
        }
    }

    public static bool operator == (State s1, State s2)
    {
        return s1.Equals(s2);
    }

    public static bool operator != (State s1, State s2)
    {
        return !s1.Equals(s2);
    }

    public ICollection<Direction> Actions()
    {
        Direction[] directions = { Direction.Down, Direction.Up, Direction.Left, Direction.Right };
        List<Direction> actions = new List<Direction>();

        foreach (Direction dir in directions)
        {
            Cell nextHead = Grid.ApplyDirection(Head, dir);

            if (IsWithinBounds(nextHead) && !HitsItself(nextHead))
            {
                actions.Add(dir);
            };
        }

        return actions;
    }

    public static State ApplyAction(Direction dir, State s)
    {
        Cell newHead = Grid.ApplyDirection(s.Head, dir);

        if (!IsWithinBounds(newHead))
        {
            throw new Exception($"Wrong direction to apply: {dir}, " +
                $"getting out of bounds {width}x{height}, " +
                $"current head at: {s.Head}");
        }

        if (s.HitsItself(newHead))
        {
            throw new Exception($"Wrong direction to apply: {dir}, " +
                $"hitting itself at {newHead}, " +
                $"current head at: {s.Head}");
        }

        LinkedList<Cell> newSnake = new LinkedList<Cell>();
        newSnake.AddFirst(newHead);

        foreach (Cell c in s.snake)
        {
            newSnake.AddLast(c);
        }

        if (!newHead.Equals(goal))
        {
            newSnake.RemoveLast();
        }

        return new State(newSnake);
    }

    public bool IsGoal
    {
        get => Head.Equals(goal);
    }

    public Cell Head
    {
        get => snake.First.Value;
    }

    static bool IsWithinBounds(Cell cell)
    {
        return IsWithinBounds(cell.Row, cell.Col);
    }

    static bool IsWithinBounds(int row, int col)
    {
        return row >= 0 && col >= 0 && row < height && col < width;
    }

    bool HitsItself(Cell cell)
    {
        return snake.Contains(cell) /*&& !cell.Equals(snake.Last.Value)*/ ;
    }
}
