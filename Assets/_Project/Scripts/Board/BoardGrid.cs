using System;
using System.Collections.Generic;
using _Project.Scripts.Base;
using UnityEngine;

namespace _Project.Scripts.Board
{
    public class BoardGrid : MonoBehaviour
    {
        private const float SLOT_SIZE = 2f;
        [SerializeField] private int _width;
        [SerializeField] private int _height;

        private GridSystemHex<GridObject> _gridSystemHex;

        private void Awake()
        {
            _gridSystemHex =
                new GridSystemHex<GridObject>(
                    _width,
                    _height,
                    SLOT_SIZE,
                    (grid, pos) => new GridObject(grid, pos));
        }

        public int GetWidth() =>
            _gridSystemHex.GetWidth();

        public int GetHeight() =>
            _gridSystemHex.GetHeight();

        public Vector3 GetWorldPosition(GridPosition targetGridPosition) =>
            _gridSystemHex.GetWorldPosition(targetGridPosition);

        public bool IsValidGridPosition(GridPosition gridPosition) =>
            _gridSystemHex.IsValidGridPosition(gridPosition);

        public GridPosition GetGridPosition(Vector3 worldPointerPosition) =>
            _gridSystemHex.GetGridPosition(worldPointerPosition);

        public List<GridPosition> GetAllGridPositions()
        {
            var result = new List<GridPosition>();
            for (var x = 0; x < _gridSystemHex.GetWidth(); x++)
            {
                for (var z = 0; z < _gridSystemHex.GetHeight(); z++)
                {
                    result.Add(new GridPosition(x, z));
                }
            }

            return result;
        }
    }
}