using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexSort.Tiles
{
    public class TileStackAnimatedVisual : MonoBehaviour
    {
        private const float TILE_HEIGHT_OFFSET = 0.2f;
        private const float TILE_SPAWN_DELAY = 0.1f; // Delay between each tile animation
        private const float TILE_ANIMATION_DURATION = 0.2f; // Duration of each tile's movement

        [SerializeField] private TilesVisualSpawner _tileSpawner;
        [SerializeField] private TileStack _stack;

        private readonly List<TileVisual> _tilesVisualList = new();
        private readonly List<ColoredTile> _previousTiles = new();
        private Coroutine _animationCoroutine;

        private void LateUpdate()
        {
            if (!_stack.GetIsDirty())
                return;

            var currentTiles = _stack.GetAllTiles();

            // Detect which tiles were added
            var tilesAdded = GetAddedTiles(_previousTiles, currentTiles);

            if (tilesAdded.Count > 0)
            {
                // Animate new tiles being added
                if (_animationCoroutine != null)
                    StopCoroutine(_animationCoroutine);

                _animationCoroutine = StartCoroutine(AnimateNewTiles(currentTiles, tilesAdded));
            }
            else
            {
                // Tiles were removed, rebuild instantly
                RebuildVisualsInstantly(currentTiles);
            }

            _previousTiles.Clear();
            _previousTiles.AddRange(currentTiles);
            _stack.ClearDirtyFlag();
        }

        private List<ColoredTile> GetAddedTiles(List<ColoredTile> previous, List<ColoredTile> current)
        {
            var added = new List<ColoredTile>();

            if (current.Count <= previous.Count)
                return added;

            // The new tiles are at the end of the list
            var newTilesCount = current.Count - previous.Count;
            for (int i = current.Count - newTilesCount; i < current.Count; i++)
            {
                added.Add(current[i]);
            }

            return added;
        }

        private IEnumerator AnimateNewTiles(List<ColoredTile> allTiles, List<ColoredTile> newTiles)
        {
            var baseIndex = allTiles.Count - newTiles.Count;

            // Spawn all new tiles at once but below their target position
            var newVisuals = new List<TileVisual>();
            for (int i = 0; i < newTiles.Count; i++)
            {
                var tileVisual = _tileSpawner.SpawnTileVisual(newTiles[i]);
                tileVisual.transform.SetParent(transform);

                var targetIndex = baseIndex + i;
                var targetPosition = Vector3.up * (targetIndex * TILE_HEIGHT_OFFSET);
                var startPosition = targetPosition + Vector3.down * 2f; // Start below

                tileVisual.transform.localPosition = startPosition;
                newVisuals.Add(tileVisual);
                _tilesVisualList.Add(tileVisual);
            }

            // Animate each tile with a delay
            for (int i = 0; i < newVisuals.Count; i++)
            {
                var tileVisual = newVisuals[i];
                var targetIndex = baseIndex + i;
                var targetPosition = Vector3.up * (targetIndex * TILE_HEIGHT_OFFSET);

                StartCoroutine(AnimateTileToPosition(tileVisual, targetPosition, TILE_ANIMATION_DURATION));

                // Wait before animating next tile
                if (i < newVisuals.Count - 1)
                    yield return new WaitForSeconds(TILE_SPAWN_DELAY);
            }
        }

        private IEnumerator AnimateTileToPosition(TileVisual tile, Vector3 targetPosition, float duration)
        {
            var startPosition = tile.transform.localPosition;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / duration;

                // Smooth ease-out animation
                t = 1f - Mathf.Pow(1f - t, 3);

                tile.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            tile.transform.localPosition = targetPosition;
        }

        private void RebuildVisualsInstantly(List<ColoredTile> tiles)
        {
            ClearVisuals();

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
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }

            foreach (var tile in _tilesVisualList)
            {
                Destroy(tile.gameObject);
            }

            _tilesVisualList.Clear();
        }
    }
}