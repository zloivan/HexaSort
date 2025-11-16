using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Board
{
    public class TilesVisualSpawner : MonoBehaviour
    {
        [Serializable]
        private struct ColorMaterialPair
        {
            public TileColor TileColor;
            public Material Material;
        }

        [SerializeField] private List<ColorMaterialPair> _colorToMaterialList;
        [SerializeField] private TileVisual _prefab;

        public TileVisual SpawnTileVisual(ColoredTile tile)
        {
            var tileVisual = Instantiate(_prefab);
            var material = GetMaterial(tile.GetColor());
            tileVisual.Setup(material);

            return tileVisual;
        }

        public void DespawnTileVisual(TileVisual tileVisual)
        {
            if (tileVisual != null)
                Destroy(tileVisual.gameObject);
        }

        private Material GetMaterial(TileColor color) =>
            _colorToMaterialList.FirstOrDefault(cm => cm.TileColor == color).Material;
    }
}