using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static AJL_ChessEngine.Constants;


namespace AJL_ChessEngine
{
    using pos = Tuple<byte, byte>;
    using Lpos = List<Tuple<byte, byte>>;

    class Program
    {
        //Just for testing the to-be dll:
        static void Main()
        {
            //Stopwatch stopwatch = Stopwatch.StartNew();

            var Boardy = new Board(new StdChessStartState());
            var aa = new pos(3, 3);
            var TestFigure = new Pawn(Boardy.state, new pos(2,1), Color.black);
            var TestFigure2 = new Pawn(Boardy.state, new pos(2,6), Color.white);
            var bb = new Lpos();
            var cc = new Lpos();
            for (int i = 0; i < 1; i++)
            {
                bb = TestFigure.PossibleFieldsToMove();
                cc = TestFigure2.PossibleFieldsToMove();
            }
            var jj = Boardy.state;
            jj[3, 0] = FigID.empty;
            jj[4, 4] = FigID.empty;
            jj[4, 5] = FigID.wPawnEnPassantable;

            Boardy.TryMove(new pos(1, 0), new pos(2, 2), FigID.wKnight);
            Boardy.TryRevertLastMove();
            Boardy.TryMove(new pos(1, 0), new pos(2, 2), FigID.wKnight);

            //stopwatch.Stop();
            //Console.WriteLine(stopwatch.ElapsedMilliseconds);




        }
    }
}
