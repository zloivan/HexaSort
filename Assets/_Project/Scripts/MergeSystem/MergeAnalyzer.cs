using System.Collections.Generic;
using HexSort.Board;
using HexSort.Grid;
using HexSort.MergeSystem.Rules;

namespace HexSort.MergeSystem
{
    public static class MergeAnalyzer
    {
        private static readonly List<IMergeRule> _rules = new()
        {
            new InboundRule()
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

            return allOperations[0];
        }
    }
}