using HexSort.Board;
using HexSort.Grid;
using HexSort.Tiles;

namespace HexSort.MergeSystem
{
    public static class MergeExecutor
    {
        private const int DESTRUCTION_THRESHOLD = 10;

        public static void ExecuteMerge(MergeOperation operation, BoardGrid grid)
        {
            var fromStack = grid.GetStackAt(operation.From);
            var toStack = grid.GetStackAt(operation.To);

            if (fromStack == null || toStack == null)
            {
                return;
            }
            
            fromStack.MoveTo(toStack, operation.TileCount);
        }

        public static bool CheckAndDestroyStack(GridPosition position, BoardGrid grid)
        {
            var stack = grid.GetStackAt(position);

            if (stack == null)
            {
                return false;
            }

            if (!ShouldDestroy(stack))
            {
                return false;
            }

            grid.RemoveStackAtPosition(position);
            return true;
        }

        private static bool ShouldDestroy(TileStack stack) =>
            stack.GetAllTiles().Count >= DESTRUCTION_THRESHOLD && stack.GetIsMonoColor();
    }
}