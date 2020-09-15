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

    public struct Move
    {
        public Move(pos oldPosition, pos newPosition, FigID movedID, FigID removedID)
        {
            oldPos = oldPosition;
            newPos = newPosition;
            this.movedID = movedID;
            this.removedID = removedID;
        }

        public pos oldPos { get; }
        public pos newPos { get; }
        public FigID movedID { get; }
        public FigID removedID { get; }
    }



}
