using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIAssignment2.Foundations;

namespace AIAssignment2.GameLogic.Renjus
{
    using Threats;

    public class GameBoard
    {
        private readonly int size;
        private readonly int[] table;
        private readonly LineCollection whiteLines;
        private readonly LineCollection blackLines;

        public const int Empty = 0;
        public const int Black = 1;
        public const int White = 2;

        public int Size { get { return size; } }
        
        public GameBoard(int size)
        {
            this.size = size;
            this.table = new int[size * size];
            this.whiteLines = new LineCollection(White);
            this.blackLines = new LineCollection(Black);
        }

        private GameBoard(int size, int[] table, LineCollection whiteLines, LineCollection blackLines)
        {
            this.size = size;
            this.table = table;
            this.whiteLines = whiteLines;
            this.blackLines = blackLines;
        }

        public int this[int x, int y]
        {
            get { return table[x * size + y]; }
            set
            {
                table[x * size + y] = value;
                blackLines.SetMove(new Move(x, y), this, value);
                whiteLines.SetMove(new Move(x, y), this, value);
            }
        }

        public int this[Move move]
        {
            get { return this[move.X, move.Y]; }
            set { this[move.X, move.Y] = value; }
        }

        public GameBoard Clone()
        {
            var newTable = new int[this.table.Length];
            Buffer.BlockCopy(this.table, 0, newTable, 0, sizeof(int) * this.table.Length);
            var newBoard = new GameBoard(size, newTable, whiteLines.Clone(), blackLines.Clone());
            return newBoard;
        }

        public bool InBoard(int x, int y)
        {
            return (x >= 0 && x < size && y >= 0 && y < size);
        }

        public bool InBoard(Move square)
        {
            return InBoard(square.X, square.Y);
        }

        public bool IsValid(int x, int y)
        {
            return (InBoard(x, y) && this[x, y] == Empty);
        }

        public bool IsValid(Move square)
        {
            return IsValid(square.X, square.Y);
        }

        public static int GetOpponentOf(int player)
        {
            switch (player)
            {
                case GameBoard.White:
                    return GameBoard.Black;
                case GameBoard.Black:
                    return GameBoard.White;
                default:
                    return GameBoard.Empty;
            }
        }

        public LineCollection GetLinesOf(int player)
        {
            if (player == GameBoard.Black) return blackLines;
            else if (player == GameBoard.White) return whiteLines;
            else return null;
        }

        public IEnumerable<Move> GetPosibleMoves(int firstPlayer, int shortestLength, bool getAllOppLines)
        {
            var firstLines = GetLinesOf(firstPlayer).GetAllSortedLines();
            var lastLines = GetLinesOf(GetOpponentOf(firstPlayer)).GetAllSortedLines();
            var allMoves = new HashSet<Move>();

            //var count = 0;

            for (int i = 5; i >= shortestLength; i--)
            {
                if (firstLines.ContainsKey(i))
                {
                    foreach (var l in firstLines[i])
                        foreach (var s in l.NextSquares.Keys)
                        {
                            allMoves.Add(s);
                        }
                }
            }

            for (int i = 5; i > 1; i--)
            {
                if (lastLines.ContainsKey(i))
                {
                    foreach (var l in lastLines[i])
                        foreach (var s in l.NextSquares.Keys)
                            allMoves.Add(s);
                    if (!getAllOppLines) break;
                }
            }

            return allMoves;
        }

        public IEnumerable<Move> GetSingleMoves(int player)
        {
            var lines = GetLinesOf(player).SingleLines;
            var allMoves = new HashSet<Move>();

            foreach (var l in lines)
            {
                foreach (var s in l.NextSquares)
                        allMoves.Add(s.Key);
            }

            return allMoves;
        }

        public IEnumerable<Move> GetRestPosibleMoves()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (this[i, j] == GameBoard.Empty)
                        yield return new Move(i, j);
                }
            }
        }

        public bool CheckWinFor(int player)
        {
            if (player == GameBoard.Black)
                return blackLines.ContainsFive();
            else if (player == GameBoard.White)
                return whiteLines.ContainsFive();
            return false;
        }

        public bool ContainsDisallowMoves()
        {
            return blackLines.ContainsOverLine(this) || blackLines.ContainsDisallowMoves();
        }

        public bool ContainsDisallowMovesAt(Move move)
        {
            return blackLines.ContainsOverLine(this) || blackLines.ContainsDisallowMovesAt(move);
        }

        public SortedLinkedList<Threat> GetThreatsOf(int player)
        {
            if (player == GameBoard.Black) return blackLines.GetAllSortedThreats(this);
            else if (player == GameBoard.White) return whiteLines.GetAllSortedThreats(this);
            else return null;
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool zeroBase)
        {
            var result = new StringBuilder();
            result.Append(" ");
            for (int j = 0; j < size; j++)
            {
                var tj = zeroBase ? j : j + 1;
                result.Append(" " + tj % 10);
            }
            result.AppendLine();
            for (int i = 0; i < size; i++)
            {
                var ti = zeroBase ? i : i + 1;
                result.Append(ti % 10);
                for (int j = 0; j < size; j++)
                {
                    var s = this[i, j];
                    var c = ' ';
                    if (s == GameBoard.Black)
                    {
                        c = 'X';
                    }
                    else if (s == GameBoard.White)
                    {
                        c = 'O';
                    }
                    result.Append("|" + c);
                }
                result.AppendLine("|");
            }
            return result.ToString();
        }
    }
}
