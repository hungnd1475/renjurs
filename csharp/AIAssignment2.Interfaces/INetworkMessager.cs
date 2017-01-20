using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIAssignment2.Foundations
{
    public enum GameResult { Win, Lose, Draw };

    public interface INetworkMessager
    {
        bool SendName(string name);
        bool SendMove(Move move);
        void ReadInfo(out int size, out float timeOut);
        bool ReadMove(out Move move, out GameResult result);
        void Disconnect();
    }
}
