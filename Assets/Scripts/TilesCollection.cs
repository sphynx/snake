using System.Collections.Generic;
using UnityEngine;

// Based on:
// https://gamedev.stackexchange.com/questions/169192/select-unity-prefab-from-enum-choice

[CreateAssetMenu]
public class TilesCollection : ScriptableObject
{
    [System.Serializable]
    public class TilePrefab
    {
        public GameObject prefab;
        public Tile tile;
    }

    // This is what we set up from the Scriptable Object UI.
    // The mapping between tiles and their prefabs.
    public TilePrefab[] AllPrefabs;

    // Internal data.
    private Dictionary<Tile, List<GameObject>> data;

    // Overrides indexing operator.
    public GameObject this[Tile tile]
    {
        get
        {
            Init();

            var tiles = data[tile];
            int randomIx = Random.Range(0, tiles.Count);
            return tiles[randomIx];
        }
    }

    private void Init()
    {
        if (data != null)
            return;

        data = new Dictionary<Tile, List<GameObject>>();
        foreach (var tilePrefab in AllPrefabs)
        {
            if (data.ContainsKey(tilePrefab.tile))
            {
                data[tilePrefab.tile].Add(tilePrefab.prefab);
            }
            else
            {
                var prefabs = new List<GameObject>();
                prefabs.Add(tilePrefab.prefab);
                data[tilePrefab.tile] = prefabs;
            }
        }
    }
}
