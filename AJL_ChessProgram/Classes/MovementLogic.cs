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
        }
        public MovementLogic(State StartState)
        {
            IDtoFigure.Add(FigID.wPawnEnPassantable, IDtoFigure[FigID.wPawn]);
            IDtoFigure.Add(FigID.bPawnEnPassantable, IDtoFigure[FigID.bPawn]);
            SetStateToAllFigures(StartState);
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








    }
}
