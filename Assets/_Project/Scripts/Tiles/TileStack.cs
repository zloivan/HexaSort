using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _Project.Scripts.Board
{
    public class TileStack : MonoBehaviour
    {
        private readonly List<ColoredTile> _tiles = new();
       
        private bool _isDirty;

        public void AddTile(ColoredTile chunk)
        {
            _tiles.Add(chunk);
            _isDirty = true;
        }

        public void AddTiles(IList<ColoredTile> tiles)
        {
            _tiles.AddRange(tiles);
            _isDirty = true;
        }

        public List<ColoredTile> RemoveTilesFromTop(int count)
        {
            if (count <= 0 || count > _tiles.Count)
            {
                return new List<ColoredTile>();
            }

            var startIndex = _tiles.Count - count;
            var toRemove = _tiles.GetRange(startIndex, count);

            _tiles.RemoveRange(startIndex, count);
            _isDirty = true;
            return toRemove;
        }

        public void MoveTo(TileStack target, int count)
        {
            var tiles = RemoveTilesFromTop(count);
            target.AddTiles(tiles);
        }

        public TileColor GetTopColor() =>
            _tiles.Count == 0 ? default : _tiles[^1].GetColor();

        public int GetTopColorBlockCount()
        {
            if (_tiles.Count == 0)
                return 0;

            var topColor = _tiles[^1].GetColor();
            var count = 0;

            for (var i = _tiles.Count; i >= 0; i--)
            {
                if (_tiles[i].GetColor() == topColor)
                    count++;
                else
                    break;
            }

            return count;
        }

        public bool GetIsMonoColor()
        {
            if (_tiles.Count == 0)
                return false;

            var firstColor = _tiles[0].GetColor();

            return _tiles.All(t => t.GetColor() == firstColor);
        }

        public void ClearDirtyFlag() =>
            _isDirty = false;

        public void Clear()
        {
            _tiles.Clear();
            _isDirty = true;
        }

        public List<ColoredTile> GetAllTiles() => new(_tiles);

        public bool GetIsDirty() =>
            _isDirty;

        public override string ToString()
        {
            var sb = new StringBuilder($"Stack with: {_tiles.Count} tiles");
            for (var i = _tiles.Count; i >= 0; i--)
            {
                sb.AppendLine(_tiles[i].GetColor().ToString());
            }

            return sb.ToString();
        }
    }
}