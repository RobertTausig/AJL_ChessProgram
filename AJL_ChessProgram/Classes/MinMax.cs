using AJL_ChessEngine;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using SimpleGameTree;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AJL_ChessProgram
{
    using plm = Dictionary<Tuple<byte, byte>, FigID>;

    //! White always maximises, Black always minimises !
    class MinMax
    {
        public MinMax(Board Gameboard, MovementLogic Logic, TranspositionTable TransTable)
        {
            this.Gameboard = Gameboard;
            this.Logic = Logic;
            this.TransTable = TransTable;
            this.PruneOpt = new PruneOptimisation(Gameboard, Logic);
        }

        Board Gameboard;
        MovementLogic Logic;
        PruneOptimisation PruneOpt;
        //AJL_Tree Tree = new AJL_Tree();
        TranspositionTable TransTable;
        // Initial values of Alpha and Beta 
        private static double MAX = 1000;
        private static double MIN = -1000;
        private List<KeyValuePair<Stack<Move>, double>> evaluatedNodes = new List<KeyValuePair<Stack<Move>, double>>();
        private int maxDepth = 1;
        public byte currentAge { get; set; } = 0;

        (double score, NodeType nodeType) AlphaBetaPruning(int currentDepth, bool maximizingPlayer, double alpha, double beta)
        {
            try
            {
                // Terminating condition. i.e  leaf node is reached 
                if (currentDepth == maxDepth)
                {
                    var leafVal = Evaluation();
                    var nodeType = NodeType.exact;
                    return (leafVal, nodeType);
                }

                if (maximizingPlayer)
                {
                    return Iteration(currentDepth, maximizingPlayer, alpha, beta);
                }
                else
                {
                    return Iteration(currentDepth, maximizingPlayer, alpha, beta);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return (0, NodeType.unkown);
            }
            
        }
        private (double score, NodeType nodeType) Iteration (int currentDepth, bool maximizingPlayer, double alpha, double beta)
        {
            double best = maximizingPlayer ? MIN : MAX;
            bool allChildNodesAreExact = true;
            //var parentNode = new AJL_Node(); parentNode.depth = currentDepth; parentNode.nodeID = MovesAsIdentifier();
            //parentNode.parentID = Tree.depthCollection[currentDepth - 1].Last().nodeID; parentNode.content = Gameboard.LoggedMoves.myClone();
            //Tree.AddNode(parentNode);
            foreach (var optMove in PruneOpt.OrderedMoves(maximizingPlayer))
            {
                //---------------------------------
                // Play actual move:
                //---------------------------------
                Gameboard.TryMove(optMove);
                var stateIdentifier = StaticStateAsIdentifier();
                //Only proceed in tree if state has not be visited through other nodes:
                var foundTransposition = TransTable.TryGet(currentDepth, maxDepth, stateIdentifier);
                if(foundTransposition.successful)
                {
                    if (foundTransposition.trsp.nodeType == NodeType.exact)
                    {
                        if (maximizingPlayer)
                        {
                            best = Math.Max(best, foundTransposition.trsp.score);
                            alpha = Math.Max(alpha, best);
                        }
                        else
                        {
                            best = Math.Min(best, foundTransposition.trsp.score);
                            beta = Math.Min(beta, best);
                        }

                        if (beta <= alpha)
                        {
                            Gameboard.TryRevertLastMove();
                            return (best, NodeType.lowerBound);
                        }
                        Gameboard.TryRevertLastMove();
                        continue;
                    }
                }

                //---------------------------------
                // Go to greater depth and save in Transposition Table:
                //---------------------------------
                var val = AlphaBetaPruning(currentDepth + 1, (!maximizingPlayer), alpha, beta);
                //var node = new AJL_Node(); node.nodeID = MovesAsIdentifier(); node.parentID = parentNode.nodeID;
                //    node.score = val; node.depth = currentDepth + 1; node.content = Gameboard.LoggedMoves.myClone();
                //Tree.AddNode(node);

                //Only save exact or lower bound nodes:
                if (val.nodeType == NodeType.exact) //|| val.nodeType == NodeType.lowerBound)
                {
                    var newTransposition = new Transposition(nodeType: val.nodeType,
                    distanceFromLeaf: (byte)(maxDepth - currentDepth - 1), age: currentAge, score: val.score);
                    TransTable.TryAdd(newTransposition, stateIdentifier);
                }
                //Parent node can not be exact, if one of the children hasn't been:
                if (val.nodeType != NodeType.exact) { allChildNodesAreExact = false; }

                //Clean up:
                Gameboard.TryRevertLastMove();

                if (maximizingPlayer)
                {
                    best = Math.Max(best, val.score);
                    alpha = Math.Max(alpha, best);
                }
                else
                {
                    best = Math.Min(best, val.score);
                    beta = Math.Min(beta, best);
                }
                //---------------------------------
                // Alpha Beta Pruning:
                //---------------------------------
                if (beta <= alpha)
                {
                    if (currentDepth <= 2)
                    {
                        evaluatedNodes.Add(new KeyValuePair<Stack<Move>, double>(Gameboard.LoggedMoves.myClone(), best));
                    }
                    return (best, NodeType.lowerBound);
                }

            }
            //All moves have been exhausted.
            if (currentDepth <= 2)
            {
                evaluatedNodes.Add(new KeyValuePair<Stack<Move>, double>(Gameboard.LoggedMoves.myClone(), best));
            }

            if (allChildNodesAreExact)
            {
                return (best, NodeType.exact);
            }
            else
            {
                return (best, NodeType.higherBound);
            }
        }

        public (Move bestMove, Move bestCounterMove) CalculateBestMove(int maxDepth, bool isWhitePlayer)
        {
            var watch = new Stopwatch();
            watch.Start();
            //Prevent 0 or negative depths:
            if (this.maxDepth < maxDepth) { this.maxDepth = maxDepth; }
            var bestValue = AlphaBetaPruning(0, isWhitePlayer, MIN, MAX);

            //Best move is first(!) one in order of evaluation with best value:
            var bestMove = evaluatedNodes.Where(y => y.Key.Count == 1).First(x => x.Value == bestValue.score).Key.First();
            var temp = evaluatedNodes.Where(y => y.Key.Count == 2 && y.Key.Last().movedID == bestMove.movedID).First();
            var bestCounterMove = temp.Key.Peek();
            //var bestCounterMove = temp.Where(x => x.Key.Peek().movedID == bestMove.Peek().movedID).ToList();

            watch.Stop();
            Console.WriteLine("Calculated move in " + watch.ElapsedMilliseconds.ToString() + " ms.");

            //Clear nodes:
            evaluatedNodes.Clear();
            //Tree.RemoveNode(1);
            return (bestMove, bestCounterMove);
        }

        private double Evaluation()
        {
            double wWorth = 0, bWorth = 0;
            foreach (var figID in Gameboard.state.WhiteFigurePlacement.Values)
            {
                wWorth += Logic.IDtoFigure[figID].worth;
            }
            foreach (var figID in Gameboard.state.BlackFigurePlacement.Values)
            {
                bWorth += Logic.IDtoFigure[figID].worth;
            }
            return (wWorth - bWorth);
        }

        /// <summary>
        /// Is unique for a unique state on the board. If the order of the moves is changed, it still must return the same identifier, as the state on the board will be the same.
        /// </summary>
        private ulong MovesAsIdentifier()
        {
            ulong evenMoves = 0, oddMoves = 0, tempUlong;
            const ulong firstMagn = 1_00, secondMagn = 1_00_00, thirdMagn = 1_00_00_00;
            var loggedMovesCopy = Gameboard.LoggedMoves.ToArray();
            for (int i = 0; i < loggedMovesCopy.Length; i++)
            {
                tempUlong = (ulong)((double)(loggedMovesCopy[i].oldPos.Item1 + loggedMovesCopy[i].oldPos.Item2 * 10) + ((byte)loggedMovesCopy[i].movedID) * firstMagn +
                        (double)(loggedMovesCopy[i].newPos.Item1 + loggedMovesCopy[i].newPos.Item2 * 10) * secondMagn + ((byte)loggedMovesCopy[i].removedID) * thirdMagn);
                //Adding values guarantees same outcome if moves of same player are switched.
                if (i % 2 == 0)
                {
                    evenMoves += tempUlong;
                }
                else
                {
                    oddMoves += tempUlong;
                }
            }
            //Mulitiplication makes Identifier large and symmetric (factors could have been switched), and substraction of factors prevents same coincidental product by different factors.
            return (oddMoves * evenMoves - oddMoves - evenMoves) + 1;
        }
        private ulong StaticStateAsIdentifier()
        {

            ulong wState = 1, bState = 1;
            const ulong firstMagn = 1_00, secondMagn = 1_00_00, thirdMagn = 1_00_00_00;
            foreach (var plac in Gameboard.state.WhiteFigurePlacement)
            {
                wState *= (byte)plac.Value * thirdMagn + plac.Key.Item1 * secondMagn + plac.Key.Item2 * firstMagn + (byte)plac.Value;
            }
            foreach (var plac in Gameboard.state.BlackFigurePlacement)
            {
                bState *= (byte)plac.Value * thirdMagn + plac.Key.Item1 * secondMagn + plac.Key.Item2 * firstMagn + (byte)plac.Value;
            }
            return (wState + bState);
        }





    }
}
