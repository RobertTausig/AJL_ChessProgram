using AJL_ChessEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace AJL_ChessProgram
{
    using pos = Tuple<byte, byte>;
    using Lpos = List<Tuple<byte, byte>>;

    public static class Notation
    {
        /// <summary>
        /// Returns null if could not be converted.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static (pos notationFrom, pos notationTo, FigID notationFigID) ConsoleInputToInternal(string s)
        {
            try
            {
                var input = s.Split(",");
                var xFrom = (byte)(char.ToUpper(input[0][1]) - 65);
                var xTo = (byte)(char.ToUpper(input[1][0]) - 65);
                var yFrom = (byte)(input[0][2] - '0' - 1);
                var yTo = (byte)(input[1][1] - '0' - 1);
                var inputFigID = Constants.WhiteShorthandToID[input[0][0].ToString()];

                return (new pos(xFrom, yFrom), new pos(xTo, yTo), inputFigID);
            }
            catch
            {
                return (null, null, FigID.empty);
            }
        }
        /// <summary>
        /// Returns null if could not be converted.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string InternalToConsoleOutput(pos from, pos to, FigID figID)
        {
            try
            {
                var xFrom = char.ToLower((char)(from.Item1 + 65));
                var yFrom = (from.Item2 + 1).ToString();
                var xTo = char.ToLower((char)(to.Item1 + 65));
                var yTo = (to.Item2 + 1).ToString();
                var outputShorthand = Constants.BlackIDToShorthand[figID];

                return outputShorthand + xFrom + yFrom + "," + xTo + yTo;
            }
            catch
            {
                return null;
            }
        }
        

    }
}
