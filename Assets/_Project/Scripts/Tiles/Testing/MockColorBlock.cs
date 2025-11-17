using System;
using UnityEngine;

namespace HexSort.Tiles.Testing.HexSort.Tiles
{
    /// <summary>
    /// Defines a single color block within a stack (e.g., 3 red tiles).
    /// Blocks are ordered from BOTTOM to TOP of the stack.
    /// </summary>
    [Serializable]
    public struct MockColorBlock
    {
        public TileColor Color;
        [Range(1, 10)]
        public int Count;

        public MockColorBlock(TileColor color, int count)
        {
            Color = color;
            Count = count;
        }
    }
}