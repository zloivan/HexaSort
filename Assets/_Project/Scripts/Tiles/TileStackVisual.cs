using System.Collections.Generic;
using UnityEngine;

namespace HexSort.Tiles
{
    public class TileStackVisual : MonoBehaviour
    {
        private const float TILE_HEIGHT_OFFSET = 0.2f;

        [SerializeField] private TilesVisualSpawner _tileSpawner;
        [SerializeField] private TileStack _stack;

        private readonly List<TileVisual> _tilesVisualList = new();

        private void LateUpdate()
        {
            if (!_stack.GetIsDirty())
                return;

            RebuildVisuals();
            _stack.ClearDirtyFlag();
        }

        private void RebuildVisuals()
        {
            ClearVisuals();

            var tiles = _stack.GetAllTiles();
            for (var i = 0; i < tiles.Count; i++)
            {
                var tileVisual = _tileSpawner.SpawnTileVisual(tiles[i]);
                tileVisual.transform.SetParent(transform);
                tileVisual.transform.localPosition = Vector3.up * (i * TILE_HEIGHT_OFFSET);
                _tilesVisualList.Add(tileVisual);
            }
        }

        private void ClearVisuals()
        {
            foreach (var tile in _tilesVisualList)
            {
                _tileSpawner.DespawnTileVisual(tile);
            }

            _tilesVisualList.Clear();
        }
    }
}