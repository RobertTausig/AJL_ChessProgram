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

    class TranspositionTable
    {

        protected Dictionary<ulong, Transposition> transTab = new Dictionary<ulong, Transposition>();
        private Transposition stdTrsp;

        public bool Contains(ulong hash)
        {
            return transTab.ContainsKey(hash);
        }
        public (bool successful, Transposition trsp) TryGet(int currentDepth, int maxDepth, ulong hash)
        {
            var successful = transTab.TryGetValue(hash, out stdTrsp);
            if (successful && stdTrsp.distanceFromLeaf + currentDepth >= maxDepth)
            {
                return (true, stdTrsp);
            }
            else
            {
                return (false, stdTrsp);
            }
        }
        public bool TryAdd(Transposition trsp, int currentDepth, int maxDepth, ulong hash)
        {
            try
            {
                if (!this.Contains(hash))
                {
                    //Node is unkown. Add:
                    transTab.Add(hash, trsp);
                    return true;
                }
                else if (trsp.distanceFromLeaf < (maxDepth - currentDepth))
                {
                    //Same node with a deeper information was found. Replace old one:
                    transTab[hash] = trsp;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }


        }
    }


    internal struct Transposition
    {
        public Transposition(bool isUpperLimit, byte distanceFromLeaf, byte age, double score)
        {
            this.isUpperLimit = isUpperLimit;
            this.distanceFromLeaf = distanceFromLeaf;
            this.age = age;
            this.score = score;
        }
        public readonly bool isUpperLimit;
        public readonly byte distanceFromLeaf;
        public readonly byte age;
        public readonly double score;
    }

}
