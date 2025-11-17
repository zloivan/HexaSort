using System;
using System.Collections;
using System.Collections.Generic;
using HexSort.Board;
using HexSort.Grid;
using UnityEngine;

namespace HexSort.MergeSystem
{
    public class MergeController : MonoBehaviour
    {
        public static MergeController Instance { get; private set; }
        [SerializeField] private float _cascadeDelyaSeconds = 0.3f;


        private BoardGrid _boardGrid;
        private bool _isMerging;

        public void Awake() =>
            Instance = this;

        private void Start() =>
            _boardGrid = BoardGrid.Instance;

        public void TriggerMerge(GridPosition placedPosition)
        {
            if (_isMerging)
                return;

            Debug.Log("Start merging");
            StartCoroutine(MergeSequence(placedPosition));
        }

        //TODO: Add unitasks and switch to them
        private IEnumerator MergeSequence(GridPosition placedPosition)
        {
            _isMerging = true;

            yield return InitialMerge(placedPosition);

            yield return CheckDestruction(new List<GridPosition> { placedPosition });

            yield return CascadeLoop(new List<GridPosition> { placedPosition });

            _isMerging = false;
        }

        private IEnumerator InitialMerge(GridPosition placedPosition)
        {
            var operation = MergeAnalyzer.FindBestMerge(placedPosition, _boardGrid);

            if (!operation.HasValue)
                yield break;

            //TODO: TEMP REMOVE LATER
            Debug.Log(operation.Value.DebugReason);
            MergeExecutor.ExecuteMerge(operation.Value, _boardGrid);
            //TODO: SIGNAL
            yield return new WaitForSeconds(_cascadeDelyaSeconds);
        }

        private IEnumerator CheckDestruction(List<GridPosition> gridPositions)
        {
            var anyDestroyed = false;

            foreach (var position in gridPositions)
            {
                if (!_boardGrid.IsValidGridPosition(position))
                    continue;

                if (MergeExecutor.CheckAndDestroyStack(position, _boardGrid)) anyDestroyed = true;
            }

            if (anyDestroyed)
                //TODO: SIGNAL
                yield return new WaitForSeconds(_cascadeDelyaSeconds);
        }

        private IEnumerator CascadeLoop(List<GridPosition> dirtyPos)
        {
            var currentDirty = new List<GridPosition>(dirtyPos);
            while (currentDirty.Count > 0)
            {
                var operation = MergeAnalyzer.FindBestMergeFromMultiple(currentDirty, _boardGrid);
                if (!operation.HasValue)
                    break;

                MergeExecutor.ExecuteMerge(operation.Value, _boardGrid);
                //TODO: SIGNAL

                yield return new WaitForSeconds(_cascadeDelyaSeconds);

                var affectedPositions = new List<GridPosition> { operation.Value.From, operation.Value.To };
                yield return CheckDestruction(affectedPositions);

                currentDirty.Clear();

                if (_boardGrid.GetStackAt(operation.Value.To) != null)
                {
                    currentDirty.Add(operation.Value.To);

                    var toStack = _boardGrid.GetStackAt(operation.Value.To);
                    var toTopColor = toStack.GetTopColor();
                    var neighbors = _boardGrid.GetNeighbors(operation.Value.To);


                    foreach (var neighbor in neighbors)
                    {
                        if (!_boardGrid.IsValidGridPosition(neighbor))
                            continue;

                        var neighborStack = _boardGrid.GetStackAt(neighbor);
                        if (neighborStack == null)
                        {
                            continue;
                        }

                        if (neighborStack.GetTopColor() == toTopColor && !currentDirty.Contains(neighbor))
                        {
                            currentDirty.Add(neighbor);
                        }
                    }
                }


                if (_boardGrid.GetStackAt(operation.Value.From) != null && !currentDirty.Contains(operation.Value.From))
                {
                    currentDirty.Add(operation.Value.From);
                }
            }
        }
    }
}