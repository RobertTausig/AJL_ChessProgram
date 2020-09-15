using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static AJL_ChessEngine.Constants;


namespace AJL_ChessEngine
{
    using pos = Tuple<byte, byte>;
    using Lpos = List<Tuple<byte, byte>>;

    public class Bishop : Figure
    {
        public Bishop(Color givenTeam) : base(givenTeam)
        {
            id = givenTeam == Color.white ? (byte)FigID.wBishop : (byte)FigID.bBishop;
            worth = 3;
        }
        public Bishop(State givenState, pos givenPosition, Color givenTeam) : base(givenState, givenPosition, givenTeam)
        {
            id = givenTeam == Color.white ? (byte)FigID.wBishop : (byte)FigID.bBishop;
            worth = 3;
        }

        public override byte id { get => _id; protected set => _id = value; }
        public override Color team { get => _team; protected set => _team = value; }
        public override double worth { get => _worth; protected set => _worth = value; }

        public override Lpos PossibleFieldsToMove()
        {
            var retVal = new Lpos();

            retVal.AddRange(NorthEast());
            retVal.AddRange(NorthWest());
            retVal.AddRange(SoutEast());
            retVal.AddRange(SoutWest());

            //Remove erroneous:
            retVal.RemoveAll(x => x == erroneousTuple);

            return retVal;
        }
    }
}
