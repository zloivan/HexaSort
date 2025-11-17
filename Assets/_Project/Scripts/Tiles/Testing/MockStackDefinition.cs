using System;
using System.Collections.Generic;

namespace HexSort.Tiles.Testing.HexSort.Tiles
{
    /// <summary>
    /// Defines a complete stack configuration for testing.
    /// ColorBlocks are ordered from BOTTOM to TOP.
    /// Example: [Red(2), Blue(3)] means 2 red tiles at bottom, 3 blue tiles on top.
    /// </summary>
    [Serializable]
    public struct MockStackDefinition
    {
        public string Description;
        public List<MockColorBlock> ColorBlocks;

        public MockStackDefinition(string description, List<MockColorBlock> colorBlocks)
        {
            Description = description;
            ColorBlocks = colorBlocks;
        }
    }
}