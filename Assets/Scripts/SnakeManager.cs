using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeManager : MonoBehaviour
{
    [SerializeField]
    private int width = 33, height = 18; // so that it's 16 * 9 ratio :)

    private Snake snake;

    [SerializeField]
    private TilesCollection tilePrefabs;

    [SerializeField]
    private float tileScale = 0.8f;

    [SerializeField]
    private float margin = 1f;

    [SerializeField]
    private float gameSpeed = 0.3f;

    [SerializeField]
    private LevelUI levelUI;

    [SerializeField]
    private Score score, hiScore;

    private GameObject[,] objectsGrid;

    void Start()
    {
        float orthSize = Camera.main.orthographicSize;
        float aspectRatio = Camera.main.aspect;

        // Set the camera's origin to bottom-left corner, so that
        // bottom-left of the screen is (0, 0) in the world.
        Camera.main.transform.position = new Vector3(aspectRatio * orthSize, orthSize, -10f);

        snake = new Snake(width, height, 2, 12, 3, Direction.Right, true);
        snake.SpawnApple(15, 15);

        objectsGrid = new GameObject[height, width];

        for (int row = 0; row < snake.Grid.Height; row++)
        {
            for (int col = 0; col < snake.Grid.Width; col++)
            {
                CreateTile(new Cell(row, col));
            }
        }

        score.Value = 0;
        levelUI.SetScore(score.Value);

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
        var pos = new Vector3(col + 0.5f + margin, row + 1.5f + margin, 0f);
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

        switch (update.Type)
        {
            case MoveResultType.Eat:
                score.Value = score.Value + 1;
                levelUI.SetScore(score.Value);
                Cell appleCell = snake.SpawnAppleInEmptyTile();
                UpdateTile(appleCell);
                break;

            case MoveResultType.Hit:
                SceneManager.LoadScene("GameOver");
                break;
        };
    }

    void Update()
    {
        var vert = Input.GetAxis("Vertical");
        var horiz = Input.GetAxis("Horizontal");
        var cancel = Input.GetAxis("Cancel");

        if (cancel > 0)
            Application.Quit();
        else if (vert < 0 && horiz == 0)
            SetSnakeDirection(Direction.Down);
        else if (vert > 0 && horiz == 0)
            SetSnakeDirection(Direction.Up);
        else if (horiz < 0 && vert == 0)
            SetSnakeDirection(Direction.Left);
        else if (horiz > 0 && vert == 0)
            SetSnakeDirection(Direction.Right);
    }

    void SetSnakeDirection(Direction dir)
    {
        // Disable moving in oppposite direction.
        // (Note: we could also allow it and reverse the whole snake,
        // making its tail a new head, but that is somewhat unconventional).
        if (snake.Dir != Grid.InvertDirection(dir))
        {
            snake.Dir = dir;
        }
    }
}
