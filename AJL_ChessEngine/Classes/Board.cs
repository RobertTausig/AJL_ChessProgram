using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static AJL_ChessEngine.Constants;
using System.ComponentModel;

namespace AJL_ChessEngine
{
    using pos = Tuple<byte, byte>;
    using Lpos = List<Tuple<byte, byte>>;

    public class Board
    {
        public Board() { }
        public Board(State StartState)
        {
            _state = StartState;
        }

        private State _state = new State();
        public State state {
            get => _state;
            set
            {
                if (IsNewStateEvenPossibleWithinAPly(_state, value))
                {
                    _state = value;
                }
                else
                {
                    throw new ArgumentException("State not reachable within a ply.");
                }
            }
        }
        public Stack<Move> LoggedMoves { get; } = new Stack<Move>();

        private bool IsNewStateEvenPossibleWithinAPly(State oldState, State newState)
        {
            List<(int, int)> positionsOfChange = new List<(int, int)>(); 
            //Check that no figure dissappeared into thin air:
            for (int x = 0; x < xwidth; x++)
            {
                for (int y = 0; y < ywidth; y++)
                {
                    if (oldState[x, y] != newState[x, y])
                    {
                        positionsOfChange.Add((x, y));
                    }
                }
            }
            //2 is for a typical ply, 4 for a rokade:
            if (!(positionsOfChange.Count == 2 || positionsOfChange.Count == 4))
            {
                return false;
            }

            //At least one field must be empty afterwards:
            if (!positionsOfChange.Exists(x => newState[x.Item1, x.Item2] == FigID.empty))
            {
                return false;
            }

            return true;
        }
        public bool TryMove(pos from, pos to, FigID ident)
        {
            try
            {
                //Can be deleted later, just for verification:
                var movedID = state[from.Item1, from.Item2];
                if (movedID != ident) { return false; }

                var removedID = state[to.Item1, to.Item2];
                LoggedMoves.Push(new Move(from, to, movedID, removedID));

                state[from.Item1, from.Item2] = FigID.empty;
                state[to.Item1, to.Item2] = ident;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool TryMove(Move givenMove)
        {
            try
            {
                //Can be deleted later, just for verification:
                var movedID = state[givenMove.oldPos.Item1, givenMove.oldPos.Item2];
                if (movedID != givenMove.movedID) { return false; }
                var removedID = state[givenMove.newPos.Item1, givenMove.newPos.Item2];
                if (removedID != givenMove.removedID) { return false; }

                LoggedMoves.Push(givenMove);

                state[givenMove.oldPos.Item1, givenMove.oldPos.Item2] = FigID.empty;
                state[givenMove.newPos.Item1, givenMove.newPos.Item2] = givenMove.movedID;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool TryRevertLastMove()
        {
            try
            {
                if (LoggedMoves.Count == 0) { return false; }
                var LastMove = LoggedMoves.Pop();
                state[LastMove.oldPos.Item1, LastMove.oldPos.Item2] = LastMove.movedID;
                state[LastMove.newPos.Item1, LastMove.newPos.Item2] = LastMove.removedID;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void ClearLoggedMoves()
        {
            LoggedMoves.Clear();
        }






    }
}
