using System.Collections.Generic;
using HexSort.Board;
using HexSort.Grid;
using HexSort.MergeSystem.Rules;
using UnityEngine;

namespace HexSort.MergeSystem
{
    public static class MergeAnalyzer
    {
        private static readonly List<IMergeRule> _rules = new()
        {
            new InboundRule(),
            new OutboundRule(),
        };

        public static MergeOperation? FindBestMerge(GridPosition sourcePosition, BoardGrid grid)
        {
            var allOperations = new List<MergeOperation>();

            foreach (var rule in _rules)
            {
                var operations = rule.Analyze(sourcePosition, grid);
                allOperations.AddRange(operations);
            }

            if (allOperations.Count == 0)
                return null;

            allOperations.Sort((a, b) => b.Score.CompareTo(a.Score));

            return allOperations[0];
        }

        public static MergeOperation? FindBestMergeFromMultiple(List<GridPosition> positions, BoardGrid grid)
        {
            var allOperations = new List<MergeOperation>();

            foreach (var gridPosition in positions)
            {
                foreach (var rule in _rules)
                {
                    var operations = rule.Analyze(gridPosition, grid);
                    allOperations.AddRange(operations);
                }
            }

            if (allOperations.Count == 0)
                return null;

            
            allOperations.Sort((a, b) => b.Score.CompareTo(a.Score));
            for (int i = 0; i < allOperations.Count; i++)
            {
                var op = allOperations[i];
                Debug.Log($"  [{i}] Score: {op.Score}, From: {op.From}, To: {op.To}");
            }
            return allOperations[0];
        }

        public static HashSet<GridPosition> GetAffectedPositions(MergeOperation operation, BoardGrid grid)
        {
            var affected = new HashSet<GridPosition>();

            if (grid.GetStackAt(operation.To) != null)
            {
                affected.Add(operation.To);

                AddMatchingNeighbors(operation.To, grid, affected);
            }

            if (grid.GetStackAt(operation.From) != null)
            {
                affected.Add(operation.From);
                AddMatchingNeighbors(operation.From, grid, affected);
            }

            return affected;
        }

        private static void AddMatchingNeighbors(GridPosition position, BoardGrid grid, HashSet<GridPosition> affected)
        {
            var stack = grid.GetStackAt(position);
            if (stack == null)
            {
                return;
            }

            var topColor = stack.GetTopColor();
            var neighbors = grid.GetValidNeighbors(position);

            foreach (var neighbor in neighbors)
            {
                var neighborStack = grid.GetStackAt(neighbor);
                if (neighborStack == null) continue;

                if (neighborStack.GetTopColor() == topColor && !affected.Contains(neighbor))
                {
                    affected.Add(neighbor);

                    AddMatchingNeighbors(neighbor, grid, affected);
                }
            }
        }
    }
}