using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace SimpleGameTree
{
    public class AJL_TreeMaker
    {

        public AJL_Tree Tree = new AJL_Tree();

        public AJL_Tree MakeTree (int maxDepth, int branching, int numOfRandomLeafNodeValues, bool deterministicRandomness = true)
        {
            for (int i = 0; i < maxDepth; i++)
            {
                NextDepthPopulation(i, branching);
            }

            //Initialise leaf node values:
            Random rand;
            if (deterministicRandomness) { rand = new Random(62); }
            else { rand = new Random(); }

            foreach (var leafNode in Tree.depthCollection[maxDepth])
            {
                leafNode.score = rand.Next(numOfRandomLeafNodeValues);
            }

            return Tree;
        }
        private void NextDepthPopulation (int currentDepth, int branching)
        {
            foreach (var child in Tree.depthCollection[currentDepth])
            {
                for (int i = 0; i < branching; i++)
                {
                    Tree.AddNode(child.nodeID);
                }
            }
        }





    }
}
