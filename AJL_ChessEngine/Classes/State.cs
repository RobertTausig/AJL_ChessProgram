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

    public class State
    {

        private FigID[,] chessState = new FigID[xwidth, ywidth];
        public Dictionary<pos, FigID> WhiteFigurePlacement { get; } = new Dictionary<pos, FigID>();
        public Dictionary<pos, FigID> BlackFigurePlacement { get; } = new Dictionary<pos, FigID>();
        private pos tempPos;

        public FigID this[int x, int y]
        {
            get => chessState[x, y];
            set => KeepTrackOnPlacement(x, y, value);
        }
        public Color ColorOfField(int x, int y)
        {
            return IDtoColor[chessState[x, y]];
        }
        //public bool TryMove(pos from, pos to, FigID ident)
        //{
        //    try
        //    {
        //        //Can be deleted later, just for verification:
        //        if (chessState[from.Item1, from.Item2] != ident) { return false; }
        //        KeepTrackOnPlacement(from.Item1, from.Item2, FigID.empty);
        //        KeepTrackOnPlacement(to.Item1, to.Item2, ident);
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        protected void KeepTrackOnPlacement(int x, int y, FigID ident)
        {
            chessState[x, y] = ident;
            tempPos = new pos((byte)x, (byte)y);
            if (IDtoColor[ident] == Color.white)
            {
                WhiteFigurePlacement.TryAdd(tempPos, ident);
                BlackFigurePlacement.Remove(tempPos);
            }
            else if (IDtoColor[ident] == Color.black)
            {
                BlackFigurePlacement.TryAdd(tempPos, ident);
                WhiteFigurePlacement.Remove(tempPos);
            }
            else
            {
                WhiteFigurePlacement.Remove(tempPos);
                BlackFigurePlacement.Remove(tempPos);
            }
        }






    }
}
