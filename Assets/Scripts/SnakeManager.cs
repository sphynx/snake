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

    private float time;

    private bool gameOver;

    void Start()
    {
        float orthSize = Camera.main.orthographicSize;
        float aspectRatio = Camera.main.aspect;
        width = (int) (height * aspectRatio) + 2;

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

        time = 0f;
        gameOver = false;
    }

    void MoveSnake(Direction dir)
    {
        if (snake.Dir != Grid.InvertDirection(dir))
        {
            snake.Dir = dir;
            MoveResult moveResult = snake.Move();
            UpdateSnake(moveResult);
            time = 0f;
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
        var pos = new Vector3(col + 0.75f + margin, row + 1.5f + margin, 0f);

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
                gameOver = true;
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
        time += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (gameOver)
        {
            return;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSnake(Direction.Down);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSnake(Direction.Up);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSnake(Direction.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSnake(Direction.Right);
        }
        else if (time >= gameSpeed)
        {
            MoveSnake(snake.Dir);
        }
    }
}
