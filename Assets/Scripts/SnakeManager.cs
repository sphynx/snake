using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    [SerializeField]
    private int width = 32, height = 18; // so that it's 16 * 9 ratio :)

    private Grid grid;

    [SerializeField]
    private TilesCollection tilePrefabs;

    void Start()
    {
        float orthSize = Camera.main.orthographicSize;
        float aspectRatio = Camera.main.aspect;

        float margin = 1f;
        float scale = 0.8f;

        // Set the camera's origin to bottom-left corner, so that bottom-left of the screen
        // is (0, 0) in the world.            
        Camera.main.transform.position = new Vector3(aspectRatio * orthSize, orthSize, -10f);

        grid = new Grid(width, height);

        // Build walls around the map:
        for (int row = 0; row < height; row++)
        {
            grid.SetTile(row, 0, Tile.Wall);
            grid.SetTile(row, width - 1, Tile.Wall);
        }
        for (int j = 0; j < width; j++)
        {
            grid.SetTile(0, j, Tile.Wall);
            grid.SetTile(height - 1, j, Tile.Wall);
        }

        // Starting snake
        grid.SetTile(2, 10, Tile.SnakeBody);
        grid.SetTile(2, 11, Tile.SnakeBody);
        grid.SetTile(2, 12, Tile.SnakeHead);

        // Starting apple
        grid.SetTile(4, 16, Tile.Apple);

        var scaleVec = Vector3.one * scale;

        for (int row = 0; row < grid.Height; row++)
        {
            for (int col = 0; col < grid.Width; col++)
            {
                var pos = new Vector3(col + 0.5f + margin, row + 0.5f + margin, 0f);
                GameObject prefab = tilePrefabs[grid.GetTile(row, col)];
                prefab.transform.localScale = scaleVec;
                Instantiate(prefab, pos, Quaternion.identity);
            }
        }
    }

    void Update()
    {
        
    }
}
