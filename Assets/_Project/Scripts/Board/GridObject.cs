using System.Collections.Generic;
using _Project.Scripts.Base;
using UnityEngine;

namespace _Project.Scripts.Board
{
    public class  GridObject
    {
        private readonly GridPosition _gridPosition;
        private readonly GridSystemHex<GridObject> _gridSystemHex;
        private readonly TileStack _stack;
        
        public GridObject(GridSystemHex<GridObject> gridSystemHex, GridPosition gridPosition)
        {
            _gridPosition = gridPosition;
            _gridSystemHex = gridSystemHex;
            _stack = new TileStack();
        }

        public override string ToString() =>
            $"{_gridPosition}\n " + _stack;
    }
}