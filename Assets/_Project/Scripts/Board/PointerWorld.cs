using System;
using UnityEngine;

namespace _Project.Scripts.Board
{
    public class PointerWorld : MonoBehaviour
    {
        private static PointerWorld Instance { get; set; }

        [SerializeField] private LayerMask _availableLayers;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _debugView;
        

        private void Awake()
        {
            _camera ??= Camera.main;
            Instance = this;
        }

        public static Vector3 GetPointerPositionInWorld()
        {
            var ray = Instance._camera.ScreenPointToRay(GetScreenPosition());
            return !Physics.Raycast(ray, out var hit, Mathf.Infinity, Instance._availableLayers)
                ? Vector3.zero
                : hit.point;
        }

        private void Update()
        {
            if (_debugView == null)
            {
                return;
            }

            _debugView.transform.position = GetPointerPositionInWorld();
        }


        private static Vector3 GetScreenPosition()
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).position;
            }
            
            return Input.mousePosition;
        }
    }
}