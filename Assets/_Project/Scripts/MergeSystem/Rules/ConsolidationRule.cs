using System.Collections.Generic;
using HexSort.Board;
using HexSort.Grid;

namespace HexSort.MergeSystem.Rules
{
    public class ConsolidationRule : IMergeRule
    {
        private const float SCORE_BASE = 500f;
        private const float SCORE_PER_CONTRIBUTING_NEIGHBOR = 100f;
        private const float SCORE_PER_TILE = 5f;

        public List<MergeOperation> Analyze(GridPosition sourcePos, BoardGrid grid)
        {
            var operations = new List<MergeOperation>();
            var sourceStack = grid.GetStackAt(sourcePos);

            if (sourceStack == null)
                return operations;

            var sourceTopColor = sourceStack.GetTopColor();
            var neighborPositions = grid.GetNeighbors(sourcePos);
            var contributingNeighbors = new List<(GridPosition pos, int blockCount)>();

            foreach (var neighborPos in neighborPositions)
            {
                if (!grid.IsValidGridPosition(neighborPos))
                    continue;

                var neighborStack = grid.GetStackAt(neighborPos);
                
                if (neighborStack == null)
                    continue;

                if (neighborStack.GetTopColor() != sourceTopColor)
                    continue;

                var blockCount = neighborStack.GetTopColorBlockCount();
                
                if (blockCount > 0) 
                    contributingNeighbors.Add((neighborPos, blockCount));
            }

            if (contributingNeighbors.Count >= 2)
            {
                var totalTiles = 0;
                foreach (var neighbor in contributingNeighbors)
                {
                    totalTiles += neighbor.blockCount;
                }

                foreach (var (neighborPos, blockCount) in contributingNeighbors)
                {
                    var score = SCORE_BASE
                                + contributingNeighbors.Count * SCORE_PER_CONTRIBUTING_NEIGHBOR
                                + totalTiles * SCORE_PER_TILE;

                    operations.Add(new MergeOperation(
                        neighborPos,
                        sourcePos,
                        blockCount,
                        score,
                        $"Consolidation: {blockCount} tiles from {neighborPos} to {sourcePos} " +
                        $"(part of {contributingNeighbors.Count}-neighbor consolidation)"));
                }
            }

            return operations;
        }
    }
}