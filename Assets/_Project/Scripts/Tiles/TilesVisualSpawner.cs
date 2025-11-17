using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexSort.Tiles
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

        //TODO: Use pool here
        public TileVisual SpawnTileVisual(ColoredTile tile)
        {
            var tileVisual = Instantiate(_prefab);
            var material = GetMaterial(tile.GetColor());
            tileVisual.Setup(material);

            return tileVisual;
        }

        //TODO: Use pool here
        public void DespawnTileVisual(TileVisual tileVisual)
        {
            if (tileVisual != null)
                Destroy(tileVisual.gameObject);
        }

        private Material GetMaterial(TileColor color) =>
            _colorToMaterialList.FirstOrDefault(cm => cm.TileColor == color).Material;
    }
}