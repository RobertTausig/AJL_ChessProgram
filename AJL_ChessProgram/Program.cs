using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AJL_ChessEngine;

namespace AJL_ChessProgram
{
    using pos = Tuple<byte, byte>;
    using Lpos = List<Tuple<byte, byte>>;
    using insane = List<KeyValuePair<Stack<Move>, double>>;

    class Program
    {
        static void Main()
        {
            Console.Title = Assembly.GetExecutingAssembly().GetName().Name + " " +
                Assembly.GetExecutingAssembly().GetName().Version;
            //-----------------------------
            // Step 1: Initialise Board and all Chess Pieces:
            //-----------------------------
            var StartingState = new StdChessStartState();
            var GameBoard = new Board(StartingState);
            var Logic = new MovementLogic(StartingState);
            var TransTable = new TranspositionTable();
            var MiniMaxi = new MinMax(GameBoard, Logic, TransTable);
            Console.WriteLine("Hi, the game has started. You are playing the white player.");
            Console.WriteLine("K = King | Q = Queen | N = Knight | R = Rook | B = Bishop | P = Pawn");
            Console.WriteLine(@"Please give your moves like ""Nb1,c3""."+"\n");
            Task Worker = null;

            //var ply_1 = GameBoard.TryMove(new pos(1, 0), new pos(2, 2), FigID.wKnight);
            //var ply_2 = GameBoard.TryMove(new pos(1, 7), new pos(2, 5), FigID.bKnight);
            //var ply_3 = GameBoard.TryMove(new pos(3, 1), new pos(3, 3), FigID.wPawn);
            //var ply_4 = GameBoard.TryMove(new pos(4, 6), new pos(4, 5), FigID.bPawn);
            //var ply_5 = GameBoard.TryMove(new pos(2, 0), new pos(5, 3), FigID.wBishop);
            ////1. Nc3 Nc6  2. d4 e6  3. Bf4 
            //GameBoard.ClearLoggedMoves();

            while (true)
            {
                try
                {
                    while (true)
                    {
                        var input = Console.ReadLine();
                        if (input != "nothing")
                        {
                            //Wait for Worker if still busy:
                            if (Worker != null) { Worker.Wait(); }
                            var (notationFrom, notationTo, notationFigID) = Notation.ConsoleInputToInternal(input);
                            if (Logic.PossibleFieldsToMove(new KeyValuePair<pos, FigID>(notationFrom, notationFigID)).Contains(notationTo))
                            {
                                if (!GameBoard.TryMove(notationFrom, notationTo, notationFigID))
                                {
                                    Console.WriteLine("Your input was erroneous and was not executed.");
                                }
                                else
                                {
                                    Console.WriteLine("White Player plays: " + input);
                                    break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Your input was erroneous and was not executed.");
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    //Clear Stack:
                    GameBoard.ClearLoggedMoves();
                    //-----------------------------
                    // Step 1.5: Only for debugging:
                    //-----------------------------
                    //var ply_1 = GameBoard.TryMove(new pos(1, 0), new pos(2, 2), FigID.wKnight);
                    //var ply_2 = GameBoard.TryMove(new pos(1, 7), new pos(2, 5), FigID.bKnight);
                    //var ply_3 = GameBoard.TryMove(new pos(3, 1), new pos(3, 3), FigID.wPawn);
                    //var ply_4 = GameBoard.TryMove(new pos(4, 6), new pos(4, 5), FigID.bPawn);
                    //var ply_5 = GameBoard.TryMove(new pos(2, 0), new pos(5, 3), FigID.wBishop);
                    ////1. Nc3 Nc6  2. d4 e6  3. Bf4 
                    ////Expected by other chess engines:
                    ////var ply_6 = GameBoard.TryMove(new pos(3, 6), new pos(3, 5), FigID.bPawn); (Level 10 CPU)
                    ////var ply_6 = GameBoard.TryMove(new pos(5, 7), new pos(1, 3), FigID.bBishop); (Stockfish 11)
                    ////var ply_6 = GameBoard.TryMove(new pos(6, 7), new pos(5, 5), FigID.bKnight); (Stockfish 11, very close to above)
                    ////var ply_6 = GameBoard.TryMove(new pos(5, 7), new pos(1, 3), FigID.bBishop); (GNU Chess 6.2.5)
                    ////var ply_6 = GameBoard.TryMove(new pos(5, 7), new pos(1, 3), FigID.bBishop); (LCZero 0.26.0)

                    //GameBoard.ClearLoggedMoves();
                    //-----------------------------
                    // Step 2: Find Move:
                    //-----------------------------
                    var (bestMove, bestCounterMove) = MiniMaxi.CalculateBestMove(6, false);
                    MiniMaxi.currentAge++;
                    //-----------------------------
                    // Step 3: Play Move and put out on console:
                    //-----------------------------
                    //Play it:
                    GameBoard.TryMove(bestMove);

                    var output = Notation.InternalToConsoleOutput(bestMove.oldPos, bestMove.newPos, bestMove.movedID);
                    Console.WriteLine("Black Player plays: " + output);
                    //Clear Stack:
                    GameBoard.ClearLoggedMoves();

                    Worker = Task.Run(() =>
                    {
                        //GameBoard.TryMove(bestCounterMove);
                        //MiniMaxi.CalculateBestMove(6, false);
                        //MiniMaxi.currentAge++;
                        TransTable.TryFreeMemory();
                        //GameBoard.TryRevertLastMove();
                    });

                }
                catch (Exception e)
                {
                    Console.WriteLine("Your input was erroneous and was not executed.");
                }
            }





        }





    }
    public static class StackExtensions
    {
        public static Stack<T> myClone<T>(this Stack<T> original)
        {
            var arr = new T[original.Count];
            original.CopyTo(arr, 0);
            Array.Reverse(arr);
            return new Stack<T>(arr);
        }
    }
}
