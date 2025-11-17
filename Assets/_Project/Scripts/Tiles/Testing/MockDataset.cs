using UnityEngine.Serialization;

namespace HexSort.Tiles.Testing
{
    using System.Collections.Generic;
    using UnityEngine;

    namespace HexSort.Tiles
    {
        /// <summary>
        /// ScriptableObject containing 100+ predefined stack configurations.
        /// Create via: Assets > Create > HexSort > Mock Data Set
        /// </summary>
        [CreateAssetMenu(fileName = "MockDataSet", menuName = "HexSort/Mock Data Set", order = 1)]
        public class MockDataSet : ScriptableObject
        {
            [FormerlySerializedAs("Stacks")]
            [Header("Stack Definitions")]
            [Tooltip("List of predefined stacks. HandProvider will cycle through these in order.")]
            public List<MockStackDefinition> _stacks;

            [ContextMenu("Generate 100 Test Stacks")]
            private void GenerateTestStacks()
            {
                _stacks ??= new List<MockStackDefinition>();
                
                _stacks.Clear();

                // Test Case Group 1: Simple mono-color stacks (10 stacks)
                for (int i = 0; i < 10; i++)
                {
                    var color = (TileColor)(i % 5);
                    _stacks.Add(new MockStackDefinition(
                        $"Mono {color} x3",
                        new List<MockColorBlock> { new MockColorBlock(color, 3) }
                    ));
                }

                // Test Case Group 2: Two-color stacks for basic merge testing (20 stacks)
                for (int i = 0; i < 20; i++)
                {
                    var color1 = (TileColor)(i % 5);
                    var color2 = (TileColor)((i + 1) % 5);
                    _stacks.Add(new MockStackDefinition(
                        $"Two-color: {color1}(2) + {color2}(2)",
                        new List<MockColorBlock>
                        {
                            new MockColorBlock(color1, 2),
                            new MockColorBlock(color2, 2)
                        }
                    ));
                }

                // Test Case Group 3: Three-color stacks for cascade testing (20 stacks)
                for (int i = 0; i < 20; i++)
                {
                    var color1 = (TileColor)(i % 5);
                    var color2 = (TileColor)((i + 1) % 5);
                    var color3 = (TileColor)((i + 2) % 5);
                    _stacks.Add(new MockStackDefinition(
                        $"Three-color: {color1}(2) + {color2}(2) + {color3}(1)",
                        new List<MockColorBlock>
                        {
                            new MockColorBlock(color1, 2),
                            new MockColorBlock(color2, 2),
                            new MockColorBlock(color3, 1)
                        }
                    ));
                }

                // Test Case Group 4: Large blocks for destruction testing (15 stacks)
                for (int i = 0; i < 15; i++)
                {
                    var color = (TileColor)(i % 5);
                    var count = 5 + (i % 3); // 5, 6, or 7 tiles
                    _stacks.Add(new MockStackDefinition(
                        $"Large block: {color} x{count}",
                        new List<MockColorBlock> { new MockColorBlock(color, count) }
                    ));
                }

                // Test Case Group 5: Consolidation test stacks (15 stacks)
                // These have matching colors that should trigger consolidation rule
                for (int i = 0; i < 15; i++)
                {
                    var color = (TileColor)(i % 5);
                    _stacks.Add(new MockStackDefinition(
                        $"Consolidation test: {color}(3)",
                        new List<MockColorBlock> { new MockColorBlock(color, 3) }
                    ));
                }

                // Test Case Group 6: Outbound rule test stacks (10 stacks)
                // Multi-color stacks where top block might split to neighbors
                for (int i = 0; i < 10; i++)
                {
                    var color1 = (TileColor)(i % 5);
                    var color2 = (TileColor)((i + 2) % 5);
                    _stacks.Add(new MockStackDefinition(
                        $"Outbound: {color1}(2) + {color2}(3)",
                        new List<MockColorBlock>
                        {
                            new MockColorBlock(color1, 2),
                            new MockColorBlock(color2, 3)
                        }
                    ));
                }

                // Test Case Group 7: Edge cases (10 stacks)
                // Single tiles, max height, etc.
                _stacks.Add(new MockStackDefinition("Single Red",
                    new List<MockColorBlock> { new MockColorBlock(TileColor.Red, 1) }));
                _stacks.Add(new MockStackDefinition("Single Blue",
                    new List<MockColorBlock> { new MockColorBlock(TileColor.Blue, 1) }));
                _stacks.Add(new MockStackDefinition("Single Green",
                    new List<MockColorBlock> { new MockColorBlock(TileColor.Green, 1) }));
                _stacks.Add(new MockStackDefinition("Four colors", new List<MockColorBlock>
                {
                    new MockColorBlock(TileColor.Red, 1),
                    new MockColorBlock(TileColor.Blue, 1),
                    new MockColorBlock(TileColor.Green, 1),
                    new MockColorBlock(TileColor.Yellow, 1)
                }));
                _stacks.Add(new MockStackDefinition("Almost destroyable: Red x9",
                    new List<MockColorBlock> { new MockColorBlock(TileColor.Red, 9) }));
                _stacks.Add(new MockStackDefinition("Exactly destroyable: Blue x10",
                    new List<MockColorBlock> { new MockColorBlock(TileColor.Blue, 10) }));
                _stacks.Add(new MockStackDefinition("Alternating small", new List<MockColorBlock>
                {
                    new MockColorBlock(TileColor.Red, 1),
                    new MockColorBlock(TileColor.Blue, 1),
                    new MockColorBlock(TileColor.Red, 1)
                }));
                _stacks.Add(new MockStackDefinition("Heavy bottom", new List<MockColorBlock>
                {
                    new MockColorBlock(TileColor.Purple, 5),
                    new MockColorBlock(TileColor.Yellow, 1)
                }));
                _stacks.Add(new MockStackDefinition("Heavy top", new List<MockColorBlock>
                {
                    new MockColorBlock(TileColor.Green, 1),
                    new MockColorBlock(TileColor.Red, 5)
                }));
                _stacks.Add(new MockStackDefinition("Symmetric", new List<MockColorBlock>
                {
                    new MockColorBlock(TileColor.Blue, 2),
                    new MockColorBlock(TileColor.Yellow, 2),
                    new MockColorBlock(TileColor.Blue, 2)
                }));

                Debug.Log($"Generated {_stacks.Count} mock stacks for testing!");
            }

            [ContextMenu("Clear All Stacks")]
            private void ClearStacks()
            {
                _stacks.Clear();
                Debug.Log("Cleared all mock stacks.");
            }

            /// <summary>
            /// Helper to add a custom stack definition from code.
            /// </summary>
            public void AddStack(string description, params (TileColor color, int count)[] blocks)
            {
                var colorBlocks = new List<MockColorBlock>();
                foreach (var (color, count) in blocks)
                {
                    colorBlocks.Add(new MockColorBlock(color, count));
                }

                _stacks.Add(new MockStackDefinition(description, colorBlocks));
            }
        }
    }
}