using HexSort.Tiles;

namespace HexSort.Grid
{
    public class GridObject
    {
        private readonly GridPosition _gridPosition;
        private readonly GridSystemHex<GridObject> _gridSystemHex;
        private TileStack _stack;
        
        public GridObject(GridSystemHex<GridObject> gridSystemHex, GridPosition gridPosition)
        {
            _gridPosition = gridPosition;
            _gridSystemHex = gridSystemHex;
        }

        public void SetStack(TileStack stack) =>
            _stack = stack;

        public TileStack GetStack() =>
            _stack;

        public void ClearStack()
        {
            _stack = null;
        }

        public override string ToString() =>
            $"{_gridPosition}\n " + _stack;
    }
}