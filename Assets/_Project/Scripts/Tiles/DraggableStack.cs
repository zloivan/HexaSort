using HexSort.Board;
using HexSort.Grid;
using HexSort.MergeSystem;
using HexSort.Utilitis;
using UnityEngine;

namespace HexSort.Tiles
{
    public class DraggableStack : MonoBehaviour //TODO: Move to some input manager and probably use PointerToWorld
    {
        private const float DRAG_HEIGHT = 2f;

        [SerializeField] private TileStack _tileStack;
        [SerializeField] private Camera _camera;


        private bool _isDragging;
        private Vector3 _originalPosition;
        private BoardGrid _boardGrid;
        private MergeController _mergeController;

        private void Awake()
        {
            _camera ??= Camera.main;
            _originalPosition = transform.position;
        }

        private void Start()
        {
            _boardGrid = BoardGrid.Instance;
            _mergeController = MergeController.Instance;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) ||
                (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                if (IsPointerOverStack())
                {
                    StartDrag();
                }
            }

            if (_isDragging)
            {
                UpdateDragPosition();

                if (Input.GetMouseButtonUp(0) ||
                    (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
                {
                    EndDrag();
                }
            }
        }

        private bool IsPointerOverStack() =>
            Physics.Raycast(_camera.ScreenPointToRay(GetScreenPosition()), out var hit)
            && hit.transform.IsChildOf(transform);

        private Vector3 GetScreenPosition() =>
            Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

        private void StartDrag()
        {
            _isDragging = true;
            _originalPosition = transform.position;
        }

        private void UpdateDragPosition()
        {
            var worldPos = PointerToWorld.GetPointerPositionInWorld();
            transform.position = worldPos + Vector3.up * DRAG_HEIGHT;
        }

        private void EndDrag()
        {
            _isDragging = false;

            var worldPos = PointerToWorld.GetPointerPositionInWorld();
            var gridPos = _boardGrid.GetGridPosition(worldPos);

            if (_boardGrid.IsValidGridPosition(gridPos) && _boardGrid.IsSlotEmpty(gridPos))
            {
                PlaceStack(gridPos);
            }
            else
            {
                transform.position = _originalPosition;
            }
        }

        private void PlaceStack(GridPosition gridPos)
        {
            var worldPos = _boardGrid.GetWorldPosition(gridPos);
            transform.position = worldPos;

            _boardGrid.PlaceStackAtPosition(gridPos, _tileStack);

            _mergeController.TriggerMerge(gridPos);
            enabled = false;
        }
    }
}