using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Board
{
    public class TileStackVisual : MonoBehaviour
    {
        [Serializable]
        private struct ColorMaterialPair
        {
            public TileColor TileColor;
            public Material Material;
        }

        
        // ------------------------------------
        private const float TILE_HEIGHT_OFFSET = 0.2f;

        [SerializeField] private List<ColorMaterialPair> _colorToMaterialList;
        [SerializeField] private TileStack _stack;
        [SerializeField] private GameObject _tilePrefab;

        private readonly List<GameObject> _tilesVisualList = new();

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
            for (var i = 0; i <  tiles.Count; i++)
            {
                
                //TODO: USE POOL AND MAYBE FACTORY, BUT NOT SURE
                var tileVisual = Instantiate(_tilePrefab, transform);
                tileVisual.transform.localPosition = Vector3.up * (i * TILE_HEIGHT_OFFSET);

                
                tileVisual.GetComponent<TileVisual>().Setup(GetMaterial(tiles[i].GetColor()));
                _tilesVisualList.Add(tileVisual);
            }
        }

        private void ClearVisuals()
        {
            foreach (var tile in _tilesVisualList)
            {
                Destroy(tile.gameObject);
            }
            
            _tilesVisualList.Clear();
        }

        private Material GetMaterial(TileColor color)
        {
            if (_tilesVisualList == null || _tilesVisualList.Count == 0)
            {
                return null;
            }

            var mat = _colorToMaterialList.FirstOrDefault(t => t.TileColor == color).Material;
            
            Debug.Assert(mat != null, "No material assigned to color");
            
            return mat;
        }
    }
}