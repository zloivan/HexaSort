using System;
using System.Collections.Generic;
using HexSort.Board;
using HexSort.Tiles.Testing.HexSort.Tiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexSort.Tiles
{
    public class HandProvider : MonoBehaviour
    {
        private const int MIN_COLORS_IN_STACK = 1;
        private const int MAX_COLORS_IN_STACK = 3;

        private const int MAX_TILES_OF_SAME_COLOR_IN_STACK = 5;
        private const int MIN_TILES_OF_SAME_COLOR_IN_STACK = 1;

        private const int HAND_SIZE = 3;

        [Header("DEBUG")]
        [SerializeField] private bool _useMockData = false;

        [SerializeField] private MockDataSet _mockDataSet;


        [SerializeField] private List<Transform> _spawnPositions;
        [SerializeField] private GameObject _stackPrefab;
        [SerializeField] private TilesVisualSpawner _tileVisualSpawner;

        private BoardGrid _boardGrid;
        private int _placedCount;
        private int _currentMockIndex;

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
                var tileStack = stackGo.GetComponent<TileStack>();

                if (!_useMockData && _mockDataSet != null)
                {
                    GenerateRandomStack(tileStack);
                }
                else
                {
                    GenerateMockStack(tileStack);
                }
            }
        }

        private void GenerateRandomStack(TileStack tileStack)
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

            tileStack.AddTiles(newTiles);
        }

        private void GenerateMockStack(TileStack tileStack)
        {
            if (_mockDataSet._stacks.Count == 0)
            {
                Debug.LogWarning("Mock data set is empty! Falling back to random generation.");
                GenerateRandomStack(tileStack);
                return;
            }

            var mockStack = _mockDataSet._stacks[_currentMockIndex];
            var tiles = new List<ColoredTile>();

            foreach (var colorBlock in mockStack.ColorBlocks)
            {
                for (int i = 0; i < colorBlock.Count; i++)
                {
                    tiles.Add(ColoredTileFactory.CreateTile(colorBlock.Color));
                }
            }

            tileStack.AddTiles(tiles);

            _currentMockIndex = (_currentMockIndex + 1) % _mockDataSet._stacks.Count;

            Debug.Log($"[HandProvider] Generate mock stack #{_currentMockIndex - 1}: {mockStack.Description}");
        }

        private void BoardGrid_OnNewStackPlaced(object sender, EventArgs e)
        {
            if (++_placedCount >= HAND_SIZE)
                GenerateNewStacks();
        }
    }
}