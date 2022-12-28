using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum Tile : byte
{
    Empty,
    Wall,
    SnakeBody,
    SnakeHead,
    Apple,
    Obstacle,
}

public enum Direction : byte
{
    Left,
    Right,
    Up,
    Down,
}

public enum MoveResultType : byte
{
    Hit,
    Eat,
    Move,
}

public class MoveResult
{
    public MoveResult(MoveResultType type, Cell[] updatedCells)
    {
        Type = type;
        UpdateCells = updatedCells;
    }

    public MoveResultType Type { get; }
    public Cell[] UpdateCells { get; }
}

public readonly struct Cell
{
    public Cell(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public int Row { get; }
    public int Col { get; }

    public override string ToString() => $"[row={Row}, col={Col}]";
}

public class Grid
{
    private Tile[,] data;

    public int Width { get; }
    public int Height { get; }

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        data = new Tile[height, width];
    }

    public void SetTile(int row, int col, Tile t)
    {
        data[row, col] = t;
    }

    public void SetTile(Cell cell, Tile t)
    {
        SetTile(cell.Row, cell.Col, t);
    }

    public Tile GetTile(int row, int col)
    {
        return data[row, col];
    }

    public Tile GetTile(Cell cell)
    {
        return GetTile(cell.Row, cell.Col);
    }

    public static Cell ApplyDirection(Cell curr, Direction dir) => dir switch
    {
        Direction.Left => new Cell(curr.Row, curr.Col - 1),
        Direction.Right => new Cell(curr.Row, curr.Col + 1),
        Direction.Up => new Cell(curr.Row + 1, curr.Col),
        Direction.Down => new Cell(curr.Row - 1, curr.Col),
        _ => throw new ArgumentOutOfRangeException(nameof(curr)),
    };

    public static Direction InvertDirection(Direction dir) => dir switch
    {
        Direction.Left => Direction.Right,
        Direction.Right => Direction.Left,
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        _ => throw new ArgumentOutOfRangeException(nameof(dir)),
    };

    public void CheckBounds(Cell cell)
    {
        if (cell.Row < 0 || cell.Row >= Height)
            throw new IndexOutOfRangeException($"Cell row is out of bounds: {cell}");

        if (cell.Col < 0 || cell.Col >= Width)
            throw new IndexOutOfRangeException($"Cell col is out of bounds: {cell}");
    }
}

public class Snake
{
    private LinkedList<Cell> snakeCells;

    public Snake(int width, int height, int snakeHeadRow, int snakeHeadCol, int snakeLength, Direction dir, bool withWalls)
    {
        Grid = new Grid(width, height);
        Dir = dir;

        // Set up the Snake linked list.
        if (snakeLength < 1)
        {
            throw new ArgumentException("Snake length should be positive");
        }

        if (snakeHeadRow < 0 || snakeHeadRow >= height)
        {
            throw new ArgumentException("Snake should start within grid bounds");
        }

        if (snakeHeadCol < 0 || snakeHeadCol >= width)
        {
            throw new ArgumentException("Snake should start within grid bounds");
        }

        var cells = new LinkedList<Cell>();

        Cell currCell = new Cell(snakeHeadRow, snakeHeadCol);
        cells.AddLast(currCell);

        var invertedDir = Grid.InvertDirection(dir);
        for (int i = 0; i < snakeLength - 1; i++)
        {
            currCell = Grid.ApplyDirection(currCell, invertedDir);
            Grid.CheckBounds(currCell);
            cells.AddLast(currCell);
        }

        this.snakeCells = cells;

        // Apply the snake to the grid:
        foreach (Cell cell in snakeCells)
        {
            Grid.SetTile(cell, Tile.SnakeBody);
        }
        Grid.SetTile(snakeCells.First.Value, Tile.SnakeHead);

        // Draw outside walls if needed:
        if (withWalls)
        {
            for (int row = 0; row < height; row++)
            {
                Grid.SetTile(row, 0, Tile.Wall);
                Grid.SetTile(row, width - 1, Tile.Wall);
            }
            for (int j = 0; j < width; j++)
            {
                Grid.SetTile(0, j, Tile.Wall);
                Grid.SetTile(height - 1, j, Tile.Wall);
            }
        }
    }

    public Direction Dir { get; set; }
    public Grid Grid { get; }

    private Cell Head
    {
        get => snakeCells.First.Value;
    }

    public MoveResult Move()
    {
        var tile = Grid.GetTile(Head.Row, Head.Col);
        Assert.IsTrue(tile == Tile.SnakeHead);

        // Move the snake and update the grid.
        Cell newHead = Grid.ApplyDirection(Head, Dir);
        Cell oldHead = Head;
        Grid.CheckBounds(newHead);

        var newHeadTile = Grid.GetTile(newHead);

        if (newHeadTile == Tile.Empty || newHeadTile == Tile.Apple)
        {
            Grid.SetTile(newHead, Tile.SnakeHead);
            Grid.SetTile(Head, Tile.SnakeBody);
            snakeCells.AddFirst(newHead);
        }
        else
        {
            // Do not update the grid and just return that we've hit something.
            var updatedCells = new Cell[0];
            return new MoveResult(MoveResultType.Hit, updatedCells);
        }

        if (newHeadTile == Tile.Empty)
        {
            var tail = snakeCells.Last.Value;
            Grid.SetTile(tail, Tile.Empty);
            snakeCells.RemoveLast();

            var updatedCells = new Cell[] { newHead, oldHead, tail };
            return new MoveResult(MoveResultType.Move, updatedCells);
        }
        else
        {
            var updatedCells = new Cell[] { newHead, oldHead };
            return new MoveResult(MoveResultType.Eat, updatedCells);
        };
    }

    public void SpawnApple(int row, int col)
    {
        var cell = new Cell(row, col);
        Grid.CheckBounds(cell);
        Grid.SetTile(cell, Tile.Apple);
    }

    public Cell SpawnAppleInEmptyTile()
    {
        const int MAX_ATTEMPTS = 200;
        for (int i = 0; i < MAX_ATTEMPTS; i++)
        {
            var row = UnityEngine.Random.Range(0, Grid.Height);
            var col = UnityEngine.Random.Range(0, Grid.Width);

            if (Grid.GetTile(row, col) == Tile.Empty)
            {
                SpawnApple(row, col);
                return new Cell(row, col);
            }
        }

        throw new Exception($"Couldn't generate apple after {MAX_ATTEMPTS} attempts, " +
            $"something is wrong with the code");
    }
}
