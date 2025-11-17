using System.Collections.Generic;
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

        private readonly List<GameObject> _currentStack = new();

        private void Start()
        {
            GenerateNewStacks();
        }

        private void GenerateNewStacks()
        {
            ClearCurrentStack();

            for (var i = 0; i < _spawnPositions.Count; i++)
            {
                var stackGo = Instantiate(_stackPrefab, _spawnPositions[i].position, Quaternion.identity);
                var stackVisual = stackGo.GetComponent<TileStack>();

                GenerateRandomStack(stackVisual);
                _currentStack.Add(stackGo);
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

        private void ClearCurrentStack()
        {
            foreach (var stackGo in _currentStack)
            {
                Destroy(stackGo);
            }

            _currentStack.Clear();
        }
    }
}