using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using static AJL_ChessEngine.Constants;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Net.Security;

namespace AJL_ChessEngine
{
    using pos = Tuple<byte, byte>;
    using Lpos = List<Tuple<byte, byte>>;

    public abstract class Figure
    {
        public Figure(Color givenTeam)
        {
            team = givenTeam != Color.none ? givenTeam : throw new ArgumentException("Figure can't have Color.none");
            teamSign = team == Color.white ? 1 : -1;
        }
        public Figure(State givenState, pos givenPosition, Color givenTeam)
        {
            state = givenState;
            position = givenPosition;
            team = givenTeam != Color.none ? givenTeam : throw new ArgumentException("Figure can't have Color.none");
            teamSign = team == Color.white ? 1 : -1;
        }

        protected byte _id;
        public abstract byte id { get; protected set; }
        protected Color _team;
        public abstract Color team { get; protected set; }
        protected double _worth;
        public abstract double worth { get; protected set; }

        public pos position { get; set; } = erroneousTuple;
        public State state;
        protected int teamSign { get; } = 0;


        public void ReDefine(State givenState, pos givenPosition)
        {
            state = givenState;
            position = givenPosition;
        }
        public abstract Lpos PossibleFieldsToMove();

        //----------------------------------
        // BASIC MOVEMENT
        //----------------------------------
        private bool AllowExecution()
        {
            if (position != erroneousTuple) { return true; }
            else { return false; }
        }
        //Will only work for 8x8 State:
        private bool IsEdgeField()
        {
            if ((position.Item1 * position.Item2) % 7 == 0) { return true; }
            else { return false; }
        }

        //----------------------------------
        // Possibly more than one field
        //----------------------------------
        protected Lpos Up()
        {
            var retVal = new Lpos();
            if (AllowExecution())
            {
                for (int i = position.Item2 + 1; i < ywidth; i++)
                {
                    if (state[position.Item1, i] == FigID.empty)
                    {
                        //Empty field, Color = none
                        retVal.Add(new pos(position.Item1, (byte)i));
                        continue;
                    }
                    else if (state.ColorOfField(position.Item1, i) != team)
                    {
                        //Opposite team field
                        retVal.Add(new pos(position.Item1, (byte)i));
                        break;
                    }
                    else
                    {
                        //Same team field
                        break;
                    }
                }
            }
            return retVal;
        }
        protected Lpos Down()
        {
            var retVal = new Lpos();
            if (AllowExecution())
            {
                for (int i = position.Item2 - 1; i >= 0; i--)
                {
                    if (state[position.Item1, i] == FigID.empty)
                    {
                        //Empty field, Color = none
                        retVal.Add(new pos(position.Item1, (byte)i));
                        continue;
                    }
                    else if (state.ColorOfField(position.Item1, i) != team)
                    {
                        //Opposite team field
                        retVal.Add(new pos(position.Item1, (byte)i));
                        break;
                    }
                    else
                    {
                        //Same team field
                        break;
                    }
                }
            }
            return retVal;
        }
        protected Lpos Right()
        {
            var retVal = new Lpos();
            if (AllowExecution())
            {
                for (int i = position.Item1 + 1; i < xwidth; i++)
                {
                    if (state[i, position.Item2] == FigID.empty)
                    {
                        //Empty field, Color = none
                        retVal.Add(new pos((byte)i, position.Item2));
                        continue;
                    }
                    else if (state.ColorOfField(i, position.Item2) != team)
                    {
                        //Opposite team field
                        retVal.Add(new pos((byte)i, position.Item2));
                        break;
                    }
                    else
                    {
                        //Same team field
                        break;
                    }
                }
            }
            return retVal;
        }
        protected Lpos Left()
        {
            var retVal = new Lpos();
            if (AllowExecution())
            {
                for (int i = position.Item1 - 1; i >= 0; i--)
                {
                    if (state[i, position.Item2] == FigID.empty)
                    {
                        //Empty field, Color = none
                        retVal.Add(new pos((byte)i, position.Item2));
                        continue;
                    }
                    else if (state.ColorOfField(i, position.Item2) != team)
                    {
                        //Opposite team field
                        retVal.Add(new pos((byte)i, position.Item2));
                        break;
                    }
                    else
                    {
                        //Same team field
                        break;
                    }
                }
            }
            return retVal;
        }
        protected Lpos NorthEast()
        {
            var retVal = new Lpos();
            if (AllowExecution())
            {
                int xPos, yPos;
                for (int i = 1; position.Item1 + i < xwidth && position.Item2 + i < ywidth; i++)
                {
                    xPos = position.Item1 + i;
                    yPos = position.Item2 + i;
                    if (state[xPos, yPos] == FigID.empty)
                    {
                        //Empty field, Color = none
                        retVal.Add(new pos((byte)xPos, (byte)yPos));
                        continue;
                    }
                    else if (state.ColorOfField(xPos, yPos) != team)
                    {
                        //Opposite team field
                        retVal.Add(new pos((byte)xPos, (byte)yPos));
                        break;
                    }
                    else
                    {
                        //Same team field
                        break;
                    }
                }
            }
            return retVal;
        }
        protected Lpos SoutWest()
        {
            var retVal = new Lpos();
            if (AllowExecution())
            {
                int xPos, yPos;
                for (int i = 1; position.Item1 - i >= 0 && position.Item2 - i >= 0; i++)
                {
                    xPos = position.Item1 - i;
                    yPos = position.Item2 - i;
                    if (state[xPos, yPos] == FigID.empty)
                    {
                        //Empty field, Color = none
                        retVal.Add(new pos((byte)xPos, (byte)yPos));
                        continue;
                    }
                    else if (state.ColorOfField(xPos, yPos) != team)
                    {
                        //Opposite team field
                        retVal.Add(new pos((byte)xPos, (byte)yPos));
                        break;
                    }
                    else
                    {
                        //Same team field
                        break;
                    }
                }
            }
            return retVal;
        }
        protected Lpos SoutEast()
        {
            var retVal = new Lpos();
            if (AllowExecution())
            {
                int xPos, yPos;
                for (int i = 1; position.Item1 + i < xwidth && position.Item2 - i >= 0; i++)
                {
                    xPos = position.Item1 + i;
                    yPos = position.Item2 - i;
                    if (state[xPos, yPos] == FigID.empty)
                    {
                        //Empty field, Color = none
                        retVal.Add(new pos((byte)xPos, (byte)yPos));
                        continue;
                    }
                    else if (state.ColorOfField(xPos, yPos) != team)
                    {
                        //Opposite team field
                        retVal.Add(new pos((byte)xPos, (byte)yPos));
                        break;
                    }
                    else
                    {
                        //Same team field
                        break;
                    }
                }
            }
            return retVal;
        }
        protected Lpos NorthWest()
        {
            var retVal = new Lpos();
            if (AllowExecution())
            {
                int xPos, yPos;
                for (int i = 1; position.Item1 - i >= 0 && position.Item2 + i < ywidth; i++)
                {
                    xPos = position.Item1 - i;
                    yPos = position.Item2 + i;
                    if (state[xPos, yPos] == FigID.empty)
                    {
                        //Empty field, Color = none
                        retVal.Add(new pos((byte)xPos, (byte)yPos));
                        continue;
                    }
                    else if (state.ColorOfField(xPos, yPos) != team)
                    {
                        //Opposite team field
                        retVal.Add(new pos((byte)xPos, (byte)yPos));
                        break;
                    }
                    else
                    {
                        //Same team field
                        break;
                    }
                }
            }
            return retVal;
        }

