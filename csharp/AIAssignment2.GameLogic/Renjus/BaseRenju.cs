using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIAssignment2.GameLogic.Renjus
{
    using Foundations;

    public abstract class BaseRenju : IRenju
    {
        protected readonly int size;
        protected readonly double timeOut;
        protected GameBoard currentBoard;
        protected bool firstMove;
        protected int ourValue;
        protected int oppValue;

        public BaseRenju(int size, double timeOut)
        {
            this.size = size;
            this.timeOut = timeOut;
            this.currentBoard = new GameBoard(size);
            this.firstMove = true;
            this.ourValue = GameBoard.White;
            this.oppValue = GameBoard.Black;
        }

        protected abstract Move doSearching(Move opponentMove);
        protected abstract void startTimer();

        public Move GetNextMove(Move opponentMove)
        {
            var nextMove = new Move();
            if (opponentMove.X < 0 && opponentMove.Y < 0)
            {
                ourValue = GameBoard.Black;
                oppValue = GameBoard.White;
                nextMove = new Move(size / 2, size / 2);
                firstMove = false;
            }
            else
            {
                currentBoard[opponentMove] = oppValue;
                if (firstMove)
                {
                    var random = new Random();
                    var t = random.Next(10000);
                    switch (t % 4)
                    {
                        case 0:
                            nextMove = new Move(opponentMove.X - 1, opponentMove.Y - 1);
                            break;
                        case 1:
                            nextMove = new Move(opponentMove.X - 1, opponentMove.Y + 1);
                            break;
                        case 2:
                            nextMove = new Move(opponentMove.X + 1, opponentMove.Y - 1);
                            break;
                        case 3:
                            nextMove = new Move(opponentMove.X + 1, opponentMove.Y + 1);
                            break;
                    }
                    firstMove = false;
                }
                else
                {
                    startTimer();
                    nextMove = doSearching(opponentMove);
                }
            }

            if (currentBoard.InBoard(nextMove))
                currentBoard[nextMove] = ourValue;
            return nextMove;
        }

        public void PrintBoard(bool zeroBase)
        {
            Console.Write(currentBoard.ToString(zeroBase));
        }
    }
}
