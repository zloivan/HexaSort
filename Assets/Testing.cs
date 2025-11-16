using HexSort.Board;
using HexSort.Utilitis;
using UnityEngine;

namespace HexSort
{
    public class Testing : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var neighbors =
                    BoardGrid.Instance.GetNeighbors(
                        BoardGrid.Instance.GetGridPosition(PointerToWorld.GetPointerPositionInWorld()));
                Debug.Log($"Neighbors of click {string.Join("", neighbors)}");
            }
        }
    }
}