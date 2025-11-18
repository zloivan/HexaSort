using System.Collections.Generic;
using HexSort.Board;
using HexSort.Grid;
using HexSort.Tiles;
using UnityEngine;

namespace HexSort.MergeSystem.Rules
{
    public class InboundRule : IMergeRule
    {
        private const float SCORE_BASE = 50f;
        private const float SCORE_PER_TILE = 2f;
        private const float SCORE_WILL_DESTROY = 150f;
        private const float SCORE_WILL_MAKE_MONO = 75f;
        private const float SCORE_PER_EXTRA_MACTHG_NEIGHBOR = 200f;

        public List<MergeOperation> Analyze(GridPosition sourcePos, BoardGrid grid)
        {
            var operations = new List<MergeOperation>();
            var sourceStack = grid.GetStackAt(sourcePos);

            if (sourceStack == null)
                return operations;

            var sourceTopColor = sourceStack.GetTopColor();
            var neighborPositions = grid.GetValidNeighbors(sourcePos);

            var matchingNeighborCount = 0;

            //Consolitdation rule
            foreach (var neighborPos in neighborPositions)
            {
                var neighborStack = grid.GetStackAt(neighborPos);
                if (neighborStack == null)
                    continue;

                if (neighborStack.GetTopColor() == sourceTopColor && neighborStack.GetTopBlock().Count > 0)
                {
                    matchingNeighborCount++;
                }
            }

            //All other rules
            foreach (var neighborPos in neighborPositions)
            {
                var neighborStack = grid.GetStackAt(neighborPos);
                if (neighborStack == null)
                    continue;

                if (neighborStack.GetTopColor() != sourceTopColor)
                    continue;

                var blockCount = neighborStack.GetTopColorBlockCount();
                if (blockCount <= 0)
                    continue;

                var score = CalculateScore(sourceStack, neighborStack, blockCount, matchingNeighborCount);


                var debugReason = matchingNeighborCount >= 2
                    ? $"Inbound (Consolidation {matchingNeighborCount} neighbors): {blockCount} tiles from {neighborPos} to {sourcePos} (score: {score:F0})"
                    : $"Inbound: {blockCount} tiles from {neighborPos} to {sourcePos} (score: {score:F0})";

                operations.Add(new MergeOperation(
                    neighborPos,
                    sourcePos,
                    blockCount,
                    score,
                    debugReason));
            }

            return operations;
        }

        private float CalculateScore(TileStack sourceStack, TileStack neighborStack, int blockCount,
            int matchingNeighborCount)
        {
            var score = SCORE_BASE + blockCount * SCORE_PER_TILE;
            Debug.Log($"[InboundRule] Base calculation: {SCORE_BASE} + ({blockCount} * {SCORE_PER_TILE}) = {score}");

            if (matchingNeighborCount >= 2)
            {
                var bonus = (matchingNeighborCount - 1) * SCORE_PER_EXTRA_MACTHG_NEIGHBOR;
                score += bonus;
                Debug.Log(
                    $"[InboundRule] Matching neighbors bonus: {matchingNeighborCount} neighbors > 2, adding ({matchingNeighborCount - 1} * {SCORE_PER_EXTRA_MACTHG_NEIGHBOR}) = +{bonus}, total = {score}");
            }


            var sourceTiles = sourceStack.GetAllTiles();
            var neighborTopBlock = neighborStack.GetTopBlock();

            var resultCount = sourceTiles.Count + blockCount;

            var willBeMonoColor = CheckWillBeMonoColor(sourceTiles)
                                  && neighborTopBlock[0].GetColor() == sourceTiles[0].GetColor();


            if (willBeMonoColor && resultCount >= 10)
            {
                score += SCORE_WILL_DESTROY;
                Debug.Log(
                    $"[InboundRule] WILL DESTROY! resultCount={resultCount} >= 10, adding {SCORE_WILL_DESTROY}, total = {score}");
            }
            else
            {
                if (willBeMonoColor)
                {
                    score += SCORE_WILL_MAKE_MONO;
                    Debug.Log(
                        $"[InboundRule] Will make mono-color (but not destroy, resultCount={resultCount} < 10), adding {SCORE_WILL_MAKE_MONO}, total = {score}");
                }
            }

            return score;
        }


        private bool CheckWillBeMonoColor(List<ColoredTile> sourceTiles)
        {
            if (sourceTiles.Count == 0)
                return false;

            var firstColor = sourceTiles[0].GetColor();

            foreach (var coloredTile in sourceTiles)
            {
                if (coloredTile.GetColor() != firstColor)
                {
                    return false;
                }
            }

            return true;
        }
    }
}