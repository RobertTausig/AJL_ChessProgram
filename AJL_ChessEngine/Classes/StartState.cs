using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static AJL_ChessEngine.Constants;
using System.Net.Sockets;

namespace AJL_ChessEngine
{
    using pos = Tuple<byte, byte>;
    using Lpos = List<Tuple<byte, byte>>;

    public class StdChessStartState : State
    {
        public StdChessStartState()
        {
            //Place Pawns:
            for (int i = 0; i < xwidth; i++)
            {
                this[i, 1] = FigID.wPawn;
                this[i, 6] = FigID.bPawn;
            }
            //Place other pieces:
            this[0, 0] = FigID.wRook;
            this[1, 0] = FigID.wKnight;
            this[2, 0] = FigID.wBishop;
            this[3, 0] = FigID.wQueen;
            this[4, 0] = FigID.wKing;
            this[5, 0] = FigID.wBishop;
            this[6, 0] = FigID.wKnight;
            this[7, 0] = FigID.wRook;
            this[0, 7] = FigID.bRook;
            this[1, 7] = FigID.bKnight;
            this[2, 7] = FigID.bBishop;
            this[3, 7] = FigID.bQueen;
            this[4, 7] = FigID.bKing;
            this[5, 7] = FigID.bBishop;
            this[6, 7] = FigID.bKnight;
            this[7, 7] = FigID.bRook;




        }







    }
}
