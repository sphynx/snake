using UnityEngine;

public class Tests : MonoBehaviour
{
    void Start()
    {
        Grid grid = new Grid(4, 3);
        grid.PrintGrid();
    }
}
