using System;
using System.Linq;

namespace SimpleGameTree
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new AJL_TreeMaker();
            a.MakeTree(6, 6, 10);
            var bb = a.Tree.GetChildNodes(a.Tree.identCollection[766]);
            var cc = a.Tree.GetParentNode(bb.First());


        }
    }
}
