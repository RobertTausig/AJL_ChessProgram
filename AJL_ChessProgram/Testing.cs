using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Transactions;
using AJL_ChessEngine;
using SimpleGameTree;

namespace AJL_ChessProgram
{
    using pos = Tuple<byte, byte>;
    using Lpos = List<Tuple<byte, byte>>;
    using insane = List<KeyValuePair<Stack<Move>, double>>;

    class Testing
    {
        static void Main()
        {
            Console.Title = Assembly.GetExecutingAssembly().GetName().Name + " " +
                Assembly.GetExecutingAssembly().GetName().Version;

            var Maker = new AJL_TreeMaker();
            int maxDepth = 6, branching = 7, numRandValues = 100;
            var Tree = Maker.MakeTree(maxDepth, branching, numRandValues);

            ClassicMinMax(Tree);
            // Results:
            //  node 1: 82
            //  node 2: 80
            //  node 3: 83
            //  node 4: 81
            //  node 5: 79
            //  node 6: 79
            //  node 7: 78  <--- Best one.






        }

        public static void ClassicMinMax(AJL_Tree Tree)
        {
            var currentDepth = Tree.depthCollection.Last().Key - 1;

            for (int i = currentDepth; i >= 0; i--)
            {
                ClassicMinMax_Iteration(i, Tree);
            }
        }
        private static void ClassicMinMax_Iteration(int currentDepth, AJL_Tree Tree)
        {
            bool isMaximisingPlayer = currentDepth % 2 != 0 ? true : false; //0,2,4...false, 1,3,5...true
            foreach (var node in Tree.depthCollection[currentDepth])
            {
                if (isMaximisingPlayer)
                {
                    double tempScore = -1_000;
                    foreach (var child in node.childIDs)
                    {
                        tempScore = Math.Max(Tree.identCollection[child].score, tempScore);
                    }
                    node.score = tempScore;
                }
                else
                {
                    double tempScore = 1_000;
                    foreach (var child in node.childIDs)
                    {
                        tempScore = Math.Min(Tree.identCollection[child].score, tempScore);
                    }
                    node.score = tempScore;
                }
            }
        }





    }
}
