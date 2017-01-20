using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace AIAssignment2.GameLogic.Renjus
{
    using Foundations;
    using Threats;
    using Foundations.CoordinateGetters;

    public class ThreatRenju : BaseRenju
    {
        int moveCount = 0;
        int shortestLength = 1;
        int oppCoeff = 1;
        readonly Timer timer;
        volatile bool hasTime = true;

        public ThreatRenju(int size, float timeOut)
            : base(size, timeOut)
        {
            this.timer = new Timer(timeOut * 1000 - 200);
            this.timer.AutoReset = false;
            this.timer.Elapsed += timer_Elapsed;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            hasTime = false;
        }

        #region Alpha-beta pruning search
        private int abSearch(GameBoard board, int depth, int a, int b, int player)
        {
            if (board.CheckWinFor(ourValue))
                return int.MaxValue;

            if (board.CheckWinFor(oppValue))
                return int.MinValue;

            if (board.ContainsDisallowMoves())
            {
                return ourValue == GameBoard.Black ? int.MinValue : int.MaxValue;
            }

            if (!hasTime || depth == 0)
            {
                return evaluate(board.GetLinesOf(ourValue)) - oppCoeff * evaluate(board.GetLinesOf(oppValue));
            }
            else
            {
                var moves = board.GetPosibleMoves(player, shortestLength, false);
                if (!moves.Any())
                {
                    return evaluate(board.GetLinesOf(ourValue)) - oppCoeff * evaluate(board.GetLinesOf(oppValue));
                }

                if (player == ourValue) // max
                {
                    foreach (var m in moves)
                    {
                        var cloneBoard = board.Clone();
                        cloneBoard[m] = ourValue;

                        if (ourValue == GameBoard.Black)
                        {
                            if (cloneBoard.ContainsDisallowMovesAt(m)) continue;
                        }

                        a = Math.Max(a, abSearch(cloneBoard, depth - 1, a, b, oppValue));
                        if (b <= a)
                        {
                            break;
                        }
                    }
                    return a;
                }
                else // min
                {
                    foreach (var m in moves)
                    {
                        var cloneBoard = board.Clone();
                        cloneBoard[m] = oppValue;

                        if (oppValue == GameBoard.Black)
                        {
                            if (cloneBoard.ContainsDisallowMovesAt(m)) continue;
                        }

                        b = Math.Min(b, abSearch(cloneBoard, depth - 1, a, b, ourValue));
                        if (b <= a)
                        {
                            break;
                        }
                    }
                    return b;
                }
            }
        }

        private bool findGoodMove(IEnumerable<Move> posibleMoves, int depth, ref Move result)
        {
            result = posibleMoves.FirstOrDefault();
            var maxScore = int.MinValue;

            if (posibleMoves.Any())
            {
                foreach (var m in posibleMoves)
                {
                    var cloneBoard = currentBoard.Clone();
                    cloneBoard[m] = ourValue;
                    var score = abSearch(cloneBoard, depth, int.MinValue, int.MaxValue, oppValue);
                    if (maxScore < score)
                    {
                        maxScore = score;
                        result = m;

                        if (!hasTime) 
                        {
                            Console.WriteLine("Time out!");
                            return true;
                        }
                    }
                }
                return true;
            }
            else return false;
        }

        #endregion

        #region Threat-Space Search
        private bool counterOpponentThreats(SortedLinkedList<Threat> opponentThreats, ref Move move)
        {
            if (!opponentThreats.Any()) return false;

            if (opponentThreats.First.Value.Type == ThreatType.Five)
            {
                move = opponentThreats.First.Value.GainSquare;
                return true;
            }

            LinkedList<Move> counterMoves = new LinkedList<Move>();
            foreach (var t in opponentThreats)
            {
                if (t.Type == ThreatType.StraightFour)
                {
                    counterMoves.AddLast(t.GainSquare);
                }
            }

            if (counterMoves.Any())
            {
                var bestValue = 0;
                Move? bestMove = null;
                foreach (var m in counterMoves)
                {
                    if (oppValue == GameBoard.Black)
                    {
                        var oppCloneBoard = currentBoard.Clone();
                        oppCloneBoard[m] = oppValue;
                        if (oppCloneBoard.ContainsDisallowMovesAt(m))
                            continue;
                    }

                    var ourCloneBoard = currentBoard.Clone();
                    ourCloneBoard[m] = ourValue;

                    if (ourValue == GameBoard.Black)
                    {
                        if (ourCloneBoard.ContainsDisallowMovesAt(m))
                            continue;
                    }

                    var value = 0;
                    foreach (var t in ourCloneBoard.GetThreatsOf(oppValue))
                    {
                        switch (t.Type)
                        {
                            case ThreatType.BrokenThree:
                                value += 1;
                                break;
                            case ThreatType.Three:
                                value += 2;
                                break;
                            case ThreatType.Four:
                                value += 6;
                                break;
                            case ThreatType.StraightFour:
                                value += 10;
                                break;
                            case ThreatType.Five:
                                value += 16;
                                break;
                        }
                    }

                    value -= evaluate(ourCloneBoard.GetLinesOf(ourValue));

                    if (bestMove == null || bestValue > value)
                    {
                        bestValue = value;
                        bestMove = m;
                    }
                }

                move = bestMove.Value;
                return true;
            }

            for (var nodei = opponentThreats.First; nodei != opponentThreats.Last; nodei = nodei.Next)
            {
                var ti = nodei.Value;
                for (var nodej = nodei.Next; nodej != null; nodej = nodej.Next)
                {
                    var tj = nodej.Value;
                    if (ti.GainSquare.Equals(tj.GainSquare))
                    {
                        var cloneBoard = currentBoard.Clone();
                        if (oppValue == GameBoard.Black)
                        {
                            cloneBoard[ti.GainSquare] = oppValue;
                            if (cloneBoard.ContainsDisallowMovesAt(ti.GainSquare))
                                continue;
                        }
                        else if (ourValue == GameBoard.Black)
                        {
                            cloneBoard[ti.GainSquare] = ourValue;
                            if (cloneBoard.ContainsDisallowMovesAt(ti.GainSquare))
                                continue;
                        }

                        move = ti.GainSquare;
                        return true;
                    }
                }
            }

            return false;
        }

        private int findThreatSequence(GameBoard board, Threat threat, int depth, int player, ref Move result)
        {
            var oppValue = GameBoard.GetOpponentOf(player);
            var oppThreats = board.GetThreatsOf(oppValue);

            if (oppThreats.Any())
            {
                var compare = (ourValue == GameBoard.Black) ? (Func<Threat, Threat, int>)Threat.Compare : Threat.CompareDefence;

                if (threat.Type.CompareTo(ThreatType.Four) < 0 && Threat.CompareDefence(threat, oppThreats.First.Value) < 0)
                {
                    return 0;
                }
                else if (threat.Type != ThreatType.Five && oppThreats.First.Value.Type == ThreatType.Five)
                {
                    return 0;
                }
            }

            if (threat.Type == ThreatType.Five)
            {
                result = threat.GainSquare;
                return depth + 1;
            }

            if (threat.Type == ThreatType.StraightFour)
            {
                result = threat.GainSquare;
                return depth + 2;
            }

            var cloneBoard = board.Clone();
            cloneBoard[threat.GainSquare] = player;

            if (ourValue == GameBoard.Black && cloneBoard.ContainsDisallowMovesAt(threat.GainSquare))
                return 0;

            foreach (var cost in threat.CostSquares)
            {
                cloneBoard[cost] = oppValue;
            }

            var ourThreats = cloneBoard.GetThreatsOf(player);
            var t_depth = 0;

            foreach (var t in ourThreats)
            {
                if (t.LastSquare.Equals(threat.GainSquare) ||
                    t.GainSquare.CloseTo(threat.GainSquare, 1) || t.GainSquare.CloseTo(threat.GainSquare, 2))
                {
                    t_depth = findThreatSequence(cloneBoard, t, depth + 1, player, ref result);
                    if (t_depth > 0)
                    {
                        break;
                    }
                }
            }

            if (t_depth > 0)
            {
                result = threat.GainSquare;
                return t_depth;
            }

            return 0;
        }

        private bool findThreatSequence(int player, ref Move result)
        {
            var threats = currentBoard.GetThreatsOf(player);
            var found = false;
            var minDepth = int.MaxValue;
            var result_temp = new Move();

            foreach (var t in threats)
            {
                var depth = findThreatSequence(currentBoard, t, 0, player, ref result_temp);

                if (depth > 0 && minDepth > depth)
                {
                    found = true;
                    minDepth = depth;
                    result = result_temp;
                }
            }

            if (found && player == oppValue && minDepth <= 2) found = false;
#if DEBUG
            if (found) Console.WriteLine("{0} threat sequence found!", player == ourValue ? "Our" : "Opp's");
#endif
            return found;
        }
        #endregion

        protected override Move doSearching(Move opponentMove)
        {
            if (oppValue == GameBoard.Black && currentBoard.ContainsDisallowMovesAt(opponentMove))
            {
                timer.Stop();
                return new Move(-1, -1);
            }

            moveCount++;
            var result = new Move();

            oppCoeff = ourValue == GameBoard.Black ? 1 : 2;

            // first find threat sequence
            if (!findThreatSequence(ourValue, ref result))
            {
                // if our color is white then we must check on opp's threat sequence
                if (ourValue == GameBoard.Black || !findThreatSequence(oppValue, ref result))
                {
                    var oppThreats = currentBoard.GetThreatsOf(oppValue);
                    // if threat sequence not found then counter opp's threats
                    if (!counterOpponentThreats(oppThreats, ref result))
                    {
                        shortestLength = 2;
                        var moves = currentBoard.GetPosibleMoves(ourValue, shortestLength, ourValue == GameBoard.White);

                        // perform searching on our lines
                        if (!findGoodMove(moves, 3, ref result))
                        {
                            moves = currentBoard.GetSingleMoves(ourValue);

                            // perform searching on our single lines
                            if (!findGoodMove(moves, 3, ref result))
                            {
                                moves = currentBoard.GetSingleMoves(oppValue);

                                // perform searching on opp's single lines
                                if (!findGoodMove(moves, 2, ref result))
                                {
                                    moves = currentBoard.GetRestPosibleMoves();

                                    // perform searching on the rest empty squares
                                    findGoodMove(moves, 0, ref result);
                                }
                            }
                        }
                    }
                }
            }

            timer.Stop();
            return result;
        }

        protected override void startTimer()
        {
            hasTime = true;
            timer.Start();
        }

        #region Helper Methods

        private static int evaluate(LineCollection lines)
        {
            var sorted = lines.GetAllSortedLines();
            return evaluate(sorted);
        }

        private static int evaluate(Dictionary<int, SortedLinkedList<Line>> sortedLines)
        {
            var result = 0;

            foreach (var lineList in sortedLines.Values)
            {
                foreach (var line in lineList)
                {
                    var score = Line.Evaluate(line);
                    if (score < int.MaxValue)
                        result += score;
                    else return score;
                }
            }

            return result;
        }
        #endregion
    }
}
