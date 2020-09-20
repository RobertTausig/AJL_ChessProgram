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

    public enum NodeType : byte
    {
        unkown,
        exact,          //Score of this node is exact. Also called "PV-Node"
        lowerBound,     //Score of this node might be higher. Also called "Cut-Node"
        higherBound     //Score of this node might be lower. Also called "All-Node"
    }



}
