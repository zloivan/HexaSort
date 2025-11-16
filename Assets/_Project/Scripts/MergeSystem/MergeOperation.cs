using HexSort.Grid;

namespace HexSort.MergeSystem
{
    public struct MergeOperation
    {
        public GridPosition From;
        public GridPosition To;
        public int TileCount;
        public float Score;
        public string DebugReason;

        public MergeOperation(GridPosition from, GridPosition to, int tileCount, float score, string debugReason = "")
        {
            From = from;
            To = to;
            TileCount = tileCount;
            Score = score;
            DebugReason = debugReason;
        }
    }
}