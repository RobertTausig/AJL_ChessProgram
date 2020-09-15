using System;
using System.Collections.Generic;

namespace SimpleGameTree
{
    public class AJL_Node
    {

        public ulong parentID { get; set; }
        public ulong nodeID { get; set; }
        public List<ulong> childIDs = new List<ulong>();

        public object content { get; set; }
        public double score { get; set; }
        public int depth { get; set; }

    }
}
