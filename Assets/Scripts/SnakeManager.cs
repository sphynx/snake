using System.Collections;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    [SerializeField]
    private int width = 32, height = 18; // so that it's 16 * 9 ratio :)

    private Snake snake;

    [SerializeField]
    private TilesCollection tilePrefabs;

    [SerializeField]
    private float tileScale = 0.8f;

    [SerializeField]
    private float margin = 1f;

    [SerializeField]
    private float gameSpeed = 0.3f;

    private GameObject[,] objectsGrid;

    void Start()
    {
        float orthSize = Camera.main.orthographicSize;
        float aspectRatio = Camera.main.aspect;

        // Set the camera's origin to bottom-left corner, so that
        // bottom-left of the screen is (0, 0) in the world.
        Camera.main.transform.position = new Vector3(aspectRatio * orthSize, orthSize, -10f);

        snake = new Snake(width, height, 2, 12, 7, Direction.Right, true);
        snake.SpawnApple(15, 15);

        var grid = snake.Grid;
        var scale = Vector3.one * tileScale;

        objectsGrid = new GameObject[height, width];

        for (int row = 0; row < grid.Height; row++)
        {
            for (int col = 0; col < grid.Width; col++)
            {
                var pos = new Vector3(col + 0.5f + margin, row + 0.5f + margin, 0f);
                GameObject prefab = tilePrefabs[grid.GetTile(row, col)];
                prefab.transform.localScale = scale;
                var obj = Instantiate(prefab, pos, Quaternion.identity);
                objectsGrid[row, col] = obj;
            }
        }

        StartCoroutine(nameof(MoveSnake));
    }

    IEnumerator MoveSnake()
    {
        while (true)
        {
            MoveResult moveResult = snake.Move();
            UpdateSnake(moveResult);

            yield return new WaitForSeconds(gameSpeed);
        }
    }

    void CreateTile(Cell cell)
    {
        var row = cell.Row;
        var col = cell.Col;
        var scale = Vector3.one * tileScale;

        GameObject prefab = tilePrefabs[snake.Grid.GetTile(row, col)];
        prefab.transform.localScale = scale;
        var pos = new Vector3(col + 0.5f + margin, row + 0.5f + margin, 0f);
        var newObj = Instantiate(prefab, pos, Quaternion.identity);
        objectsGrid[row, col] = newObj;
    }

    void DestroyTile(Cell cell)
    {
        var obj = objectsGrid[cell.Row, cell.Col];
        Destroy(obj);
        objectsGrid[cell.Row, cell.Col] = null;
    }

    void UpdateTile(Cell cell)
    {
        DestroyTile(cell);
        CreateTile(cell);
    }

    void UpdateSnake(MoveResult update)
    {
        foreach (Cell c in update.UpdateCells)
            UpdateTile(c);
    }

    private void Update()
    {
        var vert = Input.GetAxis("Vertical");
        var horiz = Input.GetAxis("Horizontal");
        var cancel = Input.GetAxis("Cancel");

        Debug.Log($"{vert}, {horiz}, {cancel}");

        if (cancel > 0)
            Application.Quit();
        else if (vert < 0 && horiz == 0)
            snake.Dir = Direction.Down;
        else if (vert > 0 && horiz == 0)
            snake.Dir = Direction.Up;
        else if (horiz < 0 && vert == 0)
            snake.Dir = Direction.Left;
        else if (horiz > 0 && vert == 0)
            snake.Dir = Direction.Right;
    }
}
