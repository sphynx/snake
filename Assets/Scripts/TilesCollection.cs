using System;
using System.Collections.Generic;
using UnityEngine;

// Based on:
// https://gamedev.stackexchange.com/questions/169192/select-unity-prefab-from-enum-choice

[CreateAssetMenu]
public class TilesCollection : ScriptableObject
{
    [Serializable]
    public class TilePrefab
    {
        public GameObject prefab;
        public Tile tile;
    }

    // This is what we set up from the Scriptable Object UI.
    // The mapping between tiles and their prefabs.
    public TilePrefab[] AllPrefabs;

    // Internal data.
    private Dictionary<Tile, GameObject> data;

    // Overrides indexing operator.
    public GameObject this[Tile tile]
    {
        get
        {
            Init();
            return data[tile];
        }
    }

    private void Init()
    {
        if (data != null)
            return;

        data = new Dictionary<Tile, GameObject>();
        foreach (var tilePrefab in AllPrefabs)
        {
            data[tilePrefab.tile] = tilePrefab.prefab;
        }
    }
}
