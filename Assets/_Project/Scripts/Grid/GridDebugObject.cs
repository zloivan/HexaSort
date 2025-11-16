using TMPro;
using UnityEngine;

namespace HexSort.Grid
{
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _debagLabel;
        private object _gridObject;

        public virtual void SetGridObject(object gridObject) =>
            _gridObject = gridObject;

        protected void Update()
        {
            if (_gridObject != null) 
                _debagLabel.text = _gridObject.ToString();
        }
    }
}