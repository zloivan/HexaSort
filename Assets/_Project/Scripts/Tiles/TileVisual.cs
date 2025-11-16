using UnityEngine;

namespace HexSort.Tiles
{
    public class TileVisual : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        private void Awake() =>
            _meshRenderer ??= GetComponent<MeshRenderer>();

        public void Setup(Material material)
        {
            _meshRenderer.material = new Material(material);
        }
    }
}