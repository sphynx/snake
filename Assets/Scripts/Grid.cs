using UnityEngine;

public enum Tile : byte
{
    Empty,
    Wall,
    SnakeBody,
    SnakeHead,
    Apple,
    Obstacle,
}

public class Grid
{
    private int width;
    private int height;
    private Tile[,] data;

    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }

    public Grid(int width, int height)
    {
        this.width = width;
        this.height = height;
        this.data = new Tile[height, width];
    }

    public void SetTile(int row, int col, Tile t)
    {
        this.data[row, col] = t;
    }

    public Tile GetTile(int row, int col)
    {
        return this.data[row, col];
    }

    public void PrintGrid()
    {
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                Debug.Log($"{i}, {j} -> {data[i, j]}");
            }
        }
    }
}
