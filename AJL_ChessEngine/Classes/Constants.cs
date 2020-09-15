using System;
using System.Collections.Generic;
using System.Text;

namespace AJL_ChessEngine
{
    using pos = Tuple<byte, byte>;
    using Lpos = List<Tuple<byte, byte>>;

    //--------------------------------------------
    //SHALL NOT BE CHANGED!
    public enum FigID : byte
    {
        empty = 0,  //Default FigID
        wKing,
        wQueen,
        wRook,
        wBishop,
        wKnight,
        wPawn,
        wPawnEnPassantable,
        bKing = 16,
        bQueen,
        bRook,
        bBishop,
        bKnight,
        bPawn,
        bPawnEnPassantable,
    }
    //CHANGES ARE ALLOWED AGAIN
    //--------------------------------------------

    public enum Color : byte
    {
        none = 0,
        white,
        black,
    }

    public static class Constants
    {
        public static readonly byte erroneousValue = 99;
        //public static readonly pos erroneousTuple = new pos(erroneousValue, erroneousValue);
        public static readonly pos erroneousTuple = null;
        public static readonly byte xwidth = 8;
        public static readonly byte ywidth = 8;
        public static readonly byte teamSeparation = (byte)FigID.bKing;

        public static Dictionary<FigID, Color> IDtoColor = new Dictionary<FigID, Color>()
        {
            [FigID.empty] = Color.none,
            [FigID.wKing] = Color.white,
            [FigID.wQueen] = Color.white,
            [FigID.wRook] = Color.white,
            [FigID.wBishop] = Color.white,
            [FigID.wKnight] = Color.white,
            [FigID.wPawn] = Color.white,
            [FigID.wPawnEnPassantable] = Color.white,
            [FigID.bKing] = Color.black,
            [FigID.bQueen] = Color.black,
            [FigID.bRook] = Color.black,
            [FigID.bBishop] = Color.black,
            [FigID.bKnight] = Color.black,
            [FigID.bPawn] = Color.black,
            [FigID.bPawnEnPassantable] = Color.black,
        };
        public static Dictionary<string, FigID> WhiteShorthandToID = new Dictionary<string, FigID>()
        {
            ["K"] = FigID.wKing,
            ["Q"] = FigID.wQueen,
            ["N"] = FigID.wKnight,
            ["B"] = FigID.wBishop,
            ["P"] = FigID.wPawn,
            ["R"] = FigID.wRook,
        };
        public static Dictionary<FigID, string> BlackIDToShorthand = new Dictionary<FigID, string>()
        {
            [FigID.bKing] = "K",
            [FigID.bQueen] = "Q",
            [FigID.bRook] = "R",
            [FigID.bBishop] = "B",
            [FigID.bKnight] = "N",
            [FigID.bPawn] = "P",
            [FigID.bPawnEnPassantable] = "P(EP)",
        };





    }
}
