using System.Collections.Generic;
using HexSort.Board;
using HexSort.Grid;
using HexSort.Tiles;

namespace HexSort.MergeSystem.Rules
{
    public class OutboundRule : IMergeRule
    {
        private const float SCORE_BASE = 10f;
        private const float SCORE_PER_TILE = 1f;
        private const float SCORE_WILL_DESTROY = 100f;
        private const float SCORE_WILL_MAKE_MONO = 50f;

        public List<MergeOperation> Analyze(GridPosition sourcePos, BoardGrid grid)
        {
            var operations = new List<MergeOperation>();
            var sourceStack = grid.GetStackAt(sourcePos);

            if (sourceStack == null)
            {
                return operations;
            }

            var sourceTopColor = sourceStack.GetTopColor();
            var neighborPositions = grid.GetNeighbors(sourcePos);

            foreach (var neighborPos in neighborPositions)
            {
                if (!grid.IsValidGridPosition(neighborPos))
                {
                    continue;
                }

                var neighborStack = grid.GetStackAt(neighborPos);
                if (neighborStack == null)
                {
                    continue;
                }

                if (neighborStack.GetTopColor() != sourceTopColor)
                {
                    continue;
                }

                var blockCount = sourceStack.GetTopColorBlockCount();
                if (blockCount <= 0)
                {
                    continue;
                }

                var score = CalculateScore(neighborStack, sourceStack, blockCount);

                operations.Add(new MergeOperation(
                    sourcePos,
                    neighborPos,
                    blockCount,
                    score,
                    $"Outbound: {blockCount} tiles from {sourcePos} to {neighborPos}"));
            }

            return operations;
        }

        private float CalculateScore(TileStack targetStack, TileStack sourceStack, int blockCount)
        {
            var score = SCORE_BASE + blockCount * SCORE_PER_TILE;

            var targetTiles = targetStack.GetAllTiles();
            var sourceTopBlock = sourceStack.GetTopBlock();

            var resultCount = targetTiles.Count + blockCount;
            var willBeMonoColor = true;

            var firstColor = targetTiles[0].GetColor();

            foreach (var tile in targetTiles)
            {
                if (tile.GetColor() != firstColor)
                {
                    willBeMonoColor = false;
                    break;
                }
            }

            if (willBeMonoColor)
            {
                foreach (var tile in sourceTopBlock)
                {
                    if (tile.GetColor() != firstColor)
                    {
                        willBeMonoColor = false;
                        break;
                    }
                }
            }

            if (willBeMonoColor && resultCount >= 10)
            {
                score += SCORE_WILL_DESTROY;
            }
            else
            {
                if (willBeMonoColor)
                {
                    score += SCORE_WILL_MAKE_MONO;
                }
            }

            return score;
        }
    }
}