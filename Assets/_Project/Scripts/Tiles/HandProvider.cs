using System;
using System.Collections.Generic;
using HexSort.Board;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexSort.Tiles
{
    public class HandProvider : MonoBehaviour
    {
        private const int MIN_COLORS_IN_STACK = 1;
        private const int MAX_COLORS_IN_STACK = 4;

        private const int MAX_TILES_OF_SAME_COLOR_IN_STACK = 3;
        private const int MIN_TILES_OF_SAME_COLOR_IN_STACK = 1;

        private const int HAND_SIZE = 3;

        [SerializeField] private List<Transform> _spawnPositions;
        [SerializeField] private GameObject _stackPrefab;
        [SerializeField] private TilesVisualSpawner _tileVisualSpawner;
        
        private BoardGrid _boardGrid;
        private int _placedCount;

        private void Start()
        {
            GenerateNewStacks();
            _boardGrid = BoardGrid.Instance;
            
            _boardGrid.OnNewStackPlaced += BoardGrid_OnNewStackPlaced;
        }

        private void GenerateNewStacks()
        {
            _placedCount = 0;
            
            for (var i = 0; i < _spawnPositions.Count; i++)
            {
                var stackGo = Instantiate(_stackPrefab, _spawnPositions[i].position, Quaternion.identity);
                var stackVisual = stackGo.GetComponent<TileStack>();

                GenerateRandomStack(stackVisual);
            }
        }

        private void GenerateRandomStack(TileStack stackVisual)
        {
            var colorCount = Random.Range(MIN_COLORS_IN_STACK, MAX_COLORS_IN_STACK);
            var newTiles = new List<ColoredTile>();
            var colorsUsed = new HashSet<TileColor>();

            for (var i = 0; i < colorCount; i++)
            {
                //Don't allow multiple blocks of the same color in one stack
                TileColor tileColor;
                do
                {
                    tileColor = ColoredTileFactory.CreateRandomTile().GetColor();
                } while (!colorsUsed.Add(tileColor));

                var tilesOfThisColor = Random.Range(MIN_TILES_OF_SAME_COLOR_IN_STACK, MAX_TILES_OF_SAME_COLOR_IN_STACK);

                for (var j = 0; j < tilesOfThisColor; j++)
                {
                    newTiles.Add(ColoredTileFactory.CreateTile(tileColor));
                }
            }

            stackVisual.AddTiles(newTiles);
        }

        //EVENT HANDLERS ----------
        private void BoardGrid_OnNewStackPlaced(object sender, EventArgs e)
        {
            if (++_placedCount >= HAND_SIZE) 
                GenerateNewStacks();
        }
    }
}