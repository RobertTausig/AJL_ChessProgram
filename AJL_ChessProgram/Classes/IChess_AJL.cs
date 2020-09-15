using System;
using System.Collections.Generic;
using System.Text;

namespace AJL_ChessProgram
{
    interface IChess_AJL
    {
        void TakeMove(bool IsWhitePlayer, string FormattedMove);
        string GiveMove(bool IsWhitePlayer);
    }
}
