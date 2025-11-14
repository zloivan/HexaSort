using System;
using UnityEngine;

namespace _Project.Scripts.Base
{
    public class GridSystemHex<TGridObject>
    {
        private readonly int _width;
        private readonly int _height;
        private readonly float _slotSize;
        private readonly TGridObject[,] _gridObjectArray;

        public GridSystemHex(int width, int height, float slotSize,
            Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> gridObjectBuilder)
        {
            _gridObjectArray = new TGridObject[width, height];
            _slotSize = slotSize;
            _width = width;
            _height = height;
            
            
            for (var x = 0; x < _width; x++)
            {
                for (var z = 0; z < _height; z++)
                {
                    var gridPosition = new GridPosition(x, z);
                    _gridObjectArray[x, z] = gridObjectBuilder(this, gridPosition);
                }
            }
        }

        public int GetHeight() =>
            _height;

        public int GetWidth() =>
            _width;

        public GridPosition GetGridPosition(Vector3 worldPosition) =>
            new(Mathf.RoundToInt(worldPosition.x / _slotSize),
                Mathf.RoundToInt(worldPosition.z / _slotSize));

        public Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            const float HEX_SLOT_HEIGHT_OFFSET_MULTIPLIER = 0.75f;
            return new Vector3(gridPosition.X, 0, 0) * _slotSize +
                   new Vector3(0, 0, gridPosition.Z) * _slotSize * HEX_SLOT_HEIGHT_OFFSET_MULTIPLIER
                   + (gridPosition.Z % 2 == 1 ? new Vector3(1, 0, 0) * _slotSize * 0.5f : Vector3.zero);
        }

        public TGridObject GetGridObject(GridPosition gridPos) =>
            _gridObjectArray[gridPos.X, gridPos.Z];
        
        public bool IsValidGridPosition(GridPosition gridPosition) =>
            gridPosition is { X: >= 0, Z: >= 0 } &&
            gridPosition.X < _width && gridPosition.Z < _height;

        public void CreateDebugObjects(Transform prefab, Transform parent)
        {
            for (var x = 0; x < _width; x++)
            {
                for (var z = 0; z < _height; z++)
                {
                    var gridPosition = new GridPosition(x, z);

                    var debugObj = UnityEngine.Object.Instantiate(prefab, GetWorldPosition(gridPosition),
                        Quaternion.identity,
                        parent);

                    debugObj.GetComponent<GridDebugObject>().SetGridObject(GetGridObject(gridPosition));
                }
            }
        }
    }
}

    