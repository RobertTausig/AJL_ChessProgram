using System;
using System.Collections.Generic;

namespace SimpleGameTree
{
    public class AJL_Tree
    {
        public AJL_Tree()
        {
            MasterNode = new AJL_Node();
            MasterNode.depth = -1;
            MasterNode.parentID = ulong.MaxValue;
            MasterNode.nodeID = ulong.MinValue;
            identCollection.Add(MasterNode.nodeID, MasterNode);
            depthCollection.Add(MasterNode.depth, new List<AJL_Node>() { MasterNode });
        }

        private AJL_Node MasterNode;
        private ulong uniqueCounter = ulong.MinValue + 1;
        public readonly Dictionary<ulong, AJL_Node> identCollection = new Dictionary<ulong, AJL_Node>();
        public readonly Dictionary<int, List<AJL_Node>> depthCollection = new Dictionary<int, List<AJL_Node>>();

        public void AddNode(ulong parentID)
        {
            if (identCollection.ContainsKey(parentID))
            {
                var parentNode = identCollection[parentID];
                var addedNode = new AJL_Node();
                addedNode.depth = parentNode.depth + 1;
                addedNode.parentID = parentID;
                addedNode.nodeID = uniqueCounter;
                parentNode.childIDs.Add(addedNode.nodeID);

                //Add in dictionaries:
                identCollection.Add(addedNode.nodeID, addedNode);
                if (depthCollection.ContainsKey(addedNode.depth))
                {
                    depthCollection[addedNode.depth].Add(addedNode);
                }
                else
                {
                    depthCollection.Add(addedNode.depth, new List<AJL_Node>() { addedNode });
                }

                uniqueCounter++;
            }
            else
            {
                throw new ArgumentException("ParentID doesn't exist.");
            }
        }
        public void AddNode(AJL_Node node)
        {
            if (identCollection.ContainsKey(node.parentID))
            {
                var parentNode = identCollection[node.parentID];
                //node.depth = parentNode.depth + 1;    //Can be commented in if you don't want to manage depth yourself.
                if (!parentNode.childIDs.Contains(node.nodeID))
                {
                    parentNode.childIDs.Add(node.nodeID);
                }

                //Add in dictionaries:
                if (identCollection.TryAdd(node.nodeID, node))
                {
                    if (depthCollection.ContainsKey(node.depth))
                    {
                        depthCollection[node.depth].Add(node);
                    }
                    else
                    {
                        depthCollection.Add(node.depth, new List<AJL_Node>() { node });
                    }
                }
                else
                {
                    //Already there. Just change score as the first initialisation will have the default value:
                    identCollection[node.nodeID].score = node.score;
                }

                uniqueCounter++;
            }
            else
            {
                throw new ArgumentException("ParentID doesn't exist.");
            }
        }
        public void RemoveNode(ulong nodeID)
        {
            if (identCollection.ContainsKey(nodeID))
            {
                var parentNode = identCollection[identCollection[nodeID].parentID];
                var removedNode = identCollection[nodeID];
                parentNode.childIDs.Remove(removedNode.nodeID);
                //All children of removedNode will be recursively removed:
                RecursivelyRemoveChilds(nodeID, removedNode.depth);

                //Remove in dictionaries:
                depthCollection[removedNode.depth].Remove(identCollection[nodeID]);
                identCollection.Remove(nodeID);
            }
            else
            {
                throw new ArgumentException("ParentID doesn't exist.");
            }
        }
        private void RecursivelyRemoveChilds(ulong nodeID, int depth)
        {
            foreach (var child in identCollection[nodeID].childIDs)
            {
                RecursivelyRemoveChilds(child, depth + 1);
                depthCollection[depth + 1].Remove(identCollection[child]);
                identCollection.Remove(child);
            }
        }

        //-------------------------------------------------
        // Not sure if I will ever need this:
        //-------------------------------------------------
        public AJL_Node GetParentNode(AJL_Node currentNode)
        {
            if (identCollection.ContainsKey(currentNode.nodeID))
            {
                return identCollection[currentNode.parentID];
            }
            else
            {
                throw new ArgumentException("ParentNode doesn't exist.");
            }
        }
        public List<AJL_Node> GetChildNodes(AJL_Node currentNode)
        {
            var retVal = new List<AJL_Node>();
            if (identCollection.ContainsKey(currentNode.nodeID))
            {
                foreach (var child in currentNode.childIDs)
                {
                    retVal.Add(identCollection[child]);
                }
                return retVal;
            }
            else
            {
                throw new ArgumentException("ParentNode doesn't exist.");
            }
        }



    }
}
