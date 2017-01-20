using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIAssignment2.GameLogic.Renjus.Threats
{
    using Foundations;

    public enum LineDirection { None, Vertical, Horizontal, UpDiagonal, DownDiagonal };
    public enum SquareRelation { None, Left, Right };

    public class Line
    {
        public Dictionary<Move, Move?> NextSquares { get; private set; }
        public Move LeftSquare { get; set; }
        public Move RightSquare { get; set; }
        public Move LastSquare { get; set; }
        public Move? BrokenSquare { get; set; }
        public Move? ExtraBrokenSquare { get; set; }

        public int Length { get; set; }
        public LineDirection Direction { get; set; }
        public bool Close { get; set; }

        public bool Broken { get { return BrokenSquare != null; } }

        public Line()
        {
            this.NextSquares = new Dictionary<Move, Move?>();
            this.Direction = LineDirection.None;
            this.Close = false;
        }

        public Line Clone()
        {
            var t = new Line();
            t.LeftSquare = this.LeftSquare;
            t.RightSquare = this.RightSquare;
            t.LastSquare = this.LastSquare;
            t.BrokenSquare = this.BrokenSquare;
            t.ExtraBrokenSquare = this.ExtraBrokenSquare;
            t.Length = this.Length;
            t.Direction = this.Direction;
            t.Close = this.Close;

            foreach (var cost in this.NextSquares)
            {
                t.NextSquares.Add(cost.Key, cost.Value);
            }

            return t;
        }

        public static int Compare(Line line1, Line line2)
        {
            if (line1.Length > line2.Length) return 1;
            if (line1.Length < line2.Length) return -1;
            if (line1.Close && !line2.Close) return -1;
            if (!line1.Close && line2.Close) return 1;
            if (line1.Broken && !line2.Broken) return -1;
            if (!line1.Broken && line2.Broken) return 1;
            return 0;
        }

        public static int Evaluate(Line t)
        {
            if (t.Length == 5)
            {
                return int.MaxValue;
            }
            else if (t.Length == 4 && !t.Broken && !t.Close)
            {
                return int.MaxValue;
            }
            else
            {
                if (t.Close) return closeWeight[t.Length - 1];
                else if (t.Broken) return brokenWeigth[t.Length - 1];
                else return openWeight[t.Length - 1];
            }
        }

        private static int[] closeWeight = { 1, 4, 32, 256 };
        private static int[] brokenWeigth = { 1, 8, 64, 512 };
        private static int[] openWeight = { 2, 16, 128, 1024 };

        public static IComparer<Line> Comparer
        { 
            get { return Comparer<Line>.Create(Line.Compare); } 
        }

        public override string ToString()
        {
            return string.Format("Length: {0} - Close: {7} - Broken: {8} - LastSquare: ({1}, {2}) - LeftSquare: ({3}, {4}) - RightSquare: ({5}, {6})",
                this.Length, LastSquare.X, LastSquare.Y, LeftSquare.X, LeftSquare.Y, RightSquare.X, RightSquare.Y,
                this.Close, this.Broken);
        }
    }
}
