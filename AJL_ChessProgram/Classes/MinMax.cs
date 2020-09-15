﻿using AJL_ChessEngine;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using SimpleGameTree;
using System.Diagnostics;
using AJL_ChessProgram.Classes;

namespace AJL_ChessProgram
{
    using plm = Dictionary<Tuple<byte, byte>, FigID>;

    //! White always maximises, Black always minimises !
    class MinMax
    {
        public MinMax(Board Gameboard, MovementLogic Logic)
        {
            this.Gameboard = Gameboard;
            this.Logic = Logic;
            this.PruneOpt = new PruneOptimisation(Gameboard, Logic);
        }

        Board Gameboard;
        MovementLogic Logic;
        PruneOptimisation PruneOpt;
        //AJL_Tree Tree = new AJL_Tree();
        Dictionary<ulong, (bool isUpperLimit, byte distanceFromLeaf, byte age, double score)> visitedBoardStates =
            new Dictionary<ulong, (bool isUpperLimit, byte distanceFromLeaf, byte age, double score)>();
        // Initial values of Alpha and Beta 
        private static double MAX = 1000;
        private static double MIN = -1000;
        private List<KeyValuePair<Stack<Move>, double>> evaluatedNodes = new List<KeyValuePair<Stack<Move>, double>>();
        private int maxDepth = 1;
        public byte currentAge { get; set; } = 0;

        double AlphaBetaPruning(int currentDepth, bool maximizingPlayer, double alpha, double beta)
        {
            try
            {
                // Terminating condition. i.e  leaf node is reached 
                if (currentDepth == maxDepth)
                {
                    var leafVal = Evaluation();
                    return leafVal;
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
                return 0;
            }
            
        }
        private double Iteration (int currentDepth, bool maximizingPlayer, double alpha, double beta)
        {
            double best = maximizingPlayer ? MIN : MAX;
            //var parentNode = new AJL_Node(); parentNode.depth = currentDepth; parentNode.nodeID = MovesAsIdentifier();
            //parentNode.parentID = Tree.depthCollection[currentDepth - 1].Last().nodeID; parentNode.content = Gameboard.LoggedMoves.myClone();
            //Tree.AddNode(parentNode);
            foreach (var optMove in PruneOpt.OrderedMoves(maximizingPlayer))
            {
                //---------------------------------
                // Visited State Pruning:
                // (Only proceed in tree if state has not be visited through other nodes)
                //---------------------------------
                

                //---------------------------------
                // Play actual move:
                //---------------------------------
                Gameboard.TryMove(optMove);
                var stateIdentifier = StaticStateAsIdentifier();
                if (visitedBoardStates.ContainsKey(stateIdentifier) &&
                    (visitedBoardStates[stateIdentifier].distanceFromLeaf + currentDepth) >= maxDepth)
                {
                    if (visitedBoardStates[stateIdentifier].isUpperLimit)
                    {
                        best = Math.Max(best, visitedBoardStates[stateIdentifier].score);
                        alpha = Math.Max(alpha, best);
                    }
                    else
                    {
                        best = Math.Min(best, visitedBoardStates[stateIdentifier].score);
                        beta = Math.Min(beta, best);
                    }
                    Gameboard.TryRevertLastMove();
                    continue;
                }

                var val = AlphaBetaPruning(currentDepth + 1, (!maximizingPlayer), alpha, beta);
                //var node = new AJL_Node(); node.nodeID = MovesAsIdentifier(); node.parentID = parentNode.nodeID;
                //    node.score = val; node.depth = currentDepth + 1; node.content = Gameboard.LoggedMoves.myClone();
                //Tree.AddNode(node);

                if (!visitedBoardStates.ContainsKey(stateIdentifier))
                {
                    //Node is unkown. Add:
                    visitedBoardStates.Add(stateIdentifier,
                        (isUpperLimit: maximizingPlayer, distanceFromLeaf: (byte)(maxDepth - currentDepth), age: currentAge, score: val));
                }
                else if (visitedBoardStates[stateIdentifier].distanceFromLeaf < (maxDepth - currentDepth))
                {
                    //Same node with a deeper information was found. Replace old one:
                    visitedBoardStates[stateIdentifier] =
                        (isUpperLimit: maximizingPlayer, distanceFromLeaf: (byte)(maxDepth - currentDepth), age: currentAge, score: val);
                }
                //Clean up:
                Gameboard.TryRevertLastMove();

                if (maximizingPlayer)
                {
                    best = Math.Max(best, val);
                    alpha = Math.Max(alpha, best);
                }
                else
                {
                    best = Math.Min(best, val);
                    beta = Math.Min(beta, best);
                }
                //---------------------------------
                // Alpha Beta Pruning:
                //---------------------------------
                if (beta <= alpha)
                {
                    if (currentDepth == 1)
                    {
                        evaluatedNodes.Add(new KeyValuePair<Stack<Move>, double>(Gameboard.LoggedMoves.myClone(), best));
                    }
                    return best;
                }

            }
            if (currentDepth == 1)
            {
                evaluatedNodes.Add(new KeyValuePair<Stack<Move>, double>(Gameboard.LoggedMoves.myClone(), best));
            }
            return best;
        }

        public Move CalculateBestMove(int maxDepth, bool isWhitePlayer)
        {
            var watch = new Stopwatch();
            watch.Start();
            //Prevent 0 or negative depths:
            if (this.maxDepth < maxDepth) { this.maxDepth = maxDepth; }
            var bestValue = AlphaBetaPruning(0, isWhitePlayer, MIN, MAX);

            //Best move is first(!) one in order of evaluation with best value:
            var bestMove = evaluatedNodes.First(x => x.Value == bestValue).Key;

            watch.Stop();
            Console.WriteLine("Calculated move in " + watch.ElapsedMilliseconds.ToString() + " ms.");
            //Clear nodes:
            evaluatedNodes.Clear();
            //Tree.RemoveNode(1);
            return bestMove.Peek();
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