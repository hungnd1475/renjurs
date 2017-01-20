using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIAssignment2.GameLogic.Renjus.Threats
{
    using Foundations;
    using Foundations.CoordinateGetters;

    public class LineCollection
    {
        private readonly SortedLinkedList<Line> currentLines;
        private readonly Dictionary<Move, Line> singleLines;
        private readonly int player;

        public IEnumerable<Line> Lines { get { return currentLines; } }
        public IEnumerable<Line> SingleLines { get { return singleLines.Values; } }

        public LineCollection(int player)
        {
            this.player = player;
            currentLines = new SortedLinkedList<Line>(Line.Comparer, true);
            singleLines = new Dictionary<Move, Line>();
        }

        public void SetMove(Move move, GameBoard board, int player)
        {
            if (player == GameBoard.Empty) return;

            if (player == this.player)
            {
                var occupiedCost = new HashSet<Move>();
                increaseLinesLength(move, board, occupiedCost);
                combineLines(move, board);
                addSingleLines(move, board, occupiedCost);
            }
            else
            {
                setOpponentMove(move, board, player);
            }

            var removeThreats = new List<LinkedListNode<Line>>();
            var removeSingleThreats = new List<Move>();

            for (var node = currentLines.First; node != null; node = node.Next)
            {
                var t = node.Value;
                if (t.NextSquares.ContainsKey(move))
                {
                    t.NextSquares.Remove(move);
                }

                if (uselessLine(t, board))
                    removeThreats.Add(node);
            }

            foreach (var m in singleLines.Keys)
            {
                var t = singleLines[m];
                if (t.NextSquares.ContainsKey(move))
                {
                    t.NextSquares.Remove(move);
                }
                if (t.NextSquares.Count <= 0)
                    removeSingleThreats.Add(m);
            }

            foreach (var t in removeThreats)
                currentLines.Remove(t);

            foreach (var m in removeSingleThreats)
                singleLines.Remove(m);
        }

        private bool uselessLine(Line l, GameBoard board)
        {
            if (l.Length == 5)
                return false;

            if (l.NextSquares.Count <= 0)
                return true;

            var oppValue = GameBoard.GetOpponentOf(player);
            var coorGetter = getCoordinator(l.Direction);

            var left = l.LeftSquare;
            for (int offset = -1; offset >= -2; offset--)
            {
                left = coorGetter.GetMove(l.LeftSquare, offset);
                if (!board.InBoard(left) || board[left] == oppValue)
                    break;
            }

            var right = l.RightSquare;
            for (int offset = 1; offset <= 2; offset++)
            {
                right = coorGetter.GetMove(l.RightSquare, offset);
                if (!board.InBoard(right) || board[right] == oppValue)
                    break;
            }

            if (!board.InBoard(left) || board[left] == oppValue)
            {
                if (!board.InBoard(right) || board[right] == oppValue)
                {
                    var offset = 1;
                    var temp = left;
                    while (!temp.Equals(right))
                    {
                        temp = coorGetter.GetMove(left, offset++);
                    }
                    if (offset - 2 < 5) return true;
                }
            }

            return false;
        }

        private void increaseLinesLength(Move move, GameBoard board, HashSet<Move> occupiedCost)
        {
            var removeThreats = new List<LinkedListNode<Line>>();
            var addThreats = new List<Line>();

            foreach (var t in singleLines.Values)
            {
                if (t.NextSquares.ContainsKey(move))
                {
                    var newThreat = new Line();
                    newThreat.Length = t.Length + 1;
                    newThreat.LastSquare = move;
                    newThreat.BrokenSquare = t.NextSquares[move];
                    var info = getDirection(t.LastSquare, move);
                    newThreat.Direction = info.Item1;

                    switch (info.Item2)
                    {
                        case SquareRelation.Left:
                            newThreat.LeftSquare = move;
                            newThreat.RightSquare = t.LastSquare;
                            break;
                        case SquareRelation.Right:
                            newThreat.LeftSquare = t.LastSquare;
                            newThreat.RightSquare = move;
                            break;
                    }

                    findNextSquares(newThreat, board);
                    t.NextSquares.Remove(move);

                    if (newThreat.NextSquares.Count > 0)
                    {
                        addThreats.Add(newThreat);
                        foreach (var cost in newThreat.NextSquares.Keys)
                        {
                            occupiedCost.Add(cost);
                        }

                        #region Removes cost squares of length-one threat that currently in newThreat
                        var coorGetter = getCoordinator(newThreat.Direction);
                        var left = coorGetter.GetMove(t.LastSquare, -1);
                        if (t.NextSquares.ContainsKey(left))
                            t.NextSquares.Remove(left);

                        var right = coorGetter.GetMove(t.LastSquare, 1);
                        if (t.NextSquares.ContainsKey(right))
                            t.NextSquares.Remove(right);

                        var outer = coorGetter.GetMove(t.LastSquare, -2); ;
                        if (t.NextSquares.ContainsKey(outer))
                            t.NextSquares.Remove(outer);

                        outer = coorGetter.GetMove(t.LastSquare, 2);
                        if (t.NextSquares.ContainsKey(outer))
                            t.NextSquares.Remove(outer);
                        #endregion
                    }
                }
            }

            for (var node = currentLines.First; node != null; node = node.Next)
            {
                var t = node.Value;
                if (t.NextSquares.ContainsKey(move))
                {
                    var info = getDirection(t.LastSquare, move);
                    var brokenSquare = t.NextSquares[move];
                    if (t.Length < 5)
                    {
                        t.Length += 1;
                        t.LastSquare = move;

                        if (t.Length == 3)
                        {
                            if (t.BrokenSquare != null && brokenSquare != null && !t.BrokenSquare.Value.Equals(brokenSquare))
                                t.ExtraBrokenSquare = brokenSquare;
                            else
                            {
                                t.BrokenSquare = brokenSquare;
                            }
                        }
                        else
                        {
                            t.BrokenSquare = brokenSquare;
                            t.ExtraBrokenSquare = null;
                        }

                        var info_temp = getDirection(t.LeftSquare, move);
                        if (info_temp.Item2 == SquareRelation.Left)
                            t.LeftSquare = move;

                        info_temp = getDirection(t.RightSquare, move);
                        if (info_temp.Item2 == SquareRelation.Right)
                            t.RightSquare = move;

                        findNextSquares(t, board);
                        t.NextSquares.Remove(move);

                        if (t.NextSquares.Count <= 0 && t.Length < 5)
                        {
                            removeThreats.Add(node);
                        }
                        else
                        {
                            foreach (var cost in t.NextSquares.Keys)
                            {
                                occupiedCost.Add(cost);
                            }
                        }
                    }
                }
            }

            foreach (var t in removeThreats)
                currentLines.Remove(t);

            foreach (var t in addThreats)
                currentLines.Add(t);
        }

        private void combineLines(Move move, GameBoard board)
        {
            var removeThreats = new HashSet<LinkedListNode<Line>>();
            var addThreats = new List<Line>();

            for (var nodei = currentLines.First; nodei != currentLines.Last; nodei = nodei.Next)
            {
                var ti = nodei.Value;
                for (var nodej = nodei.Next; nodej != null; nodej = nodej.Next)
                {
                    var tj = nodej.Value;
                    if (ti.LastSquare.Equals(move) && tj.LastSquare.Equals(move) && ti.Direction == tj.Direction)
                    {
                        var newThreat = new Line();
                        newThreat.LastSquare = move;
                        newThreat.BrokenSquare = null;
                        newThreat.Direction = ti.Direction;
                        newThreat.Length = 0;

                        Move leftMost = ti.LeftSquare, rightMost = ti.RightSquare;

                        var info = getDirection(ti.LeftSquare, tj.LeftSquare);
                        if (info.Item2 == SquareRelation.Left)
                        {
                            leftMost = tj.LeftSquare;
                        }

                        info = getDirection(ti.RightSquare, tj.RightSquare);
                        if (info.Item2 == SquareRelation.Right)
                        {
                            rightMost = tj.RightSquare;
                        }

                        var coorGetter = getCoordinator(ti.Direction);
                        var square = leftMost;
                        for (int offset = 0; !square.Equals(rightMost); offset++, newThreat.Length++)
                        {
                            square = coorGetter.GetMove(leftMost, offset);
                            if (board[square] == GameBoard.Empty)
                            {
                                newThreat.Length--;
                            }
                        }

                        newThreat.LeftSquare = leftMost;
                        newThreat.RightSquare = rightMost;

                        if (ti.BrokenSquare == null || tj.BrokenSquare == null)
                        {
                            if (ti.BrokenSquare != null) newThreat.BrokenSquare = ti.BrokenSquare;
                            else if (tj.BrokenSquare != null) newThreat.BrokenSquare = tj.BrokenSquare;

                            if (newThreat.Length < 5 || (newThreat.Length == 5 && newThreat.BrokenSquare == null))
                            {
                                findNextSquares(newThreat, board);
                                addThreats.Add(newThreat);

                                removeThreats.Add(nodei);
                                removeThreats.Add(nodej);
                            }
                        }
                        else if (ti.Length == 2 && tj.Length == 2)
                        {
                            newThreat.BrokenSquare = ti.BrokenSquare;
                            newThreat.ExtraBrokenSquare = tj.BrokenSquare;
                            findNextSquares(newThreat, board);
                            addThreats.Add(newThreat);
                            removeThreats.Add(nodei);
                            removeThreats.Add(nodej);
                        }
                    }
                }
            }

            foreach (var t in removeThreats)
                currentLines.Remove(t);

            foreach (var t in addThreats)
                currentLines.Add(t);
        }

        private void addSingleLines(Move move, GameBoard board, HashSet<Move> occupiedCost)
        {
            if (board[move] == player)
            {
                var newThreat = new Line();
                newThreat.Length = 1;
                newThreat.LastSquare = move;
                newThreat.BrokenSquare = null;
                newThreat.LeftSquare = move;
                newThreat.RightSquare = move;
                for (int x = move.X - 1; x <= move.X + 1; x++)
                    for (int y = move.Y - 1; y <= move.Y + 1; y++)
                    {
                        var square = new Move(x, y);
                        if (!occupiedCost.Contains(square) && addNextSquare(newThreat, square, null, board))
                        {
                            #region Add broken cost squares
                            var info = getDirection(move, square);

                            switch (info.Item1)
                            {
                                case LineDirection.Vertical:
                                    switch (info.Item2)
                                    {
                                        case SquareRelation.Left:
                                            addNextSquare(newThreat, new Move(x - 1, y), square, board);
                                            break;
                                        case SquareRelation.Right:
                                            addNextSquare(newThreat, new Move(x + 1, y), square, board);
                                            break;
                                    }
                                    break;
                                case LineDirection.Horizontal:
                                    switch (info.Item2)
                                    {
                                        case SquareRelation.Left:
                                            addNextSquare(newThreat, new Move(x, y - 1), square, board);
                                            break;
                                        case SquareRelation.Right:
                                            addNextSquare(newThreat, new Move(x, y + 1), square, board);
                                            break;
                                    }
                                    break;
                                case LineDirection.DownDiagonal:
                                    switch (info.Item2)
                                    {
                                        case SquareRelation.Left:
                                            addNextSquare(newThreat, new Move(x - 1, y - 1), square, board);
                                            break;
                                        case SquareRelation.Right:
                                            addNextSquare(newThreat, new Move(x + 1, y + 1), square, board);
                                            break;
                                    }
                                    break;
                                case LineDirection.UpDiagonal:
                                    switch (info.Item2)
                                    {
                                        case SquareRelation.Left:
                                            addNextSquare(newThreat, new Move(x + 1, y - 1), square, board);
                                            break;
                                        case SquareRelation.Right:
                                            addNextSquare(newThreat, new Move(x - 1, y + 1), square, board);
                                            break;
                                    }
                                    break;
                            }
                            #endregion
                        }
                    }
                if (newThreat.NextSquares.Count > 0) 
                    singleLines.Add(move, newThreat);
            }
        }

        private void findNextSquares(Line threat, GameBoard board)
        {
            var coorGetter = getCoordinator(threat.Direction);
            if (coorGetter != null)
            {
                threat.NextSquares.Clear();

                if (threat.BrokenSquare == null)
                {
                    var right = coorGetter.GetMove(threat.RightSquare, 1);
                    if (addNextSquare(threat, right, null, board) && threat.Length <= 3)
                    {
                        var square = coorGetter.GetMove(threat.RightSquare, 2);
                        addNextSquare(threat, square, right, board);
                    }
                    else 
                    {
                        threat.Close = true;
                    }

                    var left = coorGetter.GetMove(threat.LeftSquare, -1);

                    if (addNextSquare(threat, left, null, board) && threat.Length <= 3)
                    {
                        var square = coorGetter.GetMove(threat.LeftSquare, -2);
                        addNextSquare(threat, square, left, board);
                    }
                    else
                    {
                        threat.Close = true;
                    }
                }
                else
                {
                    if (threat.Length == 2)
                    {
                        addNextSquare(threat, threat.BrokenSquare.Value, null, board);

                        var rightBroken = coorGetter.GetMove(threat.RightSquare, 1);
                        if (addNextSquare(threat, rightBroken, threat.BrokenSquare, board))
                        {
                            var square = coorGetter.GetMove(threat.RightSquare, 2);
                            addNextSquare(threat, square, rightBroken, board);
                        }
                        else threat.Close = true;

                        var lefBroken = coorGetter.GetMove(threat.LeftSquare, -1);
                        if (addNextSquare(threat, lefBroken, threat.BrokenSquare, board))
                        {
                            var square = coorGetter.GetMove(threat.LeftSquare, -2);
                            addNextSquare(threat, square, lefBroken, board);
                        }
                        else threat.Close = true;
                    }
                    else if (threat.Length == 3)
                    {
                        if (threat.ExtraBrokenSquare == null)
                        {
                            addNextSquare(threat, threat.BrokenSquare.Value, null, board);

                            var square = coorGetter.GetMove(threat.RightSquare, 1);
                            if (!addNextSquare(threat, square, threat.BrokenSquare, board))
                                threat.Close = true;

                            square = coorGetter.GetMove(threat.LeftSquare, -1);
                            if (!addNextSquare(threat, square, threat.BrokenSquare, board))
                                threat.Close = true;
                        }
                        else
                        {
                            addNextSquare(threat, threat.BrokenSquare.Value, threat.ExtraBrokenSquare, board);
                            addNextSquare(threat, threat.ExtraBrokenSquare.Value, threat.BrokenSquare, board);
                        }
                    }
                    else if (threat.Length == 4)
                    {
                        addNextSquare(threat, threat.BrokenSquare.Value, null, board);
                    }
                }
            }
        }

        private void setOpponentMove(Move move, GameBoard board, int player)
        {
            var removeThreats = new List<LinkedListNode<Line>>();
            var addThreats = new List<Line>();
            for (var node = currentLines.First; node != null; node = node.Next)
            {
                var t = node.Value;
                if (t.NextSquares.ContainsKey(move))
                {
                    var coorGetter = getCoordinator(t.Direction);
                    if (move.Equals(t.BrokenSquare) || move.Equals(t.ExtraBrokenSquare)) // devide threat
                    {
                        var square = coorGetter.GetMove(move, -1);
                        var newThreat = new Line(); //create left threat
                        newThreat.LastSquare = t.LeftSquare;
                        newThreat.LeftSquare = t.LeftSquare;
                        newThreat.RightSquare = square;
                        newThreat.Close = true;
                        newThreat.ExtraBrokenSquare = null;
                        newThreat.BrokenSquare = null;

                        for (int offset = -1; !square.Equals(t.LeftSquare); offset--, newThreat.Length++)
                        {
                            square = coorGetter.GetMove(move, offset);
                            if (board[square] == GameBoard.Empty)
                            {
                                newThreat.BrokenSquare = square;
                                newThreat.Length--;
                            }
                        }

                        if (newThreat.Length > 1)
                        {
                            newThreat.Direction = t.Direction;
                            findNextSquares(newThreat, board);
                            if (newThreat.NextSquares.Count > 0)
                                addThreats.Add(newThreat);
                        }
                        else
                        {
                            singleLines.Remove(newThreat.LastSquare);
                            addSingleLines(newThreat.LastSquare, board, new HashSet<Move>());
                        }

                        square = coorGetter.GetMove(move, 1);
                        newThreat = new Line();
                        newThreat.LastSquare = t.RightSquare;
                        newThreat.RightSquare = t.RightSquare;
                        newThreat.LeftSquare = square;
                        newThreat.Close = true;
                        newThreat.ExtraBrokenSquare = null;
                        newThreat.BrokenSquare = null;

                        for (int offset = 1; !square.Equals(t.RightSquare); offset++, newThreat.Length++)
                        {
                            square = coorGetter.GetMove(move, offset);
                            if (board[square] == GameBoard.Empty)
                            {
                                newThreat.BrokenSquare = square;
                                newThreat.Length--;
                            }
                        }

                        if (newThreat.Length > 1)
                        {
                            newThreat.Direction = t.Direction;
                            findNextSquares(newThreat, board);
                            if (newThreat.NextSquares.Count > 0)
                                addThreats.Add(newThreat);
                        }
                        else
                        {
                            singleLines.Remove(newThreat.LastSquare);
                            addSingleLines(newThreat.LastSquare, board, new HashSet<Move>());
                        }

                        removeThreats.Add(node);
                    }
                    else
                    {
                        var infoLeft = getDirection(t.LeftSquare, move);
                        var infoRight = getDirection(t.RightSquare, move);

                        if (infoLeft.Item2 == SquareRelation.Left)
                        {
                            var s = coorGetter.GetMove(move, -1);
                            if (t.NextSquares.ContainsKey(s))
                                t.NextSquares.Remove(s);
                            if (move.Equals(coorGetter.GetMove(t.LeftSquare, -1)))
                                t.Close = true;
                        }
                        else if (infoRight.Item2 == SquareRelation.Right)
                        {
                            var s = coorGetter.GetMove(move, 1);
                            if (t.NextSquares.ContainsKey(s))
                                t.NextSquares.Remove(s);
                            if (move.Equals(coorGetter.GetMove(t.RightSquare, 1)))
                                t.Close = true;
                        }

                        t.NextSquares.Remove(move);

                        if (uselessLine(t, board))
                            removeThreats.Add(node);
                    }
                }
            }

            foreach (var t in removeThreats)
                currentLines.Remove(t);

            foreach (var t in addThreats)
                currentLines.Add(t);

            var removeSingleThreats = new List<Move>();
            foreach (var m in singleLines.Keys)
            {
                var t = singleLines[m];
                var info = getDirection(t.LastSquare, move);
                var coorGetter = getCoordinator(info.Item1);

                if (t.NextSquares.ContainsKey(move))
                {
                    var left = coorGetter.GetMove(move, -1);
                    if (t.NextSquares.ContainsKey(left))
                        t.NextSquares.Remove(left);

                    var right = coorGetter.GetMove(move, 1);
                    if (t.NextSquares.ContainsKey(right))
                        t.NextSquares.Remove(right);

                    t.NextSquares.Remove(move);

                    if (t.NextSquares.Count <= 0)
                        removeSingleThreats.Add(m);
                    else
                    {
                        if (left.Equals(t.RightSquare)) t.Close = true;
                        else if (right.Equals(t.LeftSquare)) t.Close = true;
                    }
                }
            }

            foreach (var m in removeSingleThreats)
                singleLines.Remove(m);
        }

        public Dictionary<int, SortedLinkedList<Line>> GetAllSortedLines()
        {
            var result = new Dictionary<int, SortedLinkedList<Line>>(5);
            foreach (var t in currentLines)
            {
                if (!result.ContainsKey(t.Length))
                {
                    result.Add(t.Length, new SortedLinkedList<Line>(Line.Comparer, true));
                }
                result[t.Length].Add(t);
            }

            foreach (var t in singleLines.Values)
            {
                if (!result.ContainsKey(t.Length))
                {
                    result.Add(t.Length, new SortedLinkedList<Line>(Line.Comparer, true));
                }
                result[t.Length].Add(t);
            }

            return result;
        }

        public bool ContainsFive()
        {
            return (currentLines.Any() && currentLines.First.Value.Length == 5);
        }

        public bool ContainsDisallowMoves()
        {
            for (var nodei = currentLines.First; nodei != currentLines.Last; nodei = nodei.Next)
            {
                var li = nodei.Value;
                for (var nodej = nodei.Next; nodej != null; nodej = nodej.Next)
                {
                    var lj = nodej.Value;
                    if (li.LastSquare.Equals(lj.LastSquare))
                    {
                        if (li.Length == 4 && lj.Length == 4)
                            return true;
                        if (li.Length == 3 && lj.Length == 3 && !li.Close && !lj.Close && li.ExtraBrokenSquare == null && lj.ExtraBrokenSquare == null)
                            return true;
                    }
                }
            }

            return false;
        }

        public bool ContainsDisallowMovesAt(Move move)
        {
            for (var nodei = currentLines.First; nodei != currentLines.Last; nodei = nodei.Next)
            {
                var li = nodei.Value;
                if (li.LastSquare.Equals(move))
                {
                    for (var nodej = nodei.Next; nodej != null; nodej = nodej.Next)
                    {
                        var lj = nodej.Value;
                        if (lj.LastSquare.Equals(move))
                        {
                            if (li.Length == 4 && lj.Length == 4)
                                return true;
                            if (li.Length == 3 && lj.Length == 3 && !li.Close && !lj.Close && li.ExtraBrokenSquare == null && lj.ExtraBrokenSquare == null)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool ContainsOverLine(GameBoard board)
        {
            for (var node = currentLines.First; node != null; node = node.Next)
            {
                var l = node.Value;
                if (l.Length < 5) break;

                var coorGetter = getCoordinator(l.Direction);
                var left = coorGetter.GetMove(l.LeftSquare, -1);
                if (board.InBoard(left) && board[left] == player)
                    return true;

                var right = coorGetter.GetMove(l.RightSquare, 1);
                if (board.InBoard(right) && board[right] == player)
                    return true;
            }

            return false;
        }

        public LineCollection Clone()
        {
            var newObj = new LineCollection(player);
            foreach (var t in singleLines)
            {
                newObj.singleLines.Add(t.Key, t.Value.Clone());
            }
            foreach (var t in currentLines)
            {
                newObj.currentLines.Add(t.Clone());
            }
            return newObj;
        }

        #region Threat
        public SortedLinkedList<Threat> GetAllSortedThreats(GameBoard board)
        {
            var result = new SortedLinkedList<Threat>(Threat.Comparer, true);
            foreach (var line in currentLines)
            {
                if (line.Length == 2)
                {
                    createThreatThreeFrom(line, board, result);
                }
                else if (line.Length == 3)
                {
                    createThreatFourFrom(line, board, result);
                }
                else if (line.Length == 4)
                {
                    createThreatFiveFrom(line, board, result);
                }
            }

            var set = new HashSet<Move>();
            foreach (var m1 in singleLines.Keys)
            {
                set.Add(m1);
                foreach (var m2 in singleLines.Keys)
                {
                    if (!set.Contains(m2))
                    {
                        createThreatThreeFrom(m1, m2, board, result);
                    }
                }
            }

            return result;
        }

        private bool addCostSquare(Threat t, Move square, GameBoard board)
        {
            if (board.InBoard(square) && board[square] == GameBoard.Empty)
            {
                t.CostSquares.Add(square);
                return true;
            }
            return false;
        }

        private bool findCostSquare(Threat t, GameBoard board)
        {
            switch (t.Type)
            {
                case ThreatType.Three:
                    return fcqTypeThree(t, board);
                case ThreatType.BrokenThree:
                    return fcqTypeBrokenThree(t, board);
                case ThreatType.Four:
                    return fcpTypeFour(t, board);
                case ThreatType.StraightFour:
                    return fcpTypeStraightFour(t, board);
                case ThreatType.Five:
                    return true;
                default:
                    return false;
            }
        }

        private bool fcqTypeThree(Threat t, GameBoard board)
        {
            var coorGetter = getCoordinator(t.Direction);
            if (addCostSquare(t, coorGetter.GetMove(t.LeftSquare, -1), board) &&
                addCostSquare(t, coorGetter.GetMove(t.RightSquare, 1), board))
            {
                var left = coorGetter.GetMove(t.LeftSquare, -2);
                var right = coorGetter.GetMove(t.RightSquare, 2);
                if (board.InBoard(left) && board[left] == GameBoard.GetOpponentOf(player))
                {
                    addCostSquare(t, right, board);
                }
                else if (board.InBoard(right) && board[right] == GameBoard.GetOpponentOf(player))
                {
                    addCostSquare(t, left, board);
                }
                return true;
            }

            return false;
        }

        private bool fcqTypeBrokenThree(Threat t, GameBoard board)
        {
            var coorGetter = getCoordinator(t.Direction);
            var good = true;

            if (t.BrokenSquare != null)
            {
                good = addCostSquare(t, t.BrokenSquare.Value, board);
            }

            if (good)
            {
                return addCostSquare(t, coorGetter.GetMove(t.LeftSquare, -1), board) &&
                    addCostSquare(t, coorGetter.GetMove(t.RightSquare, 1), board);
            }

            return false;
        }

        private bool fcpTypeFour(Threat t, GameBoard board)
        {
            return addCostSquare(t, t.BrokenSquare.Value, board);
        }

        private bool fcpTypeStraightFour(Threat t, GameBoard board)
        {
            var coorGetter = getCoordinator(t.Direction);
            var addLeft = addCostSquare(t, coorGetter.GetMove(t.LeftSquare, -1), board);
            var addRight = addCostSquare(t, coorGetter.GetMove(t.RightSquare, 1), board);

            if (addLeft || addRight)
            {
                if (!addRight || !addLeft)
                    t.Type = ThreatType.Four;
                return true;
            }
            return false;
        }

        private void createThreatThreeFrom(Line line, GameBoard board, SortedLinkedList<Threat> threatList)
        {
            Threat newThreat;
            foreach (var s in line.NextSquares)
            {
                newThreat = new Threat();
                newThreat.GainSquare = s.Key;
                newThreat.Direction = line.Direction;
                newThreat.LastSquare = line.LastSquare;

                var infoLeft = getDirection(line.LeftSquare, s.Key);
                var infoRight = getDirection(line.RightSquare, s.Key);

                if (infoLeft.Item2 == SquareRelation.Left)
                {
                    newThreat.LeftSquare = newThreat.GainSquare;
                    newThreat.RightSquare = line.RightSquare;
                }
                else if (infoRight.Item2 == SquareRelation.Right)
                {
                    newThreat.LeftSquare = line.LeftSquare;
                    newThreat.RightSquare = s.Key;
                }
                else
                {
                    newThreat.LeftSquare = line.LeftSquare;
                    newThreat.RightSquare = line.RightSquare;
                }

                if (s.Value == null)
                {
                    newThreat.Type = ThreatType.Three;
                    if (findCostSquare(newThreat, board))
                        threatList.Add(newThreat);
                }
                else if (!line.Broken || s.Value.Equals(line.BrokenSquare))
                {
                    newThreat.Type = ThreatType.BrokenThree;
                    newThreat.BrokenSquare = s.Value;
                    if (findCostSquare(newThreat, board))
                        threatList.Add(newThreat);
                }
            }
        }

        private void createThreatThreeFrom(Move m1, Move m2, GameBoard board, SortedLinkedList<Threat> threatList)
        {
            if (m1.CloseTo(m2, 3))
            {
                var info = getDirection(m1, m2);
                var coorGetter = getCoordinator(info.Item1);

                var left = new Move();
                var right = new Move();

                if (info.Item2 == SquareRelation.Left)
                {
                    left = m2;
                    right = m1;
                }
                else if (info.Item2 == SquareRelation.Right)
                {
                    left = m1;
                    right = m2;
                }

                var innerLeft = coorGetter.GetMove(left, 1);
                var innerRight = coorGetter.GetMove(right, -1);

                if (board.IsValid(innerLeft) && board.IsValid(innerRight))
                {
                    var t = new Threat();
                    t.GainSquare = innerLeft;
                    t.Direction = info.Item1;
                    t.LastSquare = m2;
                    t.LeftSquare = left;
                    t.RightSquare = right;
                    t.Type = ThreatType.BrokenThree;
                    t.BrokenSquare = innerRight;

                    if (findCostSquare(t, board))
                        threatList.Add(t);

                    t = new Threat();
                    t.GainSquare = innerRight;
                    t.Direction = info.Item1;
                    t.LastSquare = m2;
                    t.LeftSquare = left;
                    t.RightSquare = right;
                    t.Type = ThreatType.BrokenThree;
                    t.BrokenSquare = innerLeft;

                    if (findCostSquare(t, board))
                        threatList.Add(t);
                }
            }
        }

        private void createThreatFourFrom(Line line, GameBoard board, SortedLinkedList<Threat> threatList)
        {
            Threat newThreat;
            foreach (var cost in line.NextSquares)
            {
                newThreat = new Threat();
                newThreat.Direction = line.Direction;
                newThreat.GainSquare = cost.Key;
                newThreat.BrokenSquare = cost.Value;
                newThreat.LastSquare = line.LastSquare;

                var infoLeft = getDirection(line.LeftSquare, cost.Key);
                var infoRight = getDirection(line.RightSquare, cost.Key);

                if (infoLeft.Item2 == SquareRelation.Left)
                {
                    newThreat.LeftSquare = cost.Key;
                    newThreat.RightSquare = line.RightSquare;
                }
                else if (infoRight.Item2 == SquareRelation.Right)
                {
                    newThreat.RightSquare = cost.Key;
                    newThreat.LeftSquare = line.LeftSquare;
                }
                else
                {
                    newThreat.LeftSquare = line.LeftSquare;
                    newThreat.RightSquare = line.RightSquare;
                }

                if (cost.Value != null)
                {
                    newThreat.Type = ThreatType.Four;
                    newThreat.BrokenSquare = cost.Value;
                    if (findCostSquare(newThreat, board))
                        threatList.Add(newThreat);
                }
                else
                {
                    newThreat.Type = ThreatType.StraightFour;
                    if (findCostSquare(newThreat, board))
                        threatList.Add(newThreat);
                }
            }
        }

        private void createThreatFiveFrom(Line line, GameBoard board, SortedLinkedList<Threat> threatList)
        {
            Threat newThreat;
            foreach (var cost in line.NextSquares)
            {
                newThreat = new Threat();
                newThreat.Direction = line.Direction;
                newThreat.GainSquare = cost.Key;
                newThreat.Type = ThreatType.Five;
                newThreat.LastSquare = line.LastSquare;
                threatList.Add(newThreat);
            }
        }
        #endregion

        #region Helper Methods
        private Tuple<LineDirection, SquareRelation> getDirection(Move lastGainSquare, Move currentGainSquare)
        {
            var deltaX = currentGainSquare.X - lastGainSquare.X;
            var deltaY = currentGainSquare.Y - lastGainSquare.Y;
            LineDirection direction = LineDirection.None;
            SquareRelation relative = SquareRelation.None;

            if (deltaX == 0)
            {
                direction = LineDirection.Horizontal;
                relative = (deltaY > 0) ? SquareRelation.Right : SquareRelation.Left;
            }
            else if (deltaY == 0)
            {
                direction = LineDirection.Vertical;
                relative = (deltaX > 0) ? SquareRelation.Right : SquareRelation.Left;
            }
            else if (deltaX == deltaY)
            {
                direction = LineDirection.DownDiagonal;
                relative = (deltaX > 0) ? SquareRelation.Right : SquareRelation.Left;
            }
            else if (deltaX * deltaY < 0)
            {
                direction = LineDirection.UpDiagonal;
                relative = (deltaY > 0) ? SquareRelation.Right : SquareRelation.Left;
            }

            return Tuple.Create(direction, relative);
        }

        private ICoordinateGetter getCoordinator(LineDirection direction)
        {
            switch (direction)
            {
                case LineDirection.Vertical:
                    return new VerticalGetter();
                case LineDirection.Horizontal:
                    return new HorizontalGetter();
                case LineDirection.DownDiagonal:
                    return new DownDiagonalGetter();
                case LineDirection.UpDiagonal:
                    return new UpDiagonalGetter();
                default:
                    return null;
            }
        }

        private bool addNextSquare(Line line, Move square, Move? brokenSquare, GameBoard board)
        {
            if (board.InBoard(square.X, square.Y) && board[square] == GameBoard.Empty && !line.NextSquares.ContainsKey(square))
            {
                line.NextSquares.Add(square, brokenSquare);
                return true;
            }
            return false;
        }
        #endregion
    }
}
