using System.Collections.Generic;
using HexSort.Board;
using HexSort.Grid;

namespace HexSort.MergeSystem.Rules
{
    public interface IMergeRule
    {
        List<MergeOperation> Analyze(GridPosition sourcePos, BoardGrid grid);
    }
}