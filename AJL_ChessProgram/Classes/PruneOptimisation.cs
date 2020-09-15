using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AJL_ChessEngine;

namespace AJL_ChessProgram
{
    using pos = Tuple<byte, byte>;
    using Lpos = List<Tuple<byte, byte>>;
    using plac = KeyValuePair<Tuple<byte, byte>, FigID>;
    using plm = Dictionary<Tuple<byte, byte>, FigID>;


    public class PruneOptimisation
    {
        public PruneOptimisation(Board GameBoard, MovementLogic Logic)
        {
            this.GameBoard = GameBoard;
            this.Logic = Logic;
        }

        Board GameBoard;
        MovementLogic Logic;
        //To, <From, FigID>
        Dictionary<pos, List<plac>> FieldsReachableByWhom = new Dictionary<pos, List<plac>>();


        public List<Move> OrderedMoves(bool isWhitePlayer)
        {
            var retVal = new List<Move>();
            var tempList = new List<Move>();
            plm ownPlacement, opponentPlacement;
            if (isWhitePlayer)
            {
                ownPlacement = GameBoard.state.WhiteFigurePlacement;
                opponentPlacement = GameBoard.state.BlackFigurePlacement;
            }
            else
            {
                ownPlacement = GameBoard.state.BlackFigurePlacement;
                opponentPlacement = GameBoard.state.WhiteFigurePlacement;
            }
            //----------------------------------------------
            //Step 0: Get all possible moves available to Player.
            //----------------------------------------------
            foreach (var plac in ownPlacement)
            {
                foreach (var pofimo in Logic.PossibleFieldsToMove(plac))
                {
                    if (FieldsReachableByWhom.ContainsKey(pofimo))
                    {
                        FieldsReachableByWhom[pofimo].Add(plac);
                    }
                    else
                    {
                        FieldsReachableByWhom.Add(pofimo, new List<plac>() { plac });
                    }
                }
            }
            //----------------------------------------------
            //Step 1: Order by moves that capture.
            //Step 1.1: First low value pieces capturing high value pieces. Then HVP capturing LVP.
            //      (If possible here or later, explicitly try to capture the last moved figure by the enemy first)
            //----------------------------------------------
            foreach (var plac in opponentPlacement)
            {
                if (FieldsReachableByWhom.ContainsKey(plac.Key))
                {
                    //A piece can be captured.
                    foreach (var captor in FieldsReachableByWhom[plac.Key])
                    {
                        tempList.Add(new Move(captor.Key, plac.Key, captor.Value, plac.Value));
                    }
                    //Remove to not have duplicates and accelerate later searches:
                    FieldsReachableByWhom.Remove(plac.Key);
                }
            }
            retVal.AddRange(tempList.OrderByDescending(x => Logic.IDtoFigure[x.removedID].worth - Logic.IDtoFigure[x.movedID].worth));
            tempList.Clear();
            //----------------------------------------------
            //Step 2: Order by moving in general direction of opponent.
            //----------------------------------------------
            var temp = new List<Move>();
            foreach (var fieldKey in FieldsReachableByWhom)
            {
                foreach (var kvp in fieldKey.Value)
                {
                        tempList.Add(new Move(kvp.Key, fieldKey.Key, kvp.Value, FigID.empty));
                }
            }

            if (isWhitePlayer)
            {
                retVal.AddRange(tempList.OrderByDescending(y => y.newPos.Item2 - y.oldPos.Item2).ToList());
            }
            else
            {
                retVal.AddRange(tempList.OrderBy(y => y.newPos.Item2 - y.oldPos.Item2).ToList());
            }
            //Clean up:
            FieldsReachableByWhom.Clear();

            return retVal;
        }






    }
}
