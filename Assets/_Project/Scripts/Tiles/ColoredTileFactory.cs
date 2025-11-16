using UnityEngine;

namespace _Project.Scripts.Board
{
    public enum TileColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple
    }

    public readonly struct ColoredTile
    {
        private readonly TileColor _color;

        public ColoredTile(TileColor color) =>
            _color = color;

        public TileColor GetColor() =>
            _color;
    }
    
    public static class ColoredTileFactory
    {
        public static ColoredTile CreateTile(TileColor color) =>
            new(color);

        public static ColoredTile CreateRandomTile() =>
            new(GetRandomColor());

        private static TileColor GetRandomColor() =>
            (TileColor)Random.Range(0, System.Enum.GetValues(typeof(TileColor)).Length);
    }
}