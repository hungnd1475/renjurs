using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIAssignment2.Foundations
{
    using CoordinateGetters;

    public struct Move
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Move(int x, int y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }

        public bool CloseTo(Move otherMove, int distance)
        {
            return (closeTo(otherMove, distance, new VerticalGetter()) || closeTo(otherMove, distance, new HorizontalGetter()) ||
                closeTo(otherMove, distance, new UpDiagonalGetter()) || closeTo(otherMove, distance, new DownDiagonalGetter()));
        }

        private bool closeTo(Move otherMove, int distance, ICoordinateGetter coorGetter)
        {
            return (coorGetter.GetMove(this, distance).Equals(otherMove) || coorGetter.GetMove(this, -distance).Equals(otherMove));
        }
    }

    public interface IRenju
    {
        Move GetNextMove(Move opponentMove);
        void PrintBoard(bool zeroBase);
    }
}
