using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIAssignment2.GameLogic.Renjus.Threats
{
    using Foundations;
    public enum ThreatType { BrokenThree, Three, Four, StraightFour, Five };

    public class Threat
    {
        public HashSet<Move> CostSquares { get; private set; }
        public Move GainSquare { get; set; }
        public Move LastSquare { get; set; }
        public Move LeftSquare { get; set; }
        public Move RightSquare { get; set; }
        public Move? BrokenSquare { get; set; }
        public LineDirection Direction { get; set; }
        public ThreatType Type { get; set; }

        public Threat()
        {
            CostSquares = new HashSet<Move>();
        }

        public static int Compare(Threat t1, Threat t2)
        {
            var v1 = Convert.ToInt32(t1.Type);
            var v2 = Convert.ToInt32(t2.Type);
            if (v1 > v2) return 1;
            else if (v1 < v2) return -1;
            else return 0;
        }

        public static int CompareDefence(Threat t1, Threat t2)
        {
            var v1 = Convert.ToInt32(t1.Type);
            var v2 = Convert.ToInt32(t2.Type);
            var fourValue = Convert.ToInt32(ThreatType.Four);

            if (v1 <= fourValue && v2 <= fourValue)
            {
                return 0;
            }

            return v1 - v2;
        }

        public static Comparer<Threat> Comparer 
        { 
            get { return Comparer<Threat>.Create(Threat.Compare); } 
        }

        public override string ToString()
        {
            return string.Format("Type: {0} - GainSquare: ({1}, {2}) - LeftSquare: ({3}, {4}) - RightSquare: ({5}, {6})",
                this.Type, GainSquare.X, GainSquare.Y, LeftSquare.X, LeftSquare.Y, RightSquare.X, RightSquare.Y);
        }
    }
}