        //----------------------------------
        // One field at most
        //----------------------------------
        protected pos UpOne()
        {
            var retVal = erroneousTuple;
            if (AllowExecution())
            {
                if (position.Item2 < ywidth - 1 && state.ColorOfField(position.Item1, position.Item2 + 1) != team)
                {
                    retVal = new pos(position.Item1, (byte)(position.Item2 + 1));
                }
            }
            return retVal;
        }
        protected pos DownOne()
        {
            var retVal = erroneousTuple;
            if (AllowExecution())
            {
                if (position.Item2 > 0 && state.ColorOfField(position.Item1, position.Item2 - 1) != team)
                {
                    retVal = new pos(position.Item1, (byte)(position.Item2 - 1));
                }
            }
            return retVal;
        }
        protected pos RightOne()
        {
            var retVal = erroneousTuple;
            if (AllowExecution())
            {
                if (position.Item1 < xwidth - 1 && state.ColorOfField(position.Item1 + 1, position.Item2) != team)
                {
                    retVal = new pos((byte)(position.Item1 + 1), position.Item2);
                }
            }
            return retVal;
        }
        protected pos LeftOne()
        {
            var retVal = erroneousTuple;
            if (AllowExecution())
            {
                if (position.Item1 > 0 && state.ColorOfField(position.Item1 - 1, position.Item2) != team)
                {
                    retVal = new pos((byte)(position.Item1 - 1), position.Item2);
                }
            }
            return retVal;
        }
        protected pos NorthEastOne()
        {
            var retVal = erroneousTuple;
            if (AllowExecution() && (position.Item1 < xwidth - 1 && position.Item2 < ywidth - 1))
            {
                if (state.ColorOfField(position.Item1 + 1, position.Item2 + 1) != team)
                {
                    retVal = new pos((byte)(position.Item1 + 1), (byte)(position.Item2 + 1));
                }
            }
            return retVal;
        }
        protected pos SouthEastOne()
        {
            var retVal = erroneousTuple;
            if (AllowExecution() && (position.Item1 < xwidth - 1 && position.Item2 > 0))
            {
                if (state.ColorOfField(position.Item1 + 1, position.Item2 - 1) != team)
                {
                    retVal = new pos((byte)(position.Item1 + 1), (byte)(position.Item2 - 1));
                }
            }
            return retVal;
        }
        protected pos SouthWestOne()
        {
            var retVal = erroneousTuple;
            if (AllowExecution() && (position.Item1 > 0 && position.Item2 > 0))
            {
                if (state.ColorOfField(position.Item1 - 1, position.Item2 - 1) != team)
                {
                    retVal = new pos((byte)(position.Item1 - 1), (byte)(position.Item2 - 1));
                }
            }
            return retVal;
        }
        protected pos NorthWestOne()
        {
            var retVal = erroneousTuple;
            if (AllowExecution() && (position.Item1 > 0 && position.Item2 < ywidth - 1))
            {
                if (state.ColorOfField(position.Item1 - 1, position.Item2 + 1) != team)
                {
                    retVal = new pos((byte)(position.Item1 - 1), (byte)(position.Item2 + 1));
                }
            }
            return retVal;
        }

