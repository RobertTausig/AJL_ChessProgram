using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AJL_ChessEngine;

namespace AJL_ChessProgram
{
    using pos = Tuple<byte, byte>;
    using Lpos = List<Tuple<byte, byte>>;
    using plac = KeyValuePair<Tuple<byte, byte>, FigID>;
    using plm = Dictionary<Tuple<byte, byte>, FigID>;

    public class MovementLogic
    {
        public MovementLogic()
        {
            IDtoFigure.Add(FigID.wPawnEnPassantable, IDtoFigure[FigID.wPawn]);
            IDtoFigure.Add(FigID.bPawnEnPassantable, IDtoFigure[FigID.bPawn]);
            //InitialiseParallelization();
        }
        public MovementLogic(State StartState)
        {
            IDtoFigure.Add(FigID.wPawnEnPassantable, IDtoFigure[FigID.wPawn]);
            IDtoFigure.Add(FigID.bPawnEnPassantable, IDtoFigure[FigID.bPawn]);
            SetStateToAllFigures(StartState);
            //InitialiseParallelization();
            //Parallel_SetStateToAllFigures(StartState);
        }

        public readonly Dictionary<FigID, Figure> IDtoFigure = new Dictionary<FigID, Figure>()
        {
            [FigID.wPawn] = new Pawn(Color.white),
            [FigID.wKing] = new King(Color.white),
            [FigID.wQueen] = new Queen(Color.white),
            [FigID.wRook] = new Rook(Color.white),
            [FigID.wBishop] = new Bishop(Color.white),
            [FigID.wKnight] = new Knight(Color.white),
            [FigID.bPawn] = new Pawn(Color.black),
            [FigID.bKing] = new King(Color.black),
            [FigID.bQueen] = new Queen(Color.black),
            [FigID.bRook] = new Rook(Color.black),
            [FigID.bBishop] = new Bishop(Color.black),
            [FigID.bKnight] = new Knight(Color.black),
        };
        private readonly List<Dictionary<FigID, Figure>> Parallel_IDtoFigure = new List<Dictionary<FigID, Figure>>();
        private const int numPar = 16;

        public Lpos PossibleFieldsToMove(plac kvp)
        {
            SetPositionToFigure(kvp.Key, kvp.Value);
            return IDtoFigure[kvp.Value].PossibleFieldsToMove();
        }
        //Terribly slow:
        //public Dictionary<pos, List<plac>> AllPossibleFieldsToMove(plm placement)
        //{
        //    var retVal = new Dictionary<pos, List<plac>>();
        //    var tempPlacement = placement.ToList();
        //    var worker = new Task<Dictionary<plac, Lpos>>[numPar];

        //    Parallel.For(0, placement.Count, i =>
        //    {
        //        worker[i] = Task.Run(() => gg(tempPlacement[i], i));
        //    });
        //    Task.WaitAll(worker);
        //    var results = worker.SelectMany(x => x.Result).ToList();

        //    return retVal;
        //}
        //private Dictionary<plac, Lpos> gg(plac kvp, int i)
        //{
        //    Parallel_IDtoFigure[i][kvp.Value].position = kvp.Key;
        //    return new Dictionary<plac, Lpos>() { [kvp] = Parallel_IDtoFigure[i][kvp.Value].PossibleFieldsToMove() };
        //}

        protected void SetStateToAllFigures(State givenState)
        {
            foreach (var (_, val) in IDtoFigure)
            {
                val.state = givenState;
            }
        }
        protected void SetPositionToFigure(pos givenPosition, FigID givenID)
        {
            IDtoFigure[givenID].position = givenPosition;
        }
        //private void InitialiseParallelization()
        //{
        //    for (int i = 0; i < numPar; i++)
        //    {
        //        Parallel_IDtoFigure.Add(new Dictionary<FigID, Figure>()
        //        {
        //            [FigID.wPawn] = new Pawn(Color.white),
        //            [FigID.wKing] = new King(Color.white),
        //            [FigID.wQueen] = new Queen(Color.white),
        //            [FigID.wRook] = new Rook(Color.white),
        //            [FigID.wBishop] = new Bishop(Color.white),
        //            [FigID.wKnight] = new Knight(Color.white),
        //            [FigID.bPawn] = new Pawn(Color.black),
        //            [FigID.bKing] = new King(Color.black),
        //            [FigID.bQueen] = new Queen(Color.black),
        //            [FigID.bRook] = new Rook(Color.black),
        //            [FigID.bBishop] = new Bishop(Color.black),
        //            [FigID.bKnight] = new Knight(Color.black),
        //        });
        //        Parallel_IDtoFigure[i].Add(FigID.wPawnEnPassantable, IDtoFigure[FigID.wPawn]);
        //        Parallel_IDtoFigure[i].Add(FigID.bPawnEnPassantable, IDtoFigure[FigID.bPawn]);
        //    }
        //}
        //private void Parallel_SetStateToAllFigures(State givenState)
        //{
        //    foreach (var entry in Parallel_IDtoFigure)
        //    {
        //        foreach (var (_, val) in entry)
        //        {
        //            val.state = givenState;
        //        }
        //    }
        //}








    }
}
