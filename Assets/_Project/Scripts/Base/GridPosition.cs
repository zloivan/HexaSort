using System;
using UnityEngine;

namespace _Project.Scripts.Base
{
    public readonly struct GridPosition : IEquatable<GridPosition>
    {
        public int X { get; }
        public int Z { get; }

        public GridPosition(int x, int z)
        {
            X = x;
            Z = z;
        }

        public override bool Equals(object obj) =>
            obj is GridPosition pos && X == pos.X && Z == pos.Z;

        public override int GetHashCode() =>
            HashCode.Combine(X, Z);

        public static bool operator ==(GridPosition a, GridPosition b) =>
            a.X == b.X && a.Z == b.Z;

        public static bool operator !=(GridPosition a, GridPosition b) =>
            !(a == b);

        public bool Equals(GridPosition other) =>
            this == other;

        public static GridPosition operator +(GridPosition a, GridPosition b) =>
            new(a.X + b.X, a.Z + b.Z);

        public static GridPosition operator -(GridPosition a, GridPosition b) =>
            new(a.X - b.X, a.Z - b.Z);

        public static GridPosition operator *(GridPosition a, GridPosition b) =>
            new(a.X * b.X, a.Z * b.Z);

        public static GridPosition operator *(GridPosition a, float b) =>
            new(Mathf.RoundToInt(a.X * b), Mathf.RoundToInt(a.Z * b));

        public static GridPosition operator *(GridPosition a, int b) =>
            new(a.X * b, a.Z * b);

        public static GridPosition operator /(GridPosition a, GridPosition b) =>
            new(a.X / b.X, a.Z / b.Z);

        public override string ToString() =>
            $"({X}, {Z})";
    }
}