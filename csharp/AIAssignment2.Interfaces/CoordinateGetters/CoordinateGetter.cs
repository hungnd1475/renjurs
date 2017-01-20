using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIAssignment2.Foundations.CoordinateGetters
{
    public class VerticalGetter : ICoordinateGetter
    {
        public Move GetMove(Move startMove, int offset)
        {
            return new Move(startMove.X + offset, startMove.Y);
        }
    }

    public class HorizontalGetter : ICoordinateGetter
    {
        public Move GetMove(Move startMove, int offset)
        {
            return new Move(startMove.X, startMove.Y + offset);
        }
    }

    public class DownDiagonalGetter : ICoordinateGetter
    {
        public Move GetMove(Move startMove, int offset)
        {
            return new Move(startMove.X + offset, startMove.Y + offset);
        }
    }

    public class UpDiagonalGetter : ICoordinateGetter
    {
        public Move GetMove(Move startMove, int offset)
        {
            return new Move(startMove.X - offset, startMove.Y + offset);
        }
    }
}
