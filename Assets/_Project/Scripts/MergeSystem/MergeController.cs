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

            StartCoroutine(MergeSequence(placedPosition));
        }

        //TODO: Add unitasks and switch to them
        private IEnumerator MergeSequence(GridPosition placedPosition)
        {
            _isMerging = true;
            yield return CascadeLoop(new List<GridPosition> { placedPosition });
            _isMerging = false;
        }

        private IEnumerator CheckDestruction(List<GridPosition> gridPositions)
        {
            var anyDestroyed = false;
            var affectedByDestruction = new HashSet<GridPosition>();

            foreach (var position in gridPositions)
            {
                if (!_boardGrid.IsValidGridPosition(position))
                    continue;

                var neighbors = _boardGrid.GetValidNeighbors(position);

                if (MergeExecutor.CheckAndDestroyStack(position, _boardGrid))
                {
                    anyDestroyed = true;

                    foreach (var neighbor in neighbors)
                    {
                        if (_boardGrid.GetStackAt(neighbor) != null)
                        {
                            affectedByDestruction.Add(neighbor);
                        }
                    }
                }
            }

            if (anyDestroyed)
                //TODO: SIGNAL
            {
                foreach (var position in affectedByDestruction)
                {
                    if (!gridPositions.Contains(position))
                    {
                        gridPositions.Add(position);
                    }
                }

                yield return new WaitForSeconds(_cascadeDelyaSeconds);
            }
        }

        private IEnumerator CascadeLoop(List<GridPosition> dirtyPos)
        {
            //Safety
            const int MAX_INTERATIONS = 100;

            var currentDirty = new HashSet<GridPosition>(dirtyPos);
            var iterationsCount = 0;

            while (currentDirty.Count > 0)
            {
                iterationsCount++;
                if (iterationsCount > MAX_INTERATIONS)
                {
                    Debug.LogError($"CascadeLoop exceeds max iterations ({MAX_INTERATIONS})");
                    break;
                }

                Debug.Log($"[Cascade Iteration {iterationsCount}] Analyzing {currentDirty.Count} dirty position");

                var operation =
                    MergeAnalyzer.FindBestMergeFromMultiple(new List<GridPosition>(currentDirty), _boardGrid);

                if (!operation.HasValue)
                {
                    Debug.Log($"[Cascade Complete] No more valid merges found after ({iterationsCount}) iterations");
                    break;
                }

                Debug.Log($"[Merge]{operation.Value.DebugReason}");
                
                MergeExecutor.ExecuteMerge(operation.Value, _boardGrid);

                yield return new WaitForSeconds(_cascadeDelyaSeconds);

                var positionsToCheckForDestruction = new List<GridPosition> { operation.Value.From, operation.Value.To };
                yield return CheckDestruction(positionsToCheckForDestruction);

                currentDirty = MergeAnalyzer.GetAffectedPositions(operation.Value, _boardGrid);
                Debug.Log($"[Next Iteration] {currentDirty.Count} positions marked dirty for next pass");
            }
        }
    }
}