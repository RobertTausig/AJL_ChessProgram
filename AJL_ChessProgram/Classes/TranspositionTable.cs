using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
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
        private const int allowedNumberEntries = 3_000_000;

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
        public bool TryAdd(Transposition trsp, ulong hash)
        {
            try
            {
                if (!this.Contains(hash))
                {
                    //Node is unkown. Add:
                    transTab.Add(hash, trsp);
                    return true;
                }
                else if (trsp.distanceFromLeaf > transTab[hash].distanceFromLeaf)
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

        //Shall never be executed in main thread.
        public bool TryFreeMemory()
        {
            if (transTab.Count > allowedNumberEntries)
            {
                var orderedTransTab = transTab.OrderBy(x => x.Value.age).ToList();
                var lowestAge = orderedTransTab.First().Value.age;
                var highestAge = transTab.OrderByDescending(x => x.Value.age).First().Value.age;

                foreach (var (key,_) in orderedTransTab)
                {
                    if (transTab[key].age == lowestAge)
                    {
                        transTab.Remove(key);
                    }
                    else
                    {
                        break;
                    }
                }
                //Repeat until enough memory is freed:
                TryFreeMemory();
                return true;
            }
            else
            {
                return false;
            }
        }


    }


    internal struct Transposition
    {
        public Transposition(NodeType nodeType, byte distanceFromLeaf, byte age, double score)
        {
            this.nodeType = nodeType;
            this.distanceFromLeaf = distanceFromLeaf;
            this.age = age;
            this.score = score;
        }
        public readonly NodeType nodeType;
        public readonly byte distanceFromLeaf;
        public readonly byte age;
        public readonly double score;
    }

}
