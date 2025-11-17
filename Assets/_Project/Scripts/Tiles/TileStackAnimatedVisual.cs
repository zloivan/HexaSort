using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexSort.Tiles
{
    public class TileStackAnimatedVisual : MonoBehaviour
    {
        private const float TILE_HEIGHT_OFFSET = 0.2f;
        private const float TILE_SPAWN_DELAY = 0.1f;
        private const float TILE_ANIMATION_DURATION = 0.2f;

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


            var tilesAdded = GetAddedTiles(_previousTiles, currentTiles);

            if (tilesAdded.Count > 0)
            {
                if (_animationCoroutine != null)
                    StopCoroutine(_animationCoroutine);

                _animationCoroutine = StartCoroutine(AnimateNewTiles(currentTiles, tilesAdded));
            }
            else
            {
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


            var newTilesCount = current.Count - previous.Count;
            for (var i = current.Count - newTilesCount; i < current.Count; i++)
            {
                added.Add(current[i]);
            }

            return added;
        }

        private IEnumerator AnimateNewTiles(List<ColoredTile> allTiles, List<ColoredTile> newTiles)
        {
            var baseIndex = allTiles.Count - newTiles.Count;


            var newVisuals = new List<TileVisual>();
            for (var i = 0; i < newTiles.Count; i++)
            {
                var tileVisual = _tileSpawner.SpawnTileVisual(newTiles[i]);
                tileVisual.transform.SetParent(transform);

                var targetIndex = baseIndex + i;
                var targetPosition = Vector3.up * (targetIndex * TILE_HEIGHT_OFFSET);
                var startPosition = targetPosition + Vector3.down * 2f;

                tileVisual.transform.localPosition = startPosition;
                newVisuals.Add(tileVisual);
                _tilesVisualList.Add(tileVisual);
            }


            for (int i = 0; i < newVisuals.Count; i++)
            {
                var tileVisual = newVisuals[i];
                var targetIndex = baseIndex + i;
                var targetPosition = Vector3.up * (targetIndex * TILE_HEIGHT_OFFSET);

                StartCoroutine(AnimateTileToPosition(tileVisual, targetPosition, TILE_ANIMATION_DURATION));


                if (i < newVisuals.Count - 1)
                    yield return new WaitForSeconds(TILE_SPAWN_DELAY);
            }
        }

        private IEnumerator AnimateTileToPosition(TileVisual tile, Vector3 targetPosition, float duration)
        {
            var tileTransform = tile.transform;
            var startPosition = tileTransform.localPosition;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / duration;

                t = 1f - Mathf.Pow(1f - t, 3);
        
                if (tileTransform == null)
                    yield break;
        
                tileTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            if (tileTransform != null)
                tileTransform.localPosition = targetPosition;
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