using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIAssignment2.Foundations.CoordinateGetters
{
    using Foundations;
    public interface ICoordinateGetter
    {
        Move GetMove(Move startMove, int offset);
    }
}
