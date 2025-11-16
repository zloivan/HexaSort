using System.Collections.Generic;
using HexSort.Board;
using HexSort.Grid;
using HexSort.Tiles;

namespace HexSort.MergeSystem.Rules
{
    public class InboundRule : IMergeRule
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

                var blockCount = neighborStack.GetTopColorBlockCount();
                if (blockCount <= 0)
                {
                    continue;
                }

                var score = CalculateScore(sourceStack, neighborStack, blockCount);

                operations.Add(new MergeOperation(
                    neighborPos,
                    sourcePos,
                    blockCount,
                    score,
                    $"Inbound: {blockCount} tiles from {neighborPos} to {sourcePos}"));
            }

            return operations;
        }

        private float CalculateScore(TileStack sourceStack, TileStack neighborStack, int blockCount)
        {
            var score = SCORE_BASE + blockCount * SCORE_PER_TILE;

            var sourceTiles = sourceStack.GetAllTiles();
            var neighborTopBlock = neighborStack.GetTopBlock();

            var resultCount = sourceTiles.Count + blockCount;
            var willBeMonoColor = true;

            var firstColor = sourceTiles[0].GetColor();

            foreach (var tile in sourceTiles)
            {
                if (tile.GetColor() != firstColor)
                {
                    willBeMonoColor = false;
                    break;
                }
            }

            if (willBeMonoColor)
            {
                foreach (var tile in neighborTopBlock)
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