        //----------------------------------
        // Special Movement
        //----------------------------------
        protected Lpos KnightJumps()
        {
            var retVal = new Lpos();
            if (AllowExecution())
            {
                int xPos = position.Item1, yPos = position.Item2;
                //Western Hemisphere
                if (xPos - 2 >= 0)
                {
                    if (yPos - 2 >= 0)
                    {
                        //-2,-1
                        //-1,-2
                        if (state.ColorOfField(xPos - 2, yPos - 1) != team)
                        {
                            retVal.Add(new pos((byte)(xPos - 2), (byte)(yPos - 1)));
                        }
                        if (state.ColorOfField(xPos - 1, yPos - 2) != team)
                        {
                            retVal.Add(new pos((byte)(xPos - 1), (byte)(yPos - 2)));
                        }
                    }
                    else if (yPos - 1 >= 0)
                    {
                        //-2,-1
                        if (state.ColorOfField(xPos - 2, yPos - 1) != team)
                        {
                            retVal.Add(new pos((byte)(xPos - 2), (byte)(yPos - 1)));
                        }
                    }

                    if (yPos + 2 < ywidth)
                    {
                        //-2,1
                        //-1,2
                        if (state.ColorOfField(xPos - 2, yPos + 1) != team)
                        {
                            retVal.Add(new pos((byte)(xPos - 2), (byte)(yPos + 1)));
                        }
                        if (state.ColorOfField(xPos - 1, yPos + 2) != team)
                        {
                            retVal.Add(new pos((byte)(xPos - 1), (byte)(yPos + 2)));
                        }
                    }
                    else if (yPos + 1 < ywidth)
                    {
                        //-2,1
                        if (state.ColorOfField(xPos - 2, yPos + 1) != team)
                        {
                            retVal.Add(new pos((byte)(xPos - 2), (byte)(yPos + 1)));
                        }
                    }
                }
                else if (xPos - 1 >= 0)
                {
                    if (yPos - 2 >= 0)
                    {
                        //-1,-2
                        if (state.ColorOfField(xPos - 1, yPos - 2) != team)
                        {
                            retVal.Add(new pos((byte)(xPos - 1), (byte)(yPos - 2)));
                        }
                    }

                    if (yPos + 2 < ywidth)
                    {
                        //-1,2
                        if (state.ColorOfField(xPos - 1, yPos + 2) != team)
                        {
                            retVal.Add(new pos((byte)(xPos - 1), (byte)(yPos + 2)));
                        }
                    }
                }

                //Eastern Hemisphere
                if (xPos + 2 < xwidth)
                {
                    if (yPos - 2 >= 0)
                    {
                        //2,-1
                        //1,-2
                        if (state.ColorOfField(xPos + 2, yPos - 1) != team)
                        {
                            retVal.Add(new pos((byte)(xPos + 2), (byte)(yPos - 1)));
                        }
                        if (state.ColorOfField(xPos + 1, yPos - 2) != team)
                        {
                            retVal.Add(new pos((byte)(xPos + 1), (byte)(yPos - 2)));
                        }
                    }
                    else if (yPos - 1 >= 0)
                    {
                        //2,-1
                        if (state.ColorOfField(xPos + 2, yPos - 1) != team)
                        {
                            retVal.Add(new pos((byte)(xPos + 2), (byte)(yPos - 1)));
                        }
                    }

                    if (yPos + 2 < ywidth)
                    {
                        //2,1
                        //1,2
                        if (state.ColorOfField(xPos + 2, yPos + 1) != team)
                        {
                            retVal.Add(new pos((byte)(xPos + 2), (byte)(yPos + 1)));
                        }
                        if (state.ColorOfField(xPos + 1, yPos + 2) != team)
                        {
                            retVal.Add(new pos((byte)(xPos + 1), (byte)(yPos + 2)));
                        }
                    }
                    else if (yPos + 1 < ywidth)
                    {
                        //2,1
                        if (state.ColorOfField(xPos + 2, yPos + 1) != team)
                        {
                            retVal.Add(new pos((byte)(xPos + 2), (byte)(yPos + 1)));
                        }
                    }
                }
                else if (xPos + 1 < xwidth)
                {
                    if (yPos - 2 >= 0)
                    {
                        //1,-2
                        if (state.ColorOfField(xPos + 1, yPos - 2) != team)
                        {
                            retVal.Add(new pos((byte)(xPos + 1), (byte)(yPos - 2)));
                        }
                    }

                    if (yPos + 2 < ywidth)
                    {
                        //1,2
                        if (state.ColorOfField(xPos + 1, yPos + 2) != team)
                        {
                            retVal.Add(new pos((byte)(xPos + 1), (byte)(yPos + 2)));
                        }
                    }
                }


            }
            return retVal;
        }
        protected Lpos WhitePawnMoves()
        {
            var retVal = new Lpos();
            if (AllowExecution())
            {
                switch (position.Item2)
                {
                    case 1:     //Starting Position
                        if (state[position.Item1, position.Item2 + 1] == FigID.empty)
                        {
                            retVal.AddRange(PawnWithoutSpecials(Color.black));
                            //Can move 2 fields since at starting position:
                            if (state[position.Item1, position.Item2 + 2] == FigID.empty)
                            {
                                retVal.Add(new pos(position.Item1, (byte)(position.Item2 + 2)));
                            }
                        }
                        break;
                    case 4:     //En Passant capture possible
                        retVal.AddRange(PawnWithoutSpecials(Color.black));
                        retVal.AddRange(PawnEnPassantCapture(FigID.bPawnEnPassantable));
                        break;
                    case var n when (n < ywidth - 1 && 1 < n):
                        retVal.AddRange(PawnWithoutSpecials(Color.black));
                        break;
                }
            }
            return retVal;
        }
        protected Lpos BlackPawnMoves()
        {
            var retVal = new Lpos();
            if (AllowExecution())
            {
                switch (position.Item2)
                {
                    case 6:     //Starting Position
                        if (state[position.Item1, position.Item2 - 1] == FigID.empty)
                        {
                            retVal.AddRange(PawnWithoutSpecials(Color.white));
                            //Can move 2 fields since at starting position:
                            if (state[position.Item1, position.Item2 - 2] == FigID.empty)
                            {
                                retVal.Add(new pos(position.Item1, (byte)(position.Item2 - 2)));
                            }
                        }
                        break;
                    case 3:     //En Passant capture possible
                        retVal.AddRange(PawnWithoutSpecials(Color.white));
                        retVal.AddRange(PawnEnPassantCapture(FigID.wPawnEnPassantable));
                        break;
                    case var n when (0 < n && n < ywidth - 2):
                        retVal.AddRange(PawnWithoutSpecials(Color.white));
                        break;
                }
            }
            return retVal;
        }
        private Lpos PawnWithoutSpecials(Color oppositeColor)
        {
            var retVal = new Lpos();

            if (state[position.Item1, position.Item2 + teamSign] == FigID.empty)
            {
                retVal.Add(new pos(position.Item1, (byte)(position.Item2 + teamSign)));
            }
            if (position.Item1 < xwidth - 1 && state.ColorOfField(position.Item1 + 1, position.Item2 + teamSign) == oppositeColor)
            {
                retVal.Add(new pos((byte)(position.Item1 + 1), (byte)(position.Item2 + teamSign)));
            }
            if (position.Item1 > 0 && state.ColorOfField(position.Item1 - 1, position.Item2 + teamSign) == oppositeColor)
            {
                retVal.Add(new pos((byte)(position.Item1 - 1), (byte)(position.Item2 + teamSign)));
            }
            return retVal;
        }
        private Lpos PawnEnPassantCapture(FigID oppositeFigID)
        {
            var retVal = new Lpos();

            if (position.Item1 < xwidth - 1 && state[position.Item1 + 1, position.Item2] == oppositeFigID)
            {
                retVal.Add(new pos((byte)(position.Item1 + 1), (byte)(position.Item2 + 1)));
            }
            if (position.Item1 > 0 && state[position.Item1 - 1, position.Item2] == oppositeFigID)
            {
                retVal.Add(new pos((byte)(position.Item1 - 1), (byte)(position.Item2 + 1)));
            }
            return retVal;
        }




    } //End class








} //End namespace
