using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeManager : MonoBehaviour
{
    [SerializeField]
    private int height;

    private int width; // determined based on the screen aspect ration

    private Snake snake;

    [SerializeField]
    private TilesCollection tilePrefabs;

    [SerializeField]
    private float tileScale = 0.9f;

    [SerializeField]
    private float margin = 0.5f;

    [SerializeField]
    private float gameSpeed = 0.15f;

    [SerializeField]
    private LevelUI levelUI;

    [SerializeField]
    private Score score, hiScore;

    [SerializeField]
    private AudioSource appleCrunch;

    [SerializeField]
    private AudioSource snakeHit;

    private GameObject[,] objectsGrid;

    void Start()
    {
        float orthSize = Camera.main.orthographicSize;
        float aspectRatio = Camera.main.aspect;
        width = (int) (height * aspectRatio);

        // Set the camera's origin to bottom-left corner, so that
        // bottom-left of the screen is (0, 0) in the world.
        Camera.main.transform.position = new Vector3(aspectRatio * orthSize, orthSize, -10f);

        snake = new Snake(width, height, 2, 12, 4, Direction.Right, true);
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

        Tile tile = snake.Grid.GetTile(row, col);
        GameObject prefab = tilePrefabs[tile];
        prefab.transform.localScale = scale;
        var pos = new Vector3(col + 0.5f + margin, row + 1.5f + margin, 0f);

        Quaternion q = Quaternion.identity;
        if (tile == Tile.SnakeHead)
        {
            q = snake.Dir switch
            {
                Direction.Up => Quaternion.Euler(0, 0, 90),
                Direction.Down => Quaternion.Euler(0, 0, -90),
                Direction.Left => Quaternion.Euler(0, 0, 180),
                Direction.Right => Quaternion.identity,
                _ => throw new System.Exception($"Wrong direction: {snake.Dir}"),
            };
        }

        var newObj = Instantiate(prefab, pos, q);
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
                appleCrunch.Play();
                score.Value = score.Value + 1;
                levelUI.SetScore(score.Value);
                Cell appleCell = snake.SpawnAppleInEmptyTile();
                UpdateTile(appleCell);
                gameSpeed *= 0.9f;
                break;

            case MoveResultType.Hit:
                snakeHit.Play();
                StopCoroutine(nameof(MoveSnake));
                Invoke(nameof(GameOver), 0.5f);
                break;
        };
    }

    void GameOver()
    {
        SceneManager.LoadScene("GameOver");
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